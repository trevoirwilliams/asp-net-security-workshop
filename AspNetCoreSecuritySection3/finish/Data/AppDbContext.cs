using AspNetCoreSecuritySection3.Models;
using Microsoft.EntityFrameworkCore;

namespace AspNetCoreSecuritySection3.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<SurveySubmission> SurveySubmissions => Set<SurveySubmission>();
    public DbSet<SupportTicket> SupportTickets => Set<SupportTicket>();
    public DbSet<ProductReview> ProductReviews => Set<ProductReview>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<SurveySubmission>().HasData(
            new SurveySubmission
            {
                Id = 1,
                Name = "Alice Johnson",
                Email = "alice@example.com",
                Score = 5,
                Comment = "Great service overall!",
                SubmittedAtUtc = new DateTime(2026, 1, 10, 9, 0, 0, DateTimeKind.Utc)
            },
            new SurveySubmission
            {
                Id = 2,
                Name = "Bob Smith",
                Email = "bob@example.com",
                Score = 3,
                Comment = "Average experience.",
                SubmittedAtUtc = new DateTime(2026, 1, 15, 14, 30, 0, DateTimeKind.Utc)
            }
        );

        modelBuilder.Entity<SupportTicket>().HasData(
            new SupportTicket
            {
                Id = 1,
                FullName = "Carol White",
                Email = "carol@example.com",
                Category = "Billing",
                Title = "Invoice discrepancy",
                Description = "My last invoice has an incorrect charge.",
                AttachmentPath = null,
                CreatedAtUtc = new DateTime(2026, 1, 20, 10, 0, 0, DateTimeKind.Utc)
            },
            new SupportTicket
            {
                Id = 2,
                FullName = "David Brown",
                Email = "david@example.com",
                Category = "Technical",
                Title = "Login page error",
                Description = "Cannot log in after the recent update.",
                AttachmentPath = null,
                CreatedAtUtc = new DateTime(2026, 2, 1, 8, 0, 0, DateTimeKind.Utc)
            }
        );

        modelBuilder.Entity<ProductReview>().HasData(
            new ProductReview
            {
                Id = 1,
                ProductName = "Wireless Keyboard",
                ReviewerName = "Eve Turner",
                Rating = 4,
                ReviewText = "Solid build quality and comfortable to type on.",
                CreatedAtUtc = new DateTime(2026, 2, 10, 11, 0, 0, DateTimeKind.Utc)
            },
            new ProductReview
            {
                Id = 2,
                ProductName = "USB-C Hub",
                ReviewerName = "Frank Miller",
                Rating = 2,
                ReviewText = "Stopped working after a week.",
                CreatedAtUtc = new DateTime(2026, 2, 20, 16, 0, 0, DateTimeKind.Utc)
            }
        );
    }
}
