using Microsoft.EntityFrameworkCore;
using StreetWorkoutMap.Data;
using StreetWorkoutMap.DTOs.WorkoutSpot;
using StreetWorkoutMap.Models;
using StreetWorkoutMap.Models.UpdateRequest;
using StreetWorkoutMap.Services.Contrancts;
using StreetWorkoutMap.Services.ImageStorage;

namespace StreetWorkoutMap.Services
{
    public class WorkoutSpotUpdateRequestService
        : IWorkoutSpotUpdateRequestService
    {
        private readonly ApplicationDbContext dbContext;
        private readonly IImageStorageService imageStorageService;

        public WorkoutSpotUpdateRequestService(
            ApplicationDbContext dbContext,
            IImageStorageService imageStorageService)
        {
            this.dbContext = dbContext;
            this.imageStorageService = imageStorageService;
        }

        public async Task<ICollection<PendingRequestDto>> GetPendingRequestsAsync()
        {
            return await dbContext.WorkoutSpotsUpdateRequests
                .AsNoTracking()
                .OrderBy(request => request.Name)
                .Select(request => new PendingRequestDto
                {
                    Id = request.Id,
                    Name = request.Name,
                    City = request.City,
                    District = request.District,

                    SubmittedByName = (request.SubmittedByUser!.FirstName + " " + request.SubmittedByUser.LastName).Trim(),

                    ImageUrl = request.Images
                            .Select(img => img.StoragePath)
                            .Select(path => imageStorageService.GetPublicUrl(path))
                            .FirstOrDefault(),

                    IsUpdateRequest = true

                })
                .ToListAsync();
        }

        public async Task<int> GetPendingRequestsCountAsync()
        {
            return await dbContext.WorkoutSpotsUpdateRequests
                    .CountAsync();
        }

        public async Task SubmitAsync(
            EditSpotDto dto,
            WorkoutSpot spot,
            string userId)
        {
            var hasPendingUpdateRequest =
                await dbContext.WorkoutSpotsUpdateRequests
                    .AnyAsync(request =>
                        request.WorkoutSpotId == spot.Id);

            if (hasPendingUpdateRequest)
            {
                throw new InvalidOperationException(
                    "За тази площадка вече има чакаща заявка за редакция.");
            }

            var updateRequest = new WorkoutSpotUpdateRequest
            {
                Id = Guid.NewGuid(),

                WorkoutSpotId = spot.Id,

                Name = dto.Name.Trim(),
                Description = dto.Description.Trim(),

                City = dto.City.Trim(),
                District = dto.District.Trim(),
                Latitude = dto.Latitude,
                Longitude = dto.Longitude,

                HasPullUpBars = dto.HasPullUpBars,
                HasParallelBars = dto.HasParallelBars,
                HasRings = dto.HasRings,

                HasLighting = dto.HasLighting,
                IsIndoor = dto.IsIndoor,

                SubmittedByUserId = userId,
                CreatedOn = DateTime.UtcNow
            };

            var imageIdsToDelete = dto.ImagesToDelete
                .Distinct()
                .ToHashSet();

            var keptImages = spot.Images
                .Where(image =>
                    !imageIdsToDelete.Contains(image.Id))
                .ToList();

            var uploadedPaths = new List<string>();

            try
            {
                if (dto.NewImages.Count > 0)
                {
                    uploadedPaths =
                        await imageStorageService.UploadImagesAsync(
                            updateRequest.Id,
                            dto.NewImages);
                }

                foreach (var image in keptImages)
                {
                    updateRequest.Images.Add(
                        new WorkoutSpotUpdateImage
                        {
                            Id = Guid.NewGuid(),
                            StoragePath = image.StoragePath
                        });
                }

                foreach (var path in uploadedPaths)
                {
                    updateRequest.Images.Add(
                        new WorkoutSpotUpdateImage
                        {
                            Id = Guid.NewGuid(),
                            StoragePath = path
                        });
                }

                await dbContext.WorkoutSpotsUpdateRequests
                    .AddAsync(updateRequest);

                await dbContext.SaveChangesAsync();
            }
            catch
            {
                if (uploadedPaths.Count > 0)
                {
                    try
                    {
                        await imageStorageService
                            .DeleteImagesAsync(uploadedPaths);
                    }
                    catch
                    {
                        // TODO: ILogger for orphan files
                    }
                }

                throw;
            }
        }
    }
}