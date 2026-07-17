namespace StreetWorkoutMap.Services.ImageStorage
{
    public interface IImageStorageService
    {
        Task<List<string>> UploadImagesAsync(Guid workoutSpotId, IEnumerable<IFormFile> images);

        Task DeleteImagesAsync(IEnumerable<string> paths);
    }
}
