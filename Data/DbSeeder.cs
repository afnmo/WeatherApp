namespace WeatherApp.Data;

using Microsoft.AspNetCore.Identity;
using WeatherApp.Models;

public static class DbSeeder
{
    public static async Task SeedUsersAsync(IServiceProvider serviceProvider)
    {
        var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();

        // user 1
        var user1 = await userManager.FindByNameAsync("user1");
        if (user1 == null)
        {
            var newUser1 = new ApplicationUser
            {
                UserName = "user1"
            };

            await userManager.CreateAsync(newUser1, "User@123");
        }

        // user 2
        var user2 = await userManager.FindByNameAsync("user2");
        if (user2 == null)
        {
            var newUser2 = new ApplicationUser
            {
                UserName = "user2"
            };

            await userManager.CreateAsync(newUser2, "User@123");
        }
    }
}