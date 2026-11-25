using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SecureReviewPortal.Authorization;
using SecureReviewPortal.Data;
using SecureReviewPortal.Extensions;
using SecureReviewPortal.Models;
using SecureReviewPortal.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services
    .AddIdentity<ApplicationUser, IdentityRole>(options =>
    {
        options.Password.RequireDigit = true;
        options.Password.RequireLowercase = true;
        options.Password.RequireUppercase = true;
        options.Password.RequireNonAlphanumeric = true;
        options.Password.RequiredLength = 8;
        options.SignIn.RequireConfirmedAccount = false;
    })
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Account/Login";
    options.AccessDeniedPath = "/Account/AccessDenied";
});

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));
    options.AddPolicy("ReviewerOrAdmin", policy => policy.RequireRole("Reviewer", "Admin"));
    options.AddPolicy("RequestOwner", policy => policy.Requirements.Add(new RequestOwnerRequirement()));
});

builder.Services.AddScoped<SecurityReviewService>();
builder.Services.AddScoped<FileUploadService>();
builder.Services.AddScoped<ChecklistService>();
builder.Services.AddScoped<IAuthorizationHandler, RequestOwnerHandler>();

builder.Services.AddControllersWithViews();

var app = builder.Build();

// TODO: Section 7 review item - verify production error handling before release.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}
else
{
    app.UseDeveloperExceptionPage();
}

app.UseHttpsRedirection();
app.UseBasicSecurityHeaders();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

await DbInitializer.InitializeAsync(app.Services);

app.Run();
