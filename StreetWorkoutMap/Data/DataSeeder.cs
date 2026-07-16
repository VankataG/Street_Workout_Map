using Microsoft.EntityFrameworkCore;
using StreetWorkoutMap.Models;
using System.Text.Json;

namespace StreetWorkoutMap.Data
{
    public static class DataSeeder
    {
        public static async Task SeedAsync(IServiceProvider services)
        {
            using var scope = services.CreateScope();

            ApplicationDbContext dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            if (await dbContext.WorkoutSpots.AnyAsync())
            {
                return;
            }

            var environment = scope.ServiceProvider.GetRequiredService<IWebHostEnvironment>();

            var filePath = Path.Combine(environment.WebRootPath, "data", "spots.json");

            var json = await File.ReadAllTextAsync(filePath);

            var spots = JsonSerializer.Deserialize<List<WorkoutSpot>>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            if (spots is null)
            {
                return;
            }

            foreach (var spot in spots)
            {
                spot.Id = Guid.NewGuid();
                spot.Status = SpotStatus.Approved;

                foreach (var image in spot.Images)
                {
                    image.Id = Guid.NewGuid();
                }
            }

            await dbContext.WorkoutSpots.AddRangeAsync(spots);
            await dbContext.SaveChangesAsync();

        }
    }
}
