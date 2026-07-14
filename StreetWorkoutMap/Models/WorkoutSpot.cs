namespace StreetWorkoutMap.Models
{
    public class WorkoutSpot
    {
        public Guid Id { get; set; }

        //General
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;

        //Location
        public string City { get; set; } = string.Empty;
        public string District { get; set; } = string.Empty;
        public double Latitude { get; set; }
        public double Longitude { get; set; }

        //Media
        public ICollection<SpotImage> Images { get; set; } = [];

        //Equipment
        public bool HasPullUpBars { get; set; }

        public bool HasParallelBars { get; set; }

        public bool HasRings { get; set; }

        //Features
        public bool HasLighting { get; set; }

        public bool IsIndoor { get; set; }

        public SpotStatus Status { get; set; } = SpotStatus.Pending;
    }
}
