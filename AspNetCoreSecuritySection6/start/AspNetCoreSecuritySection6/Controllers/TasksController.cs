using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AspNetCoreSecuritySection6.Data;
using AspNetCoreSecuritySection6.Models;
using AspNetCoreSecuritySection6.Services;

namespace AspNetCoreSecuritySection6.Controllers;

/// <summary>
/// TASK MANAGEMENT SURFACE: CRUD operations for tasks, comments, and attachments.
/// </summary>
[Authorize]
public class TasksController(
    ApplicationDbContext db,
    UserManager<ApplicationUser> userManager,
    IWebHostEnvironment env,
    WebhookService webhook) : Controller
{
    // ---- LIST ----

    public async Task<IActionResult> Index()
    {
        var userId = userManager.GetUserId(User);
        var isAdmin = User.IsInRole("Admin");

        var query = db.Tasks.Include(t => t.Owner).AsQueryable();
        if (!isAdmin)
            query = query.Where(t => t.OwnerId == userId);

        var tasks = await query.OrderByDescending(t => t.CreatedAtUtc).ToListAsync();
        return View(tasks);
    }

    // ---- DETAILS ----

    public async Task<IActionResult> Details(int id)
    {
        var task = await db.Tasks
            .Include(t => t.Owner)
            .Include(t => t.Comments).ThenInclude(c => c.Author)
            .Include(t => t.Attachments).ThenInclude(a => a.UploadedBy)
            .FirstOrDefaultAsync(t => t.Id == id);

        if (task is null) return NotFound();
        if (!CanAccess(task)) return Forbid();

        return View(task);
    }

    // ---- CREATE ----

    public IActionResult Create() => View();

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(TaskItem model)
    {
        ModelState.Remove("Owner");
        ModelState.Remove("OwnerId");

        if (!ModelState.IsValid) return View(model);

        model.OwnerId = userManager.GetUserId(User);
        model.CreatedAtUtc = DateTime.UtcNow;
        db.Tasks.Add(model);
        await db.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    // ---- EDIT ----

    public async Task<IActionResult> Edit(int id)
    {
        var task = await db.Tasks.FindAsync(id);
        if (task is null) return NotFound();
        if (!CanAccess(task)) return Forbid();
        return View(task);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, TaskItem model)
    {
        var task = await db.Tasks.FindAsync(id);
        if (task is null) return NotFound();
        if (!CanAccess(task)) return Forbid();

        ModelState.Remove("Owner");
        ModelState.Remove("OwnerId");

        if (!ModelState.IsValid) return View(model);

        task.Title = model.Title;
        task.Description = model.Description;
        task.Status = model.Status;
        task.DueDate = model.DueDate;
        await db.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    // ---- MARK COMPLETE ----

    /// <summary>
    /// WEBHOOK TRIGGER POINT: Fires outbound HTTP notification on task completion.
    /// </summary>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Complete(int id)
    {
        var task = await db.Tasks.FindAsync(id);
        if (task is null) return NotFound();
        if (!CanAccess(task)) return Forbid();

        task.IsCompleted = true;
        task.CompletedAtUtc = DateTime.UtcNow;
        task.Status = Models.TaskStatus.Done;
        await db.SaveChangesAsync();

        // EXTERNAL INTEGRATION: send webhook notification
        await webhook.NotifyTaskCompletedAsync(task.Id, task.Title);

        return RedirectToAction(nameof(Details), new { id });
    }

    // ---- ADD COMMENT ----

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AddComment(int taskId, string body)
    {
        var task = await db.Tasks.FindAsync(taskId);
        if (task is null) return NotFound();
        if (!CanAccess(task)) return Forbid();

        var comment = new TaskComment
        {
            TaskItemId = taskId,
            AuthorId = userManager.GetUserId(User),
            Body = body,
            CreatedAtUtc = DateTime.UtcNow
        };
        db.TaskComments.Add(comment);
        await db.SaveChangesAsync();
        return RedirectToAction(nameof(Details), new { id = taskId });
    }

    // ---- UPLOAD ATTACHMENT ----

    /// <summary>
    /// UPLOAD SURFACE: Accepts file upload and stores it under wwwroot/uploads.
    /// Saves metadata to the database.
    /// </summary>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UploadAttachment(int taskId, IFormFile file)
    {
        var task = await db.Tasks.FindAsync(taskId);
        if (task is null) return NotFound();
        if (!CanAccess(task)) return Forbid();

        if (file is null || file.Length == 0)
        {
            TempData["Error"] = "Please select a file.";
            return RedirectToAction(nameof(Details), new { id = taskId });
        }

        // Store file on disk using a generated name to avoid collisions
        var uploadsDir = Path.Combine(env.WebRootPath, "uploads");
        Directory.CreateDirectory(uploadsDir);

        var storedFileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
        var filePath = Path.Combine(uploadsDir, storedFileName);

        using (var stream = System.IO.File.Create(filePath))
        {
            await file.CopyToAsync(stream);
        }

        var attachment = new TaskAttachment
        {
            TaskItemId = taskId,
            UploadedById = userManager.GetUserId(User),
            OriginalFileName = file.FileName,
            StoredFileName = storedFileName,
            ContentType = file.ContentType,
            FileSizeBytes = file.Length,
            UploadedAtUtc = DateTime.UtcNow
        };
        db.TaskAttachments.Add(attachment);
        await db.SaveChangesAsync();

        return RedirectToAction(nameof(Details), new { id = taskId });
    }

    // ---- HELPERS ----

    private bool CanAccess(TaskItem task) =>
        User.IsInRole("Admin") || task.OwnerId == userManager.GetUserId(User);
}
