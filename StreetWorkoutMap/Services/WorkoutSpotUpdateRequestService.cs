using Microsoft.AspNetCore.Mvc.Routing;
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
        private readonly ILogger<WorkoutSpotUpdateRequestService> logger;

        public WorkoutSpotUpdateRequestService(ApplicationDbContext dbContext, IImageStorageService imageStorageService, ILogger<WorkoutSpotUpdateRequestService> logger)
        {
            this.dbContext = dbContext;
            this.imageStorageService = imageStorageService;
            this.logger = logger;
        }

        public async Task ApproveAsync(Guid requestId)
        {
            var request = await dbContext.WorkoutSpotsUpdateRequests
                        .Include(request => request.Images)
                        .FirstOrDefaultAsync(request => request.Id == requestId);

            if (request is null)
            {
                throw new KeyNotFoundException("Заявката не беше намерена.");
            }


            var spot = await dbContext.WorkoutSpots
                        .Include(spot => spot.Images)
                        .FirstOrDefaultAsync(spot => spot.Id == request.WorkoutSpotId);

            if (spot is null)
            {
                throw new KeyNotFoundException("Оригиналната площадка не беше намерена.");
            }


            var requestedPaths = request.Images
                                    .Select(image => image.StoragePath)
                                    .ToHashSet();

            var imagesToRemove = spot.Images
                                .Where(image => !requestedPaths.Contains(image.StoragePath))
                                .ToList();

            var storagePathsToDelete = imagesToRemove
                                   .Select(image => image.StoragePath)
                                   .Where(path => !string.IsNullOrWhiteSpace(path))
                                   .Distinct()
                                   .ToList();


            var currrentPaths = spot.Images
                                .Select(image => image.StoragePath)
                                .ToHashSet();

            var imagesToAdd = request.Images
                             .Where(image => !currrentPaths.Contains(image.StoragePath))
                             .Select(image => new SpotImage
                             {
                                 Id = Guid.NewGuid(),
                                 WorkoutSpotId = spot.Id,
                                 StoragePath = image.StoragePath
                             })
                             .ToList();


            await using var transaction = await dbContext.Database.BeginTransactionAsync();

            try
            {
                spot.Name = request.Name;
                spot.Description = request.Description;
                spot.City = request.City;
                spot.District = request.District;
                spot.Latitude = request.Latitude;
                spot.Longitude = request.Longitude;

                spot.HasPullUpBars = request.HasPullUpBars;
                spot.HasParallelBars = request.HasParallelBars;
                spot.HasRings = request.HasRings;
                spot.HasLighting = request.HasLighting;
                spot.IsIndoor = request.IsIndoor;

                spot.Status = SpotStatus.Approved;


                if (imagesToRemove.Count > 0)
                {
                    dbContext.SpotImages.RemoveRange(imagesToRemove);
                }


                if (imagesToAdd.Count > 0)
                {
                    await dbContext.SpotImages.AddRangeAsync(imagesToAdd);
                }

                dbContext.WorkoutSpotsUpdateRequests.Remove(request);

                await dbContext.SaveChangesAsync();
                await transaction.CommitAsync();

            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }


            if (storagePathsToDelete.Count > 0)
            {
                try
                {
                    await imageStorageService
                        .DeleteImagesAsync(storagePathsToDelete);
                }
                catch (Exception exception)
                {
                    logger.LogError(
                        exception,
                        "Failed to delete obsolete images after approving update request. RequestId: {RequestId}. Paths: {Paths}",
                        requestId,
                        string.Join(", ", storagePathsToDelete));
                }
            }
        }


        public async Task RejectAsync(Guid requestId)
        {
            var request = await dbContext.WorkoutSpotsUpdateRequests
                        .Include(request => request.Images)
                        .FirstOrDefaultAsync(request => request.Id == requestId);

            if (request is null)
            {
                throw new KeyNotFoundException("Заявката не беше намерена.");
            }


            var spot = await dbContext.WorkoutSpots
                        .Include(spot => spot.Images)
                        .FirstOrDefaultAsync(spot => spot.Id == request.WorkoutSpotId);

            if (spot is null)
            {
                throw new KeyNotFoundException("Оригиналната площадка не беше намерена.");
            }



            var originalPaths = spot.Images
                                .Select(image => image.StoragePath)
                                .ToHashSet();

            var newStoragePaths = request.Images
                                 .Select(image => image.StoragePath)
                                 .Where(path => !originalPaths.Contains(path))
                                 .Where(path => !string.IsNullOrWhiteSpace(path))
                                 .Distinct()
                                 .ToList();

            await using var transaction = await dbContext.Database.BeginTransactionAsync();

            try
            {
                dbContext.WorkoutSpotsUpdateRequests.Remove(request);

                await dbContext.SaveChangesAsync();

                await transaction.CommitAsync();
            }
            catch 
            {
                await transaction.RollbackAsync();
                throw;
            }


            if (newStoragePaths.Count > 0)
            {
                try
                {
                    await imageStorageService
                        .DeleteImagesAsync(newStoragePaths);
                }
                catch (Exception exception)
                {
                    logger.LogError(
                        exception,
                        "Failed to delete newly uploaded images after rejecting update request. RequestId: {RequestId}. Paths: {Paths}",
                        requestId,
                        string.Join(", ", newStoragePaths));
                }
            }


        }

        public async Task<SpotDetailsDto?> GetDetailsAsync(Guid id)
        {
            var request = await dbContext.WorkoutSpotsUpdateRequests
                          .AsNoTracking()
                          .Include(request => request.Images)
                          .FirstOrDefaultAsync(request => request.Id == id);

            if (request is null) return null;

            return new SpotDetailsDto
            {
                Id = request.Id,
                Name = request.Name,
                Description = request.Description,
                City = request.City,
                District = request.District,
                Latitude = request.Latitude,
                Longitude = request.Longitude,

                HasPullUpBars = request.HasPullUpBars,
                HasParallelBars = request.HasParallelBars,
                HasRings = request.HasRings,

                HasLighting = request.HasLighting,
                IsIndoor = request.IsIndoor,

                SubmittedByUserId = request.SubmittedByUserId,

                ImageUrls = request.Images
                        .Select(image => imageStorageService.GetPublicUrl(image.StoragePath))
                        .ToList(),

                Status = "Pending"
            };
        }

        public async Task<Guid?> GetOriginalSpotIdAsync(Guid requestId)
        {
            return await dbContext.WorkoutSpotsUpdateRequests
                    .AsNoTracking()
                    .Where(request => request.Id == requestId)
                    .Select(request => (Guid?)request.WorkoutSpotId)
                    .FirstOrDefaultAsync();
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
                Description = dto.Description?.Trim(),

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
                            spot.Id,
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
                    catch (Exception exception)
                    {
                        logger.LogError(
                            exception,
                            "Failed to delete uploaded images after update request submission failed. RequestId: {RequestId}. Paths: {Paths}",
                            updateRequest.Id,
                            string.Join(", ", uploadedPaths));
                    }
                }

                throw;
            }
        }
    }
}