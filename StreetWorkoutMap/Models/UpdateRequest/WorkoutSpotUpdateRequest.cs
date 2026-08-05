using StreetWorkoutMap.Data;

namespace StreetWorkoutMap.Models.UpdateRequest
{
    public class WorkoutSpotUpdateRequest
    {
        public Guid Id { get; set; }

        public Guid WorkoutSpotId { get; set; }

        public WorkoutSpot WorkoutSpot { get; set; } = null!;

        //General
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;

        //Location
        public string City { get; set; } = string.Empty;
        public string District { get; set; } = string.Empty;
        public double Latitude { get; set; }
        public double Longitude { get; set; }

        //Media
        public ICollection<WorkoutSpotUpdateImage> Images { get; set; } = [];

        //Equipment
        public bool HasPullUpBars { get; set; }

        public bool HasParallelBars { get; set; }

        public bool HasRings { get; set; }

        //Features
        public bool HasLighting { get; set; }

        public bool IsIndoor { get; set; }


        //Other
        public string SubmittedByUserId { get; set; } = string.Empty;

        public ApplicationUser? SubmittedByUser { get; set; } = null!;

        public DateTime CreatedOn { get; set; } = DateTime.UtcNow;
    }
}
