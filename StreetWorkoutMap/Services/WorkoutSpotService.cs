using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using StreetWorkoutMap.Data;
using StreetWorkoutMap.DTOs;
using StreetWorkoutMap.DTOs.WorkoutSpot;
using StreetWorkoutMap.Models;
using StreetWorkoutMap.Services.Contrancts;
using StreetWorkoutMap.Services.ImageStorage;
using System.Collections;
using System.Security.Claims;

namespace StreetWorkoutMap.Services
{
    public class WorkoutSpotService : IWorkoutSpotService
    {
        private readonly ApplicationDbContext dbContext;

        private readonly UserManager<ApplicationUser> userManager;

        private readonly IImageStorageService imageStorageService;


        public WorkoutSpotService(ApplicationDbContext dbContext, UserManager<ApplicationUser> userManager, IImageStorageService imageStorageService)
        {
            this.dbContext = dbContext;
            this.userManager = userManager;
            this.imageStorageService = imageStorageService;
        }

        public async Task<ICollection<MapSpotDto>> GetAllApprovedAsync()
        {
            return await dbContext.WorkoutSpots
                .Include(spot => spot.Images)
                .Where(spot => spot.Status == SpotStatus.Approved)
                .Select(spot => new MapSpotDto
                {
                    Id = spot.Id,
                    Name = spot.Name,
                    Description = spot.Description,
                    City = spot.City,
                    District = spot.District,
                    Latitude = spot.Latitude,
                    Longitude = spot.Longitude,
                    HasPullUpBars = spot.HasPullUpBars,
                    HasParallelBars = spot.HasParallelBars,
                    HasRings = spot.HasRings,
                    HasLighting = spot.HasLighting,
                    IsIndoor = spot.IsIndoor,
                    Rating = null,
                    ImageUrl = spot.Images
                        .Select(image => image.StoragePath)
                        .Select(path => imageStorageService.GetPublicUrl(path))
                        .FirstOrDefault()
                })
                .ToListAsync();
        }

        public async Task CreateAsync(CreateSpotDto dto, ClaimsPrincipal user)
        {
            var userId = userManager.GetUserId(user);

            if (userId is null)
            {
                throw new UnauthorizedAccessException("Потребителят трябва да е влязъл в профила си.");
            }

            var workoutSpot = new WorkoutSpot
            {
                Id = Guid.NewGuid(),
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

                Status = SpotStatus.Pending,
                SubmittedByUserId = userId
            };


            var uploadedPaths = new List<string>();
            try
            {
                uploadedPaths = await imageStorageService.UploadImagesAsync(workoutSpot.Id, dto.Images);

                var images = uploadedPaths
                    .Select(path => new SpotImage
                    {
                        WorkoutSpotId = workoutSpot.Id,
                        StoragePath = path
                    })
                    .ToList();

                await dbContext.SpotImages.AddRangeAsync(images);
                await dbContext.WorkoutSpots.AddAsync(workoutSpot);
                await dbContext.SaveChangesAsync();

            }
            catch
            {
                if (uploadedPaths.Count > 0)
                {
                    await imageStorageService.DeleteImagesAsync(uploadedPaths);
                }
                
                throw;
            }
            
        }

        public async Task<SpotDetailsDto?> GetDetailsAsync(Guid id, ClaimsPrincipal user)
        {
             var spot = await dbContext.WorkoutSpots
                        .AsNoTracking()
                        .Include(spot => spot.Images)
                        .FirstOrDefaultAsync(spot => spot.Id == id);

            if (spot is null) return null;

            var currentUserId = userManager.GetUserId(user);

            var isAdmin = user.Identity?.IsAuthenticated == true && user.IsInRole("Admin");
            var isOwner = currentUserId is not null && spot.SubmittedByUserId == currentUserId;


            var isApproved = spot.Status == SpotStatus.Approved;

            if (!isApproved && !isAdmin && !isOwner)  return null;
            

            var canEdit = isAdmin || isOwner;

            return new SpotDetailsDto
            {
                Id = spot.Id,
                Name = spot.Name,
                Description = spot.Description,
                City = spot.City,
                District = spot.District,
                Latitude = spot.Latitude,
                Longitude = spot.Longitude,
                HasPullUpBars = spot.HasPullUpBars,
                HasParallelBars = spot.HasParallelBars,
                HasRings = spot.HasRings,
                HasLighting = spot.HasLighting,
                IsIndoor = spot.IsIndoor,

                Status = spot.Status.ToString(),
                SubmittedByUserId = spot.SubmittedByUserId,

                ImageUrls = spot.Images
                            .Select(img => imageStorageService.GetPublicUrl(img.StoragePath))
                            .ToList(),

                CanEdit = canEdit,
                IsAdmin = isAdmin,
                IsOwner = isOwner
            };
        }

        public async Task<EditSpotDto?> GetForEditAsync(Guid id, ClaimsPrincipal user)
        {
            var spot = await dbContext.WorkoutSpots
                        .AsNoTracking()
                        .Include(spot => spot.Images)
                        .FirstOrDefaultAsync(spot => spot.Id == id);


            if (spot is null) return null;

            var currentUserId = userManager.GetUserId(user);

            var isAdmin = user.Identity?.IsAuthenticated == true && user.IsInRole("Admin");
            var isOwner = currentUserId is not null && spot.SubmittedByUserId == currentUserId;

            if (!isAdmin && !isOwner) return null;

            return new EditSpotDto
            {
                Id = spot.Id,
                Name = spot.Name,
                Description = spot.Description,
                City = spot.City,
                District = spot.District,
                Latitude = spot.Latitude,
                Longitude = spot.Longitude,
                HasPullUpBars = spot.HasPullUpBars,
                HasParallelBars = spot.HasParallelBars,
                HasRings = spot.HasRings,
                HasLighting = spot.HasLighting,
                IsIndoor = spot.IsIndoor,

                ExistingImages = spot.Images
                            .Select(img => new ExistingImageDto 
                            { 
                                Id = img.Id, 
                                Url = imageStorageService.GetPublicUrl(img.StoragePath)
                            })
                            .ToList()


            };

        }

        public async Task EditAsync(EditSpotDto dto, ClaimsPrincipal user)
        {
            var spot = await dbContext.WorkoutSpots
                        .Include(s => s.Images)
                        .FirstOrDefaultAsync(s => s.Id == dto.Id);

            if (spot is null)
            {
                throw new InvalidOperationException("Workout spot not found.");
            }


            var currentUserId = userManager.GetUserId(user);

            var isAdmin =
                user.Identity?.IsAuthenticated == true &&
                user.IsInRole("Admin");

            var isOwner =
                currentUserId != null &&
                spot.SubmittedByUserId == currentUserId;

            if (!isAdmin && !isOwner)
            {
                throw new UnauthorizedAccessException();
            }


            spot.Name = dto.Name.Trim();
            spot.Description = dto.Description.Trim();
            spot.City = dto.City.Trim();
            spot.District = dto.District.Trim();

            spot.Latitude = dto.Latitude;
            spot.Longitude = dto.Longitude;

            spot.HasPullUpBars = dto.HasPullUpBars;
            spot.HasParallelBars = dto.HasParallelBars;
            spot.HasRings = dto.HasRings;
            spot.HasLighting = dto.HasLighting;
            spot.IsIndoor = dto.IsIndoor;

            if (!isAdmin)
            {
                spot.Status = SpotStatus.Pending;
            }


            //Photo logic

            await dbContext.SaveChangesAsync();

        }
    }
}
