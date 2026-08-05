namespace StreetWorkoutMap.Models.UpdateRequest
{
    public class WorkoutSpotUpdateImage
    {
        public Guid Id { get; set; }
        public string StoragePath { get; set; } = string.Empty;
        public Guid WorkoutSpotUpdateRequestId { get; set; }
        public WorkoutSpotUpdateRequest WorkoutSpotUpdateRequest { get; set; } = null!;
    }
}
