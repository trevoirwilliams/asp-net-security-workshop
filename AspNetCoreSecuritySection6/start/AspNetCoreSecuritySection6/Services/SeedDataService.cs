using Microsoft.AspNetCore.Identity;
using AspNetCoreSecuritySection6.Models;

namespace AspNetCoreSecuritySection6.Services;

/// <summary>
/// Seeds roles and default users on application startup.
/// </summary>
public static class SeedDataService
{
    public static async Task SeedAsync(IServiceProvider services)
    {
        var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
        var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();

        // Seed roles
        string[] roles = ["Admin", "User"];
        foreach (var role in roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
                await roleManager.CreateAsync(new IdentityRole(role));
        }

        // Seed admin user
        await EnsureUser(userManager, "admin@AspNetCoreSecuritySection6.local", "Admin@123!", "Admin User", "Admin");

        // Seed standard user
        await EnsureUser(userManager, "alice@AspNetCoreSecuritySection6.local", "Alice@123!", "Alice Smith", "User");
    }

    private static async Task EnsureUser(
        UserManager<ApplicationUser> userManager,
        string email, string password, string displayName, string role)
    {
        var user = await userManager.FindByEmailAsync(email);
        if (user is null)
        {
            user = new ApplicationUser
            {
                UserName = email,
                Email = email,
                DisplayName = displayName,
                EmailConfirmed = true
            };
            var result = await userManager.CreateAsync(user, password);
            if (!result.Succeeded)
                throw new Exception($"Failed to seed user {email}: {string.Join(", ", result.Errors.Select(e => e.Description))}");
        }

        if (!await userManager.IsInRoleAsync(user, role))
            await userManager.AddToRoleAsync(user, role);
    }
}
