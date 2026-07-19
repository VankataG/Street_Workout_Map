namespace StreetWorkoutMap.DTOs.WorkoutSpot
{
    public class EditSpotDto
    {
        public Guid Id { get; set; }

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
        public bool HasLighting { get; set; }
        public bool IsIndoor { get; set; }

        // Existing images
        public List<ExistingImageDto> ExistingImages { get; set; } = [];

        // Images to delete
        public List<Guid> ImagesToDelete { get; set; } = [];

        // New images
        public List<IFormFile> NewImages { get; set; } = [];
    }
}
