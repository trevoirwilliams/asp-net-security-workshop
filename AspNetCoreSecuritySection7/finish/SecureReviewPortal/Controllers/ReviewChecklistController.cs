using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SecureReviewPortal.Models;
using SecureReviewPortal.Services;
using SecureReviewPortal.ViewModels;

namespace SecureReviewPortal.Controllers;

[Authorize]
public class ReviewChecklistController : Controller
{
    private readonly ChecklistService _checklistService;

    public ReviewChecklistController(ChecklistService checklistService)
    {
        _checklistService = checklistService;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var items = await _checklistService.GetAllAsync();
        return View(items);
    }

    [HttpGet]
    [Authorize(Roles = "Admin")]
    public IActionResult Create()
    {
        return View(new ReviewChecklistItemViewModel());
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(ReviewChecklistItemViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        await _checklistService.AddAsync(new SecurityReviewChecklistItem
        {
            Category = model.Category,
            ReviewQuestion = model.ReviewQuestion,
            ExpectedProtection = model.ExpectedProtection,
            SortOrder = model.SortOrder
        });

        return RedirectToAction(nameof(Index));
    }
}
