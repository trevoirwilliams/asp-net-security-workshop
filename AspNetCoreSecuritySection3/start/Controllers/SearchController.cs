using AspNetCoreSecuritySection3.Data;
using AspNetCoreSecuritySection3.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AspNetCoreSecuritySection3.Controllers;

public class SearchController : Controller
{
    private readonly AppDbContext _db;

    public SearchController(AppDbContext db) => _db = db;

    [HttpGet]
    public async Task<IActionResult> Index(string? query, string? category)
    {
        // Safe LINQ-based search — no raw SQL
        var results = _db.SupportTickets.AsQueryable();

        if (!string.IsNullOrWhiteSpace(query))
            results = results.Where(t =>
                t.Title.Contains(query) || t.Description.Contains(query));

        if (!string.IsNullOrWhiteSpace(category))
            results = results.Where(t => t.Category == category);

        var vm = new SearchViewModel
        {
            Query = query,
            Category = category,
            Results = await results.OrderByDescending(t => t.CreatedAtUtc).ToListAsync()
        };

        return View(vm);
    }
}
