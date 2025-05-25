using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration.UserSecrets;
using RecipeFinderAPI.Common;
using RecipeFinderAPI.Entities;

namespace RecipeFinderAPI.Data
{
    public class SeedData
    {
        public static async Task SeedAsync(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<RecipeFinderDbContext>();

            string adminUsername = "admin";
            string adminEmail = "admin@gmail.com";
            string adminPassword = "admin123";

            // Провери дали съществува вече такъв потребител
            var existingUser = await context.Users
                .FirstOrDefaultAsync(u => u.Username == adminUsername);

            if (existingUser == null)
            {
                var newAdmin = new User
                {
                    Email = adminEmail,
                    Username = adminUsername,
                    Password = adminPassword,
                    Role = Constants.AdminRole.ToString(),
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                };

                context.Users.Add(newAdmin);
                await context.SaveChangesAsync();
            }

            var defaultUser = await context.Users
                .FirstOrDefaultAsync(u => u.UserId == Constants.UndefinedUserId);

            if (defaultUser == null)
            {

                var newDefaultUser = new User
                {
                    UserId = Constants.UndefinedUserId,
                    Email = "undefined@system.local",
                    Username = "Undefined",
                    Password = "N/A", 
                    Role = "system",
                    IsActive = false,
                    CreatedAt = DateTime.UtcNow
                };
                context.Users.Add(newDefaultUser);
                await context.SaveChangesAsync();

            }
        }
    }
}

