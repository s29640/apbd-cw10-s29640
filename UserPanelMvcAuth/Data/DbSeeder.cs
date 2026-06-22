using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using UserPanelMvcAuth.Models;

namespace UserPanelMvcAuth.Data
{
    public static class DbSeeder
    {
        private const string ADMIN_EMAIL = "admin@example.com";
        private const string ADMIN_PASSWORD = "admin";

        public static async Task SeedAsync(AppDbContext context)
        {
            await context.Database.MigrateAsync();

            if (await context.AppUsers.AnyAsync(u => u.Role == "Admin"))
                return;

            var admin = new AppUser
            {
                Email = ADMIN_EMAIL,
                PasswordHash = string.Empty,
                Role = "Admin",
                CreatedAt = DateTime.UtcNow
            };

            var passwordHasher = new PasswordHasher<AppUser>();

            admin.PasswordHash = passwordHasher.HashPassword(
                admin,
                ADMIN_PASSWORD
            );

            context.AppUsers.Add(admin);
            await context.SaveChangesAsync();
        }
    }
}
