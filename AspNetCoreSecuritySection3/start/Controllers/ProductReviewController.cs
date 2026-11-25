using AspNetCoreSecuritySection3.Data;
using AspNetCoreSecuritySection3.Models;
using AspNetCoreSecuritySection3.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace AspNetCoreSecuritySection3.Controllers;

public class ProductReviewController : Controller
{
    private readonly AppDbContext _db;

    public ProductReviewController(AppDbContext db) => _db = db;

    [HttpGet]
    public IActionResult Create() => View(new ProductReviewInputModel());

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(ProductReviewInputModel model)
    {
        // COURSE NOTE: strengthen ModelState handling in Video 3.3

        // COURSE NOTE: add business-rule validation in Video 3.6
        // e.g. low-score-requires-comment and duplicate review checks

        var review = new ProductReview
        {
            ProductName = model.ProductName,
            ReviewerName = model.ReviewerName,
            Rating = model.Rating,
            ReviewText = model.ReviewText,
            CreatedAtUtc = DateTime.UtcNow
        };

        _db.ProductReviews.Add(review);
        await _db.SaveChangesAsync();

        TempData["SuccessMessage"] = "Review submitted successfully.";
        return RedirectToAction(nameof(Confirmation));
    }

    public IActionResult Confirmation() => View();
}
