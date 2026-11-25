using AspNetCoreSecuritySection3.Data;
using AspNetCoreSecuritySection3.Models;
using AspNetCoreSecuritySection3.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AspNetCoreSecuritySection3.Controllers;

public class SurveyController : Controller
{
    private readonly AppDbContext _db;

    public SurveyController(AppDbContext db) => _db = db;

    [HttpGet]
    public IActionResult Create() => View(new SurveyInputModel());

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(SurveyInputModel model)
    {
        var recentCutoff = DateTime.UtcNow.AddMinutes(-5);

        var duplicateExists = await _db.SurveySubmissions.AnyAsync(s =>
            s.Email == model.Email &&
            s.SubmittedAtUtc >= recentCutoff);
        
        if (duplicateExists)
        {
            ModelState.AddModelError(string.Empty,
                "You must wait at least 5 minutes before submitting another survey with the same email address.");
        }

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var survey = new SurveySubmission
        {
            Name = model.Name,
            Email = model.Email,
            Score = model.Score,
            Comment = model.Comment,
            SubmittedAtUtc = DateTime.UtcNow
        };

        _db.SurveySubmissions.Add(survey);
        await _db.SaveChangesAsync();

        TempData["SuccessMessage"] = "Survey submitted successfully.";
        return RedirectToAction(nameof(Confirmation));
    }

    public IActionResult Confirmation() => View();
}
