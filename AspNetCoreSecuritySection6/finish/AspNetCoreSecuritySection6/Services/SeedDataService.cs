using Microsoft.AspNetCore.Identity;
using AspNetCoreSecuritySection6.Models;

namespace AspNetCoreSecuritySection6.Services;

/// <summary>
/// Seeds roles and default users on application startup.
///
/// SR-14 — Hardcoded Credential Guard
/// Demo accounts (admin / alice) are seeded ONLY when the host environment
/// is Development.  In Staging or Production the method returns immediately
/// after creating roles, so hardcoded passwords never reach a shared system.
/// </summary>
public static class SeedDataService
{
    // SR-14: Accept IHostEnvironment so the caller (Program.cs) can pass
    //        the runtime environment and gate demo-credential seeding.
    public static async Task SeedAsync(IServiceProvider services, IHostEnvironment environment)
    {
        var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
        var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();

        // Roles are safe to create in every environment
        string[] roles = ["Admin", "User"];
        foreach (var role in roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
                await roleManager.CreateAsync(new IdentityRole(role));
        }

        // SR-14: Hardcoded demo credentials must NEVER be seeded outside Development
        if (!environment.IsDevelopment())
            return;

        await EnsureUser(userManager, "admin@AspNetCoreSecuritySection6.local", "Admin@123!", "Admin User", "Admin");
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
                UserName       = email,
                Email          = email,
                DisplayName    = displayName,
                EmailConfirmed = true
            };
            var result = await userManager.CreateAsync(user, password);
            if (!result.Succeeded)
                throw new Exception(
                    $"Failed to seed user {email}: {string.Join(", ", result.Errors.Select(e => e.Description))}");
        }

        if (!await userManager.IsInRoleAsync(user, role))
            await userManager.AddToRoleAsync(user, role);
    }
}
