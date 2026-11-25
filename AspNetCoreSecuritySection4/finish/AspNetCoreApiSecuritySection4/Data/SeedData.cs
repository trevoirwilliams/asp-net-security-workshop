using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace AspNetCoreApiSecuritySection4.Data;

public static class SeedData
{
    public static async Task InitializeAsync(IServiceProvider serviceProvider)
    {
        var dbContext = serviceProvider.GetRequiredService<ApplicationDbContext>();
        await dbContext.Database.EnsureCreatedAsync();

        var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        var userManager = serviceProvider.GetRequiredService<UserManager<IdentityUser>>();

        string[] roles = ["Administrator", "User"];

        foreach (var role in roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                await roleManager.CreateAsync(new IdentityRole(role));
            }
        }

        await SeedUserAsync(
            userManager,
            email: "admin@demo.com",
            password: "P@ssword1",
            role: "Administrator",
            extraClaim: new Claim("Permission", "CanAccessAdminPanel")
        );

        await SeedUserAsync(
            userManager,
            email: "user@demo.com",
            password: "P@ssword1",
            role: "User",
            extraClaim: null
        );
    }

    private static async Task SeedUserAsync(
        UserManager<IdentityUser> userManager,
        string email,
        string password,
        string role,
        Claim? extraClaim)
    {
        if (await userManager.FindByEmailAsync(email) is not null)
        {
            return;
        }

        var user = new IdentityUser
        {
            UserName = email,
            Email = email,
            EmailConfirmed = true
        };

        var result = await userManager.CreateAsync(user, password);

        if (!result.Succeeded)
        {
            throw new InvalidOperationException($"Failed to seed user '{email}': {string.Join(", ", result.Errors.Select(e => e.Description))}");
        }

        await userManager.AddToRoleAsync(user, role);

        if (extraClaim is not null)
        {
            await userManager.AddClaimAsync(user, extraClaim);
        }
    }
}
