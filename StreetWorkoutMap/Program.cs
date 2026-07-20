using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

using StreetWorkoutMap.Data;
using StreetWorkoutMap.Services;
using StreetWorkoutMap.Services.ImageStorage;
using StreetWorkoutMap.Services.Contrancts;

namespace StreetWorkoutMap
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            //Add DbContext
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseNpgsql(
                    builder.Configuration.GetConnectionString("DefaultConnection")));

            //Add Identity
            builder.Services
                .AddDefaultIdentity<ApplicationUser>(options =>
                {
                    options.SignIn.RequireConfirmedAccount = false;
                    options.Password.RequiredLength = 8;
                    options.Password.RequireNonAlphanumeric = false;
                })
                .AddRoles<IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>();

            // Add services to the container.
            builder.Services.AddControllers().AddJsonOptions( options => options.JsonSerializerOptions.PropertyNamingPolicy = null);
            builder.Services.AddRazorPages();
            builder.Services.AddScoped<IWorkoutSpotService, WorkoutSpotService>();
            builder.Services.AddHttpClient<IImageStorageService, SupabaseImageStorageService>();

            var app = builder.Build();

            await DataSeeder.SeedAsync(app.Services);
            await IdentitySeeder.SeedAsync(app.Services);

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();

            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();
            app.MapStaticAssets();
            app.MapRazorPages()
               .WithStaticAssets();

            app.Run();
        }
    }
}
