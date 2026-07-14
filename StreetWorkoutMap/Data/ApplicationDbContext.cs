using Microsoft.EntityFrameworkCore;
using StreetWorkoutMap.Models;

namespace StreetWorkoutMap.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<WorkoutSpot> WorkoutSpots { get; set; }

        public DbSet<SpotImage> SpotImages { get; set; }
    }
}
