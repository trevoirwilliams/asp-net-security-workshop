using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AspNetCoreSecuritySection6.Data;

namespace AspNetCoreSecuritySection6.Controllers;

/// <summary>
/// CSV EXPORT SURFACE: Returns task data as a CSV file download.
/// Accessible to authenticated users; admins see all tasks, users see their own.
/// </summary>
[Authorize]
public class ExportController(ApplicationDbContext db) : Controller
{
    public async Task<IActionResult> Tasks()
    {
        var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        var isAdmin = User.IsInRole("Admin");

        var query = db.Tasks.Include(t => t.Owner).AsQueryable();
        if (!isAdmin)
            query = query.Where(t => t.OwnerId == userId);

        var tasks = await query.OrderBy(t => t.Id).ToListAsync();

        var sb = new StringBuilder();
        sb.AppendLine("Id,Title,Status,IsCompleted,DueDate,CreatedAtUtc,Owner");
        foreach (var t in tasks)
        {
            sb.AppendLine(
                $"{t.Id}," +
                $"\"{t.Title.Replace("\"", "\"\"")}\"," +
                $"{t.Status}," +
                $"{t.IsCompleted}," +
                $"{t.DueDate?.ToString("yyyy-MM-dd") ?? ""}," +
                $"{t.CreatedAtUtc:yyyy-MM-dd HH:mm}," +
                $"\"{t.Owner?.Email ?? ""}\"");
        }

        var bytes = Encoding.UTF8.GetBytes(sb.ToString());
        return File(bytes, "text/csv", "tasks-export.csv");
    }
}
