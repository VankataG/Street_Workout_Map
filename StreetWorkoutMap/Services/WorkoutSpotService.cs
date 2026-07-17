using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using StreetWorkoutMap.Data;
using StreetWorkoutMap.DTOs;
using StreetWorkoutMap.DTOs.WorkoutSpot;
using StreetWorkoutMap.Models;
using StreetWorkoutMap.Services.ImageStorage;
using System.Collections;
using System.Security.Claims;

namespace StreetWorkoutMap.Services
{
    public class WorkoutSpotService
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
    }
}
