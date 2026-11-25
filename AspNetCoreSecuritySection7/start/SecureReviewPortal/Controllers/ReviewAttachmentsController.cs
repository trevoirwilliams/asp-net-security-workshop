using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SecureReviewPortal.Data;
using SecureReviewPortal.Services;
using SecureReviewPortal.ViewModels;

namespace SecureReviewPortal.Controllers;

[Authorize(Roles = "Employee,Admin")]
public class ReviewAttachmentsController : Controller
{
    private readonly ApplicationDbContext _db;
    private readonly SecurityReviewService _reviewService;
    private readonly FileUploadService _uploadService;

    public ReviewAttachmentsController(ApplicationDbContext db, SecurityReviewService reviewService, FileUploadService uploadService)
    {
        _db = db;
        _reviewService = reviewService;
        _uploadService = uploadService;
    }

    [HttpGet]
    public async Task<IActionResult> Upload(int requestId)
    {
        var request = await _reviewService.GetByIdWithDetailsAsync(requestId);
        if (request is null)
        {
            return NotFound();
        }

        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrWhiteSpace(userId))
        {
            return Challenge();
        }

        if (!User.IsInRole("Admin") && request.OwnerUserId != userId)
        {
            return Forbid();
        }

        ViewBag.RequestTitle = request.Title;
        return View(new UploadAttachmentViewModel { SecurityReviewRequestId = requestId });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Upload(UploadAttachmentViewModel model)
    {
        var request = await _reviewService.GetByIdWithDetailsAsync(model.SecurityReviewRequestId);
        if (request is null)
        {
            return NotFound();
        }

        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrWhiteSpace(userId))
        {
            return Challenge();
        }

        if (!User.IsInRole("Admin") && request.OwnerUserId != userId)
        {
            return Forbid();
        }

        if (!ModelState.IsValid)
        {
            ViewBag.RequestTitle = request.Title;
            return View(model);
        }

        var (attachment, error) = await _uploadService.SaveAsync(model.File!, model.SecurityReviewRequestId, userId);
        if (!string.IsNullOrWhiteSpace(error) || attachment is null)
        {
            ModelState.AddModelError(nameof(model.File), error ?? "Upload failed.");
            ViewBag.RequestTitle = request.Title;
            return View(model);
        }

        _db.ReviewAttachments.Add(attachment);
        request.LastUpdatedAtUtc = DateTime.UtcNow;
        await _db.SaveChangesAsync();

        return RedirectToAction("Details", "SecurityReviewRequests", new { id = model.SecurityReviewRequestId });
    }
}
