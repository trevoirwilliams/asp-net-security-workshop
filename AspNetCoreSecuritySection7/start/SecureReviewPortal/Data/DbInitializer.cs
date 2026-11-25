using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SecureReviewPortal.Models;

namespace SecureReviewPortal.Data;

public static class DbInitializer
{
    public static async Task InitializeAsync(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var services = scope.ServiceProvider;

        var db = services.GetRequiredService<ApplicationDbContext>();
        var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
        var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();

        await db.Database.MigrateAsync();

        await SeedRolesAsync(roleManager);
        await SeedUsersAsync(userManager);
        await SeedChecklistAsync(db);
        await SeedSampleRequestsAsync(db, userManager);
    }

    private static async Task SeedRolesAsync(RoleManager<IdentityRole> roleManager)
    {
        string[] roles = ["Admin", "Reviewer", "Employee"];

        foreach (var role in roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                await roleManager.CreateAsync(new IdentityRole(role));
            }
        }
    }

    private static async Task SeedUsersAsync(UserManager<ApplicationUser> userManager)
    {
        await EnsureUserAsync(
            userManager,
            email: "admin@securereviewportal.local",
            fullName: "Casey Admin",
            department: "Security Engineering",
            role: "Admin",
            password: "DevOnly!123");

        await EnsureUserAsync(
            userManager,
            email: "reviewer@securereviewportal.local",
            fullName: "Robin Reviewer",
            department: "Internal Audit",
            role: "Reviewer",
            password: "DevOnly!123");

        await EnsureUserAsync(
            userManager,
            email: "employee@securereviewportal.local",
            fullName: "Elliot Employee",
            department: "Product Engineering",
            role: "Employee",
            password: "DevOnly!123");
    }

    private static async Task EnsureUserAsync(
        UserManager<ApplicationUser> userManager,
        string email,
        string fullName,
        string department,
        string role,
        string password)
    {
        var user = await userManager.FindByEmailAsync(email);
        if (user is null)
        {
            user = new ApplicationUser
            {
                UserName = email,
                Email = email,
                EmailConfirmed = true,
                FullName = fullName,
                Department = department,
                CreatedAtUtc = DateTime.UtcNow
            };

            var result = await userManager.CreateAsync(user, password);
            if (!result.Succeeded)
            {
                var errors = string.Join("; ", result.Errors.Select(e => e.Description));
                throw new InvalidOperationException($"Failed to create seeded user '{email}'. {errors}");
            }
        }

        if (!await userManager.IsInRoleAsync(user, role))
        {
            await userManager.AddToRoleAsync(user, role);
        }
    }

    private static async Task SeedChecklistAsync(ApplicationDbContext db)
    {
        if (await db.SecurityReviewChecklistItems.AnyAsync())
        {
            return;
        }

        var items = new List<SecurityReviewChecklistItem>
        {
            new() { Category = "Request and input handling", ReviewQuestion = "Are all form inputs validated server-side?", ExpectedProtection = "Data annotations and ModelState checks applied on all post actions.", SortOrder = 1 },
            new() { Category = "Authentication and authorization", ReviewQuestion = "Are ownership and role checks enforced server-side?", ExpectedProtection = "Employees are restricted to own records; reviewer and admin actions are role-protected.", SortOrder = 2 },
            new() { Category = "Sensitive data protection", ReviewQuestion = "Are uploaded files stored and handled safely?", ExpectedProtection = "Uploads are outside web root and not directly executable/served.", SortOrder = 3 },
            new() { Category = "Configuration and environment safety", ReviewQuestion = "Are secrets managed outside source control?", ExpectedProtection = "Development placeholders only; production secrets use secure stores.", SortOrder = 4 },
            new() { Category = "Error handling and logging", ReviewQuestion = "Do logs and errors avoid sensitive data exposure?", ExpectedProtection = "Logs are structured and minimal; production errors are generic.", SortOrder = 5 },
            new() { Category = "Release readiness", ReviewQuestion = "Are findings triaged and verified before release?", ExpectedProtection = "Critical/high findings addressed or accepted with documented risk.", SortOrder = 6 }
        };

        db.SecurityReviewChecklistItems.AddRange(items);
        await db.SaveChangesAsync();
    }

    private static async Task SeedSampleRequestsAsync(ApplicationDbContext db, UserManager<ApplicationUser> userManager)
    {
        if (await db.SecurityReviewRequests.AnyAsync())
        {
            return;
        }

        var employee = await userManager.FindByEmailAsync("employee@securereviewportal.local");
        if (employee is null)
        {
            throw new InvalidOperationException("Seed employee user was not found.");
        }

        var draftRequest = new SecurityReviewRequest
        {
            Title = "Internal Analytics Dashboard",
            Description = "New internal dashboard exposing operations metrics for product teams.",
            BusinessJustification = "Needed for quarterly planning and KPI visibility.",
            SystemName = "OpsInsights",
            DataClassification = DataClassification.Internal,
            Status = SecurityReviewRequestStatus.Draft,
            OwnerUserId = employee.Id,
            CreatedAtUtc = DateTime.UtcNow.AddDays(-4),
            LastUpdatedAtUtc = DateTime.UtcNow.AddDays(-2)
        };

        var submittedRequest = new SecurityReviewRequest
        {
            Title = "Partner Integration API",
            Description = "Inbound integration endpoint for trusted third-party partner onboarding.",
            BusinessJustification = "Reduces manual onboarding delays and support workload.",
            SystemName = "PartnerBridge",
            DataClassification = DataClassification.Confidential,
            Status = SecurityReviewRequestStatus.Submitted,
            OwnerUserId = employee.Id,
            CreatedAtUtc = DateTime.UtcNow.AddDays(-8),
            SubmittedAtUtc = DateTime.UtcNow.AddDays(-6),
            LastUpdatedAtUtc = DateTime.UtcNow.AddDays(-6)
        };

        db.SecurityReviewRequests.AddRange(draftRequest, submittedRequest);
        await db.SaveChangesAsync();

        db.ReviewFindings.Add(new ReviewFinding
        {
            SecurityReviewRequestId = submittedRequest.Id,
            Title = "Insufficient API input validation",
            Description = "Request payload schema allows unbounded free-form fields.",
            Severity = FindingSeverity.Medium,
            Source = FindingSource.ManualReview,
            Status = FindingStatus.Open,
            Evidence = "Controller accepts model without max-length annotations.",
            RecommendedAction = "Define stricter model constraints and reject invalid payloads.",
            CreatedAtUtc = DateTime.UtcNow.AddDays(-5)
        });

        await db.SaveChangesAsync();
    }
}
