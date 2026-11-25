using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AspNetCoreSecuritySection6.Data;
using AspNetCoreSecuritySection6.Models;

namespace AspNetCoreSecuritySection6.Controllers;

/// <summary>
/// ADMIN SURFACE: Allows admins to view all users and all tasks.
/// </summary>
[Authorize(Roles = "Admin")]
public class AdminController(ApplicationDbContext db, UserManager<ApplicationUser> userManager) : Controller
{
    public async Task<IActionResult> Index()
    {
        var users = await userManager.Users.ToListAsync();
        var tasks = await db.Tasks.Include(t => t.Owner).OrderByDescending(t => t.CreatedAtUtc).ToListAsync();

        ViewBag.Users = users;
        return View(tasks);
    }
}
