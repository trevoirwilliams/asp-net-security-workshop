using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SecureReviewPortal.Data;
using SecureReviewPortal.Models;
using SecureReviewPortal.Services;
using SecureReviewPortal.ViewModels;

namespace SecureReviewPortal.Controllers;

[Authorize]
public class SecurityReviewRequestsController : Controller
{
    private readonly ApplicationDbContext _db;
    private readonly SecurityReviewService _reviewService;
    private readonly ILogger<SecurityReviewRequestsController> _logger;

    public SecurityReviewRequestsController(
        ApplicationDbContext db,
        SecurityReviewService reviewService,
        ILogger<SecurityReviewRequestsController> logger)
    {
        _db = db;
        _reviewService = reviewService;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrWhiteSpace(userId))
        {
            return Challenge();
        }

        if (User.IsInRole("Admin") || User.IsInRole("Reviewer"))
        {
            var submitted = await _reviewService.GetSubmittedRequestsAsync();
            return View("Submitted", submitted);
        }

        var owned = await _reviewService.GetOwnedRequestsAsync(userId);
        return View(owned);
    }

    [HttpGet]
    [Authorize(Roles = "Employee,Admin")]
    public IActionResult Create()
    {
        return View(new CreateReviewRequestViewModel());
    }

    [HttpPost]
    [Authorize(Roles = "Employee,Admin")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateReviewRequestViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrWhiteSpace(userId))
        {
            return Challenge();
        }

        var request = new SecurityReviewRequest
        {
            Title = model.Title,
            Description = model.Description,
            BusinessJustification = model.BusinessJustification,
            SystemName = model.SystemName,
            DataClassification = model.DataClassification,
            Status = SecurityReviewRequestStatus.Draft,
            OwnerUserId = userId,
            CreatedAtUtc = DateTime.UtcNow,
            LastUpdatedAtUtc = DateTime.UtcNow
        };

        _db.SecurityReviewRequests.Add(request);
        await _db.SaveChangesAsync();

        return RedirectToAction(nameof(Details), new { id = request.Id });
    }

    [HttpGet]
    public async Task<IActionResult> Details(int id)
    {
        var request = await _reviewService.GetByIdWithDetailsAsync(id);
        if (request is null)
        {
            return NotFound();
        }

        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrWhiteSpace(userId))
        {
            return Challenge();
        }

        if (!User.IsInRole("Admin") && !User.IsInRole("Reviewer") && request.OwnerUserId != userId)
        {
            return Forbid();
        }

        return View(request);
    }

    [HttpGet]
    [Authorize(Roles = "Employee,Admin")]
    public async Task<IActionResult> Edit(int id)
    {
        var request = await _db.SecurityReviewRequests.FindAsync(id);
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

        if (request.Status != SecurityReviewRequestStatus.Draft && !User.IsInRole("Admin"))
        {
            return BadRequest("Only draft requests can be edited.");
        }

        var model = new EditReviewRequestViewModel
        {
            Id = request.Id,
            Title = request.Title,
            Description = request.Description,
            BusinessJustification = request.BusinessJustification,
            SystemName = request.SystemName,
            DataClassification = request.DataClassification,
            Status = request.Status
        };

        return View(model);
    }

    [HttpPost]
    [Authorize(Roles = "Employee,Admin")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(EditReviewRequestViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var request = await _db.SecurityReviewRequests.FindAsync(model.Id);
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

        if (request.Status != SecurityReviewRequestStatus.Draft && !User.IsInRole("Admin"))
        {
            return BadRequest("Only draft requests can be edited.");
        }

        request.Title = model.Title;
        request.Description = model.Description;
        request.BusinessJustification = model.BusinessJustification;
        request.SystemName = model.SystemName;
        request.DataClassification = model.DataClassification;
        request.LastUpdatedAtUtc = DateTime.UtcNow;

        await _db.SaveChangesAsync();
        return RedirectToAction(nameof(Details), new { id = request.Id });
    }

    [HttpPost]
    [Authorize(Roles = "Employee,Admin")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Submit(int id)
    {
        var request = await _db.SecurityReviewRequests.FindAsync(id);
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

        if (request.Status != SecurityReviewRequestStatus.Draft)
        {
            return BadRequest("Only draft requests can be submitted.");
        }

        request.Status = SecurityReviewRequestStatus.Submitted;
        request.SubmittedAtUtc = DateTime.UtcNow;
        request.LastUpdatedAtUtc = DateTime.UtcNow;

        // TODO: Section 7 review item - confirm this log message does not capture excessive business detail.
        _logger.LogInformation(
            "Security review request submitted. RequestId={RequestId}, System={SystemName}, Classification={DataClassification}, Description={Description}, BusinessJustification={BusinessJustification}",
            request.Id,
            request.SystemName,
            request.DataClassification,
            request.Description,
            request.BusinessJustification);

        await _db.SaveChangesAsync();

        return RedirectToAction(nameof(Details), new { id });
    }
}
