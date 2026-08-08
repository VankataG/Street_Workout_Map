namespace StreetWorkoutMap.DTOs.WorkoutSpot
{
    public class PendingRequestDto
    {
        public Guid Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public string City { get; set; } = string.Empty;

        public string District { get; set; } = string.Empty;

        public string? ImageUrl { get; set; }

        public string SubmittedByName { get; set; } = string.Empty;

        public bool IsUpdateRequest { get; set; }
    }
}
