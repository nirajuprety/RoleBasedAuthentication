using Microsoft.AspNetCore.Identity;

namespace RolesBaseIdentification.Seeder
{
    public class UserSeeder
    {
        public static async Task SeedUsersAsync(IServiceProvider serviceProvider)
        {
            var userManager = serviceProvider.GetRequiredService<UserManager<IdentityUser>>();

            // Define users to create
            var users = new[]
            {
            new { Email = "superadmin@example.com", Password = "SuperAdmin@123", Role = "Super Admin" },
            new { Email = "staff@example.com", Password = "Staff@123", Role = "Staff" },
            new { Email = "lowlevelstaff@example.com", Password = "LowLevelStaff@123", Role = "Low-Level Staff" },
            new { Email = "niraj787985465@yopmail.com", Password = "User@123", Role = "User" }
            };

            foreach (var userInfo in users)
            {
                var user = await userManager.FindByEmailAsync(userInfo.Email);
                if (user == null)
                {
                    user = new IdentityUser
                    {
                        UserName = userInfo.Email,
                        Email = userInfo.Email,
                        EmailConfirmed = true
                    };

                    var result = await userManager.CreateAsync(user, userInfo.Password);
                    if (result.Succeeded)
                    {
                        await userManager.AddToRoleAsync(user, userInfo.Role);
                    }
                }
            }
        }
    }

}
