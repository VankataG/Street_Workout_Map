namespace StreetWorkoutMap.Models
{
    public class SpotImage
    {
        public Guid Id { get; set; }
        public string StoragePath { get; set; } = string.Empty;
        public Guid WorkoutSpotId { get; set; }
        public WorkoutSpot WorkoutSpot { get; set; } = null!;
    }
}
