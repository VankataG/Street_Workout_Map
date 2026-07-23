using StreetWorkoutMap.Models;

namespace StreetWorkoutMap.DTOs.WorkoutSpot
{
    public class MySpotDto
    {
        public Guid Id { get; set; }
        
        public string Name { get; set; } = string.Empty;
        
        public string City { get; set; } = string.Empty;
        
        public string District { get; set; } = string.Empty;

        public string? ImageUrl { get; set; }
        
        public SpotStatus Status { get; set; }
    }
}
