using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using AspNetCoreSecuritySection5.Data;
using AspNetCoreSecuritySection5.Options;
using AspNetCoreSecuritySection5.Services;
using Microsoft.AspNetCore.DataProtection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.AddScoped<ISecureNoteStore, SecureNoteFileStore>();
builder.Services.AddScoped<IProtectedLinkService, ProtectedLinkService>();
builder.Services.AddScoped<IWebhookSignatureService, WebhookSignatureService>();
builder.Services.AddScoped<IConfidentialNoteCryptoService, ConfidentialNoteCryptoService>();

builder.Services.AddRazorPages();

var secureDocumentSection = builder.Configuration.GetSection("SecureDocument");
builder.Services.Configure<SecureDocumentOptions>(secureDocumentSection);

var secureDocumentOptions = secureDocumentSection.Get<SecureDocumentOptions>()
?? throw new InvalidOperationException("SecureDocument configuration section is missing.");

var keyRingDirectory = new DirectoryInfo(
    Path.Combine(builder.Environment.ContentRootPath, secureDocumentOptions.KeyRingPath));

builder.Services
.AddDataProtection()
.PersistKeysToFileSystem(keyRingDirectory)
.SetApplicationName(secureDocumentOptions.ApplicationName);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();
app.MapRazorPages()
   .WithStaticAssets();

app.Run();
