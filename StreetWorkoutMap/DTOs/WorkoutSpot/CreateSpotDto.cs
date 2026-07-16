namespace StreetWorkoutMap.DTOs.WorkoutSpot
{
    public class CreateSpotDto
    {
        // General
        public string Name { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        // Location
        public string City { get; set; } = string.Empty;

        public string District { get; set; } = string.Empty;

        public double Latitude { get; set; }

        public double Longitude { get; set; }

        // Equipment
        public bool HasPullUpBars { get; set; }

        public bool HasParallelBars { get; set; }

        public bool HasRings { get; set; }

        // Features
        public bool HasLighting { get; set; }

        public bool IsIndoor { get; set; }

        public List<IFormFile> Images { get; set; } = [];
    }
}
