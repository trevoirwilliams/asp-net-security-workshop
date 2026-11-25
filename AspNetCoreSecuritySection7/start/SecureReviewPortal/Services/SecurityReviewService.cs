using Microsoft.EntityFrameworkCore;
using SecureReviewPortal.Data;
using SecureReviewPortal.Models;

namespace SecureReviewPortal.Services;

public class SecurityReviewService
{
    private readonly ApplicationDbContext _db;

    public SecurityReviewService(ApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<List<SecurityReviewRequest>> GetOwnedRequestsAsync(string ownerUserId)
    {
        return await _db.SecurityReviewRequests
            .Include(r => r.Findings)
            .Include(r => r.Attachments)
            .Where(r => r.OwnerUserId == ownerUserId)
            .OrderByDescending(r => r.LastUpdatedAtUtc)
            .ToListAsync();
    }

    public async Task<List<SecurityReviewRequest>> GetSubmittedRequestsAsync()
    {
        return await _db.SecurityReviewRequests
            .Include(r => r.OwnerUser)
            .Where(r => r.Status != SecurityReviewRequestStatus.Draft)
            .OrderByDescending(r => r.LastUpdatedAtUtc)
            .ToListAsync();
    }

    public async Task<SecurityReviewRequest?> GetByIdWithDetailsAsync(int id)
    {
        return await _db.SecurityReviewRequests
            .Include(r => r.OwnerUser)
            .Include(r => r.Attachments)
            .Include(r => r.Findings)
            .Include(r => r.Notes)
            .FirstOrDefaultAsync(r => r.Id == id);
    }

    public async Task<bool> UserOwnsRequestAsync(int requestId, string userId)
    {
        return await _db.SecurityReviewRequests.AnyAsync(r => r.Id == requestId && r.OwnerUserId == userId);
    }

    public async Task SaveChangesAsync()
    {
        await _db.SaveChangesAsync();
    }
}
