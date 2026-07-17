using System.Net.Http.Headers;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;

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

    public async Task<List<string>> UploadImagesAsync(
        Guid workoutSpotId,
        IEnumerable<IFormFile> images)
    {
        var uploadedPaths = new List<string>();

        try
        {
            foreach (var image in images)
            {
                ValidateImage(image);

                var extension = GetSafeExtension(image);
                var fileName = $"{Guid.NewGuid()}{extension}";

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

    private static void ValidateImage(IFormFile image)
    {
        const long maxFileSize = 5 * 1024 * 1024;

        string[] allowedContentTypes =
        [
            "image/jpeg",
            "image/png",
            "image/webp"
        ];

        if (image.Length == 0)
        {
            throw new ArgumentException(
                "Празен файл не може да бъде качен.");
        }

        if (image.Length > maxFileSize)
        {
            throw new ArgumentException(
                $"Снимката '{image.FileName}' е по-голяма от 5 MB.");
        }

        if (!allowedContentTypes.Contains(
                image.ContentType,
                StringComparer.OrdinalIgnoreCase))
        {
            throw new ArgumentException(
                $"Форматът на '{image.FileName}' не е позволен.");
        }
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