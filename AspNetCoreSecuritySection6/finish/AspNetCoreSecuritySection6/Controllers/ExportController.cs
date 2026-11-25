using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AspNetCoreSecuritySection6.Data;

namespace AspNetCoreSecuritySection6.Controllers;

/// <summary>
/// CSV EXPORT SURFACE: Returns task data as a CSV file download.
/// Accessible to authenticated users; admins see all tasks, users see their own.
///
/// SR-12 — CSV Formula Injection Prevention
/// Any cell that begins with a spreadsheet formula trigger character
/// (= + - @ TAB CR) is prefixed with a single-quote so spreadsheet
/// applications treat the value as literal text rather than executing it.
/// </summary>
[Authorize]
public class ExportController(ApplicationDbContext db) : Controller
{
    public async Task<IActionResult> Tasks()
    {
        var userId  = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        var isAdmin = User.IsInRole("Admin");

        var query = db.Tasks.Include(t => t.Owner).AsQueryable();
        if (!isAdmin)
            query = query.Where(t => t.OwnerId == userId);

        var tasks = await query.OrderBy(t => t.Id).ToListAsync();

        var sb = new StringBuilder();
        sb.AppendLine("Id,Title,Status,IsCompleted,DueDate,CreatedAtUtc,Owner");
        foreach (var t in tasks)
        {
            // SR-12: Sanitize every user-supplied string cell before writing to CSV
            sb.AppendLine(
                $"{t.Id}," +
                $"\"{SanitizeCsvCell(t.Title)}\"," +
                $"{t.Status}," +
                $"{t.IsCompleted}," +
                $"{t.DueDate?.ToString("yyyy-MM-dd") ?? ""}," +
                $"{t.CreatedAtUtc:yyyy-MM-dd HH:mm}," +
                $"\"{SanitizeCsvCell(t.Owner?.Email ?? "")}\"");
        }

        var bytes = Encoding.UTF8.GetBytes(sb.ToString());
        return File(bytes, "text/csv", "tasks-export.csv");
    }

    /// <summary>
    /// SR-12 — Neutralises formula-injection triggers per OWASP CSV Injection guidance.
    /// Prefixes cells that open with = + - @ \t \r with a single-quote.
    /// Also escapes embedded double-quotes per RFC 4180.
    /// </summary>
    private static string SanitizeCsvCell(string value)
    {
        if (string.IsNullOrEmpty(value)) return value;

        // RFC 4180: escape embedded double-quotes by doubling them
        value = value.Replace("\"", "\"\"");

        // Formula-injection: prefix any dangerous leading character
        if (value[0] is '=' or '+' or '-' or '@' or '\t' or '\r')
            return "'" + value;

        return value;
    }
}
