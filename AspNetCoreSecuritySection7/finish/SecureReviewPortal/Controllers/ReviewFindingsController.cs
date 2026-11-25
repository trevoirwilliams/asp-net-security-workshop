using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SecureReviewPortal.Data;
using SecureReviewPortal.Models;
using SecureReviewPortal.ViewModels;

namespace SecureReviewPortal.Controllers;

[Authorize]
public class ReviewFindingsController : Controller
{
    private readonly ApplicationDbContext _db;

    public ReviewFindingsController(ApplicationDbContext db)
    {
        _db = db;
    }

    [HttpGet]
    [Authorize(Roles = "Reviewer,Admin")]
    public async Task<IActionResult> Queue()
    {
        var requests = await _db.SecurityReviewRequests
            .Include(r => r.OwnerUser)
            .Where(r => r.Status != SecurityReviewRequestStatus.Draft)
            .OrderByDescending(r => r.LastUpdatedAtUtc)
            .ToListAsync();

        return View(requests);
    }

    [HttpGet]
    [Authorize(Roles = "Reviewer,Admin")]
    public async Task<IActionResult> ReviewReady(int id)
    {
        var request = await _db.SecurityReviewRequests
            .Include(r => r.OwnerUser)
            .Include(r => r.Findings)
            .FirstOrDefaultAsync(r => r.Id == id);

        if (request is null)
        {
            return NotFound();
        }

        if (request.Status == SecurityReviewRequestStatus.Draft)
        {
            return BadRequest("Draft requests are not in review scope.");
        }

        return View(request);
    }

    [HttpGet]
    [Authorize(Roles = "Reviewer,Admin")]
    public async Task<IActionResult> Create(int requestId)
    {
        var request = await _db.SecurityReviewRequests.FirstOrDefaultAsync(r => r.Id == requestId);
        if (request is null)
        {
            return NotFound();
        }

        if (request.Status == SecurityReviewRequestStatus.Draft)
        {
            return BadRequest("Cannot add findings to draft requests.");
        }

        ViewBag.RequestTitle = request.Title;
        return View(new ReviewFindingViewModel { SecurityReviewRequestId = requestId });
    }

    [HttpPost]
    [Authorize(Roles = "Reviewer,Admin")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(ReviewFindingViewModel model)
    {
        if (!ModelState.IsValid)
        {
            var requestTitle = await _db.SecurityReviewRequests
                .Where(r => r.Id == model.SecurityReviewRequestId)
                .Select(r => r.Title)
                .FirstOrDefaultAsync();

            ViewBag.RequestTitle = requestTitle ?? "Unknown request";
            return View(model);
        }

        var request = await _db.SecurityReviewRequests.FirstOrDefaultAsync(r => r.Id == model.SecurityReviewRequestId);
        if (request is null)
        {
            return NotFound();
        }

        if (request.Status == SecurityReviewRequestStatus.Draft)
        {
            return BadRequest("Cannot add findings to draft requests.");
        }

        var finding = new ReviewFinding
        {
            SecurityReviewRequestId = model.SecurityReviewRequestId,
            Title = model.Title,
            Description = model.Description,
            Severity = model.Severity,
            Source = model.Source,
            Status = model.Status,
            Evidence = model.Evidence,
            RecommendedAction = model.RecommendedAction,
            CreatedAtUtc = DateTime.UtcNow
        };

        _db.ReviewFindings.Add(finding);
        request.LastUpdatedAtUtc = DateTime.UtcNow;

        if (request.Status == SecurityReviewRequestStatus.Submitted)
        {
            request.Status = SecurityReviewRequestStatus.InReview;
        }

        await _db.SaveChangesAsync();

        return RedirectToAction(nameof(ReviewReady), new { id = model.SecurityReviewRequestId });
    }
}
