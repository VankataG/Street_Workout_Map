using StreetWorkoutMap.Data;
using StreetWorkoutMap.Models;

namespace StreetWorkoutMap.DTOs.WorkoutSpot
{
    public class SpotDetailsDto
    {
        public Guid Id { get; set; }

        //General
        public string Name { get; set; } = null!;
        public string? Description { get; set; }

        //Location
        public string City { get; set; } = null!;
        public string? District { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }

        //Media
        public List<string> ImageUrls { get; set; } = [];

        //Equipment
        public bool HasPullUpBars { get; set; }

        public bool HasParallelBars { get; set; }

        public bool HasRings { get; set; }

        //Features
        public bool HasLighting { get; set; }

        public bool IsIndoor { get; set; }

        public string Status { get; set; } = null!;

        //Other
        public string? SubmittedByUserId { get; set; }

        public string? SubmittedByUser { get; set; }

        public bool CanEdit { get; set; }

        public bool IsOwner { get; set; }

        public bool IsAdmin { get; set; }
    }
}
