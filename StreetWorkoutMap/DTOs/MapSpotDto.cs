namespace StreetWorkoutMap.DTOs
{
    public class MapSpotDto
    {
        public Guid Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public string City { get; set; } = string.Empty;

        public string District { get; set; } = string.Empty;

        public double Latitude { get; set; }

        public double Longitude { get; set; }

        public string? ImageUrl { get; set; }

        public bool HasPullUpBars { get; set; }

        public bool HasParallelBars { get; set; }

        public bool HasRings { get; set; }

        public bool HasLighting { get; set; }

        public bool IsIndoor { get; set; }

        public double? Rating { get; set; }
    }
}
