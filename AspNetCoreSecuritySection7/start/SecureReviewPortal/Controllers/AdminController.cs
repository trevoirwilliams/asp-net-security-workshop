using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SecureReviewPortal.Data;

namespace SecureReviewPortal.Controllers;

[Authorize(Roles = "Admin")]
public class AdminController : Controller
{
    private readonly ApplicationDbContext _db;

    public AdminController(ApplicationDbContext db)
    {
        _db = db;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var requests = await _db.SecurityReviewRequests
            .Include(r => r.OwnerUser)
            .OrderByDescending(r => r.LastUpdatedAtUtc)
            .ToListAsync();

        return View(requests);
    }
}
