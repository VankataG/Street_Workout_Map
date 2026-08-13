using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using StreetWorkoutMap.Constants;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace StreetWorkoutMap.Services.ImageStorage;

public class SupabaseImageStorageService : IImageStorageService
{
    private const string BucketName = "workout-spot-images";

    private readonly HttpClient httpClient;
    private readonly string supabaseUrl;
    private readonly string serviceRoleKey;

    public SupabaseImageStorageService(
        HttpClient httpClient,
        IConfiguration configuration)
    {
        this.httpClient = httpClient;

        supabaseUrl = configuration["Supabase:Url"]
            ?? throw new InvalidOperationException(
                "Supabase:Url is missing.");

        serviceRoleKey = configuration["Supabase:ServiceRoleKey"]
            ?? throw new InvalidOperationException(
                "Supabase:ServiceRoleKey is missing.");
    }


    public string GetPublicUrl(string storagePath)
    {
        return $"{supabaseUrl.TrimEnd('/')}/storage/v1/object/public/{BucketName}/{storagePath}";
    }


    private static string SanitizeFileName(string fileName)
    {
        if (string.IsNullOrWhiteSpace(fileName))
        {
            return "image";
        }

        var invalidChars =
            Path.GetInvalidFileNameChars();

        var sanitized = new string(
            fileName
                .Where(character =>
                    !invalidChars.Contains(character))
                .ToArray());

        sanitized = sanitized
            .Trim()
            .Replace(" ", "-");

        if (sanitized.Length > 50)
        {
            sanitized = sanitized[..50];
        }

        return string.IsNullOrWhiteSpace(sanitized)
            ? "image"
            : sanitized;
    }

    public async Task<List<string>> UploadImagesAsync(Guid workoutSpotId, IEnumerable<IFormFile> images)
    {

        var imageList = images.ToList();

        if (imageList.Count > SpotConstants.MaxImages)
        {
            throw new ArgumentException($"Могат да бъдат качени най-много {SpotConstants.MaxImages} снимки.");
        }

        var uploadedPaths = new List<string>();

        try
        {
            foreach (var image in imageList)
            {
                await ValidateImageAsync(image);

                var extension = GetSafeExtension(image);

                var originalName =
                    Path.GetFileNameWithoutExtension(image.FileName);

                var safeOriginalName =
                    SanitizeFileName(originalName);

                var shortId =
                    Guid.NewGuid()
                        .ToString("N")[..8];

                var fileName =
                    $"{safeOriginalName}_{shortId}{extension}";

                var storagePath =
                    $"spots/{workoutSpotId}/{fileName}";

                await UploadImageAsync(image, storagePath);

                uploadedPaths.Add(storagePath);
            }

            return uploadedPaths;
        }
        catch
        {
            if (uploadedPaths.Count > 0)
            {
                await DeleteImagesAsync(uploadedPaths);
            }

            throw;
        }
    }

    public async Task DeleteImagesAsync(
        IEnumerable<string> paths)
    {
        var pathList = paths
            .Where(path => !string.IsNullOrWhiteSpace(path))
            .Distinct()
            .ToList();

        if (pathList.Count == 0)
        {
            return;
        }

        var requestUrl =
            $"{supabaseUrl.TrimEnd('/')}/storage/v1/object/{BucketName}";

        using var request = new HttpRequestMessage(
            HttpMethod.Delete,
            requestUrl);

        AddAuthorizationHeaders(request);

        request.Content = JsonContent.Create(new
        {
            prefixes = pathList
        });

        using var response =
            await httpClient.SendAsync(request);

        if (!response.IsSuccessStatusCode)
        {
            var responseBody =
                await response.Content.ReadAsStringAsync();

            throw new InvalidOperationException(
                $"Supabase image deletion failed. " +
                $"Status: {(int)response.StatusCode}. " +
                $"Response: {responseBody}");
        }
    }

    private async Task UploadImageAsync(
        IFormFile image,
        string storagePath)
    {
        var encodedPath = EncodeStoragePath(storagePath);

        var requestUrl =
            $"{supabaseUrl.TrimEnd('/')}/storage/v1/object/" +
            $"{BucketName}/{encodedPath}";

        using var request = new HttpRequestMessage(
            HttpMethod.Post,
            requestUrl);

        AddAuthorizationHeaders(request);

        request.Headers.Add("x-upsert", "false");

        await using var imageStream = image.OpenReadStream();

        request.Content = new StreamContent(imageStream);

        request.Content.Headers.ContentType =
            new MediaTypeHeaderValue(image.ContentType);

        request.Content.Headers.ContentLength = image.Length;

        using var response =
            await httpClient.SendAsync(request);

        if (!response.IsSuccessStatusCode)
        {
            var responseBody =
                await response.Content.ReadAsStringAsync();

            throw new InvalidOperationException(
                $"Supabase image upload failed for '{image.FileName}'. " +
                $"Status: {(int)response.StatusCode}. " +
                $"Response: {responseBody}");
        }
    }

    private void AddAuthorizationHeaders(
        HttpRequestMessage request)
    {
        request.Headers.Authorization =
            new AuthenticationHeaderValue(
                "Bearer",
                serviceRoleKey);

        request.Headers.Add("apikey", serviceRoleKey);
    }

    private static async Task ValidateImageAsync(IFormFile image)
    {
        if (image.Length == 0)
        {
            throw new ArgumentException(
                "Празен файл не може да бъде качен.");
        }

        if (image.Length > SpotConstants.MaxImageSizeBytes)
        {
            throw new ArgumentException(
                $"Снимката '{image.FileName}' е по-голяма от 5 MB.");
        }

        if (!SpotConstants.AllowedImageContentTypes.Contains(
            image.ContentType,
            StringComparer.OrdinalIgnoreCase))
        {
            throw new ArgumentException(
                $"Форматът на '{image.FileName}' не е позволен.");
        }

        if (!await HasValidImageSignatureAsync(image))
        {
            throw new ArgumentException(
                $"Файлът '{image.FileName}' не е валидно изображение.");
        }
    }

    private static async Task<bool> HasValidImageSignatureAsync(
    IFormFile image)
    {
        var header = new byte[12];

        await using var stream = image.OpenReadStream();

        var bytesRead = await stream.ReadAsync(
            header.AsMemory(0, header.Length));

        if (bytesRead < 12)
        {
            return false;
        }

        return image.ContentType.ToLowerInvariant() switch
        {
            "image/jpeg" =>
                header[0] == 0xFF &&
                header[1] == 0xD8 &&
                header[2] == 0xFF,

            "image/png" =>
                header[0] == 0x89 &&
                header[1] == 0x50 &&
                header[2] == 0x4E &&
                header[3] == 0x47 &&
                header[4] == 0x0D &&
                header[5] == 0x0A &&
                header[6] == 0x1A &&
                header[7] == 0x0A,

            "image/webp" =>
                header[0] == 0x52 &&
                header[1] == 0x49 &&
                header[2] == 0x46 &&
                header[3] == 0x46 &&
                header[8] == 0x57 &&
                header[9] == 0x45 &&
                header[10] == 0x42 &&
                header[11] == 0x50,

            _ => false
        };
    }

    private static string GetSafeExtension(IFormFile image)
    {
        return image.ContentType.ToLowerInvariant() switch
        {
            "image/jpeg" => ".jpg",
            "image/png" => ".png",
            "image/webp" => ".webp",

            _ => throw new ArgumentException(
                "Неподдържан формат на изображението.")
        };
    }

    private static string EncodeStoragePath(string path)
    {
        return string.Join(
            "/",
            path.Split('/')
                .Select(Uri.EscapeDataString));
    }
}