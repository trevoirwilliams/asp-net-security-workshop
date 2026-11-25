using Microsoft.EntityFrameworkCore;
using SecureReviewPortal.Data;
using SecureReviewPortal.Models;

namespace SecureReviewPortal.Services;

public class ChecklistService
{
    private readonly ApplicationDbContext _db;

    public ChecklistService(ApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<List<SecurityReviewChecklistItem>> GetAllAsync()
    {
        return await _db.SecurityReviewChecklistItems
            .OrderBy(c => c.SortOrder)
            .ThenBy(c => c.Id)
            .ToListAsync();
    }

    public async Task AddAsync(SecurityReviewChecklistItem item)
    {
        _db.SecurityReviewChecklistItems.Add(item);
        await _db.SaveChangesAsync();
    }
}
