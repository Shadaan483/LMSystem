using Microsoft.AspNetCore.Identity;

namespace LMSystem.Data
{
    // Runs once at application startup. Creates the three roles and the three
    // demo users (matching the old hardcoded LoginController accounts) if they
    // don't already exist. Safe to run on every startup - CreateAsync/RoleExistsAsync
    // checks make it idempotent, so it will not duplicate data on restart.
    public static class SeedData
    {
        private static readonly string[] Roles = { "Administrator", "Librarian", "Member" };

        public static async Task InitializeAsync(IServiceProvider services)
        {
            var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = services.GetRequiredService<UserManager<IdentityUser>>();

            foreach (var role in Roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                }
            }

            // Same three accounts/passwords that used to live in the hardcoded
            // LoginController list, now backed by real Identity + roles.
            await EnsureUserAsync(userManager, "admin", "12345", "Administrator");
            await EnsureUserAsync(userManager, "mycodingproject", "myc546", "Librarian");
            await EnsureUserAsync(userManager, "my", "myc", "Member");
        }

        private static async Task EnsureUserAsync(UserManager<IdentityUser> userManager, string userName, string password, string role)
        {
            var existingUser = await userManager.FindByNameAsync(userName);
            if (existingUser != null)
            {
                return;
            }

            var user = new IdentityUser
            {
                UserName = userName,
                Email = $"{userName}@lmsystem.local",
                EmailConfirmed = true
            };

            var result = await userManager.CreateAsync(user, password);
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(user, role);
            }
        }
    }
}
