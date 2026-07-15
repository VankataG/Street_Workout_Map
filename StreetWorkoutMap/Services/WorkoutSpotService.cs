using Microsoft.EntityFrameworkCore;
using StreetWorkoutMap.Data;
using StreetWorkoutMap.DTOs;
using StreetWorkoutMap.Models;
using System.Collections;

namespace StreetWorkoutMap.Services
{
    public class WorkoutSpotService
    {
        private readonly ApplicationDbContext dbContext;
        public WorkoutSpotService(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
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
                    ImageUrl = spot.Images.Select(image => image.StoragePath).FirstOrDefault()
                })
                .ToListAsync();
        }
    }
}
