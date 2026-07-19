namespace StreetWorkoutMap.Constants;

public static class SpotConstants
{
    public const int MaxImages = 3;

    public const long MaxImageSizeBytes = 5 * 1024 * 1024;

    public static readonly string[] AllowedImageContentTypes =
    [
        "image/jpeg",
        "image/png",
        "image/webp"
    ];
}