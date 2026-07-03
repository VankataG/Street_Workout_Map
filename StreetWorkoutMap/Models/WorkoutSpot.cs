namespace StreetWorkoutMap.Models
{
    public class WorkoutSpot
    {
        public string Name { get; set; } = string.Empty;

        public double Latitude { get; set; }

        public double Longitude { get; set; }

        public string Description { get; set; } = string.Empty;

        public int Rating { get; set; }

        public bool HasParallelBars { get; set; }

        public bool HasRings { get; set; }
    }
}
