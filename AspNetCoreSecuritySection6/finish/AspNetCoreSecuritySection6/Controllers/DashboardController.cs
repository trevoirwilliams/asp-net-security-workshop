using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AspNetCoreSecuritySection6.Data;
using AspNetCoreSecuritySection6.Models;

namespace AspNetCoreSecuritySection6.Controllers;

[Authorize]
public class DashboardController(ApplicationDbContext db, UserManager<ApplicationUser> userManager) : Controller
{
    public async Task<IActionResult> Index()
    {
        var userId = userManager.GetUserId(User);

        var myTasks = await db.Tasks
            .Where(t => t.OwnerId == userId)
            .OrderByDescending(t => t.CreatedAtUtc)
            .Take(10)
            .ToListAsync();

        var totalTasks = await db.Tasks.CountAsync(t => t.OwnerId == userId);
        var completedTasks = await db.Tasks.CountAsync(t => t.OwnerId == userId && t.IsCompleted);

        ViewBag.TotalTasks = totalTasks;
        ViewBag.CompletedTasks = completedTasks;

        return View(myTasks);
    }
}
