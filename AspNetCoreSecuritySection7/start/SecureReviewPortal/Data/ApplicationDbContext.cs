using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SecureReviewPortal.Models;

namespace SecureReviewPortal.Data;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<SecurityReviewRequest> SecurityReviewRequests => Set<SecurityReviewRequest>();
    public DbSet<ReviewAttachment> ReviewAttachments => Set<ReviewAttachment>();
    public DbSet<ReviewFinding> ReviewFindings => Set<ReviewFinding>();
    public DbSet<SecurityReviewChecklistItem> SecurityReviewChecklistItems => Set<SecurityReviewChecklistItem>();
    public DbSet<ReviewNote> ReviewNotes => Set<ReviewNote>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<SecurityReviewRequest>()
            .HasOne(r => r.OwnerUser)
            .WithMany()
            .HasForeignKey(r => r.OwnerUserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<ReviewAttachment>()
            .HasOne(a => a.SecurityReviewRequest)
            .WithMany(r => r.Attachments)
            .HasForeignKey(a => a.SecurityReviewRequestId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<ReviewFinding>()
            .HasOne(f => f.SecurityReviewRequest)
            .WithMany(r => r.Findings)
            .HasForeignKey(f => f.SecurityReviewRequestId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<ReviewNote>()
            .HasOne(n => n.SecurityReviewRequest)
            .WithMany(r => r.Notes)
            .HasForeignKey(n => n.SecurityReviewRequestId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
