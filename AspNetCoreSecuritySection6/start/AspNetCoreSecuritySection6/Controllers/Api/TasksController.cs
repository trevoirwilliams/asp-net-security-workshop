using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AspNetCoreSecuritySection6.Data;
using AspNetCoreSecuritySection6.Models;

namespace AspNetCoreSecuritySection6.Controllers.Api;

/// <summary>
/// JSON API SURFACE: Minimal API controller for listing tasks.
/// Accessible to authenticated users only.
/// </summary>
[Authorize]
[ApiController]
[Route("api/tasks")]
public class TasksController(ApplicationDbContext db) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetTasks()
    {
        var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        var isAdmin = User.IsInRole("Admin");

        var query = db.Tasks.AsQueryable();
        if (!isAdmin)
            query = query.Where(t => t.OwnerId == userId);

        var tasks = await query
            .Select(t => new
            {
                t.Id,
                t.Title,
                t.Status,
                t.IsCompleted,
                t.DueDate,
                t.CreatedAtUtc
            })
            .ToListAsync();

        return Ok(tasks);
    }
}
