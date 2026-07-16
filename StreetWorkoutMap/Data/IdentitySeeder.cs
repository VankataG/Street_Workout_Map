using Microsoft.AspNetCore.Identity;
using StreetWorkoutMap.Models;
using System.Globalization;
using System.Text.Json;

namespace StreetWorkoutMap.Data
{
    public class IdentitySeeder
    {
        public static async Task SeedAsync(IServiceProvider services)
        {
            using var scope = services.CreateScope();

            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            var configuration = scope.ServiceProvider.GetRequiredService<IConfiguration>();

            string[] roles = ["User", "Admin"];

            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                }
            }

            var adminEmail = configuration["AdminSeed:Email"];
            var adminPass = configuration["AdminSeed:Password"];

            if (string.IsNullOrWhiteSpace(adminEmail) || string.IsNullOrWhiteSpace(adminPass))
            {
                return;
            }

            var admin = await userManager.FindByEmailAsync(adminEmail);

            if (admin is null)
            {
                admin = new ApplicationUser 
                { 
                    UserName = adminEmail, 
                    Email = adminEmail, 
                    EmailConfirmed = true 
                };

                var createResult = await userManager.CreateAsync(admin, adminPass);

                if (!createResult.Succeeded)
                {
                    var errors = string.Join(Environment.NewLine, createResult.Errors.Select(err => err.Description));

                    throw new InvalidOperationException($"Admin creation failed:{Environment.NewLine}{errors}");
                }

            }

            if (!await userManager.IsInRoleAsync(admin, "Admin"))
            {
                await userManager.AddToRoleAsync(admin, "Admin");
            }

        }
    }
}
