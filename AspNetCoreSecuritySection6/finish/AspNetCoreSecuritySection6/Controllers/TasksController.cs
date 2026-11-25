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
///
/// Security controls applied here:
///   SR-08  — File upload extension allowlist + 5 MB size cap + filename sanitisation
///   SR-09  — Files stored outside wwwroot; served via authorised DownloadAttachment action
///   SR-10  — CreateTaskInput / EditTaskInput DTOs prevent mass-assignment of internal state
///   SR-11  — Comment body length guard at the action boundary
///   SR-16  — Structured AUDIT log entries on every security-significant state change
/// </summary>
[Authorize]
public class TasksController(
    ApplicationDbContext db,
    UserManager<ApplicationUser> userManager,
    IWebHostEnvironment env,
    WebhookService webhook,
    ILogger<TasksController> logger) : Controller
{
    // SR-08: Only these extensions may be uploaded — everything else is rejected
    private static readonly HashSet<string> AllowedExtensions =
        new(StringComparer.OrdinalIgnoreCase) { ".pdf", ".png", ".jpg", ".jpeg", ".gif", ".txt", ".docx", ".xlsx" };

    // SR-08: Hard cap — files larger than 5 MB are rejected before being written to disk
    private const long MaxFileSizeBytes = 5 * 1024 * 1024;

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

    public IActionResult Create() => View(new CreateTaskInput());

    [HttpPost]
    [ValidateAntiForgeryToken]
    // SR-10: Bind the DTO, not the full TaskItem entity.
    // IsCompleted, CompletedAtUtc, OwnerId, and CreatedAtUtc are absent from
    // CreateTaskInput so the model binder cannot touch them even if an
    // attacker includes them in the POST body.
    public async Task<IActionResult> Create(CreateTaskInput input)
    {
        if (!ModelState.IsValid) return View(input);

        var task = new TaskItem
        {
            Title        = input.Title,
            Description  = input.Description,
            Status       = input.Status,
            DueDate      = input.DueDate,
            OwnerId      = userManager.GetUserId(User), // server-assigned — never from input
            CreatedAtUtc = DateTime.UtcNow              // server-assigned — never from input
        };

        db.Tasks.Add(task);
        await db.SaveChangesAsync();

        // SR-16: Structured audit log — task created
        logger.LogInformation(
            "AUDIT | Action=TaskCreated | UserId={UserId} | TaskId={TaskId}",
            task.OwnerId, task.Id);

        return RedirectToAction(nameof(Index));
    }

    // ---- EDIT ----

    public async Task<IActionResult> Edit(int id)
    {
        var task = await db.Tasks.FindAsync(id);
        if (task is null) return NotFound();
        if (!CanAccess(task)) return Forbid();

        // SR-10: Project entity → DTO; view never sees IsCompleted or OwnerId
        var input = new EditTaskInput
        {
            Title       = task.Title,
            Description = task.Description,
            Status      = task.Status,
            DueDate     = task.DueDate
        };
        ViewData["TaskId"] = id;
        return View(input);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    // SR-10: Bind EditTaskInput — route id is trusted; body can only update editable fields
    public async Task<IActionResult> Edit(int id, EditTaskInput input)
    {
        var task = await db.Tasks.FindAsync(id);
        if (task is null) return NotFound();
        if (!CanAccess(task)) return Forbid();

        if (!ModelState.IsValid)
        {
            ViewData["TaskId"] = id;
            return View(input);
        }

        task.Title       = input.Title;
        task.Description = input.Description;
        task.Status      = input.Status;
        task.DueDate     = input.DueDate;
        await db.SaveChangesAsync();

        // SR-16: Structured audit log — task edited
        logger.LogInformation(
            "AUDIT | Action=TaskEdited | UserId={UserId} | TaskId={TaskId}",
            userManager.GetUserId(User), id);

        return RedirectToAction(nameof(Index));
    }

    // ---- MARK COMPLETE ----

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Complete(int id)
    {
        var task = await db.Tasks.FindAsync(id);
        if (task is null) return NotFound();
        if (!CanAccess(task)) return Forbid();

        task.IsCompleted    = true;
        task.CompletedAtUtc = DateTime.UtcNow;
        task.Status         = Models.TaskStatus.Done;
        await db.SaveChangesAsync();

        // SR-16: Structured audit log — task completed (also triggers outbound webhook)
        logger.LogInformation(
            "AUDIT | Action=TaskCompleted | UserId={UserId} | TaskId={TaskId}",
            userManager.GetUserId(User), id);

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

        // SR-11: Action-boundary guard — catches raw API calls that bypass model annotations
        if (string.IsNullOrWhiteSpace(body) || body.Length > 4000)
        {
            TempData["Error"] = "Comment must be between 1 and 4 000 characters.";
            return RedirectToAction(nameof(Details), new { id = taskId });
        }

        var comment = new TaskComment
        {
            TaskItemId   = taskId,
            AuthorId     = userManager.GetUserId(User),
            Body         = body,
            CreatedAtUtc = DateTime.UtcNow
        };
        db.TaskComments.Add(comment);
        await db.SaveChangesAsync();

        // SR-16: Structured audit log — comment posted
        logger.LogInformation(
            "AUDIT | Action=CommentAdded | UserId={UserId} | TaskId={TaskId} | CommentId={CommentId}",
            comment.AuthorId, taskId, comment.Id);

        return RedirectToAction(nameof(Details), new { id = taskId });
    }

    // ---- UPLOAD ATTACHMENT ----

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

        // SR-08: Reject files that exceed the maximum allowed size
        if (file.Length > MaxFileSizeBytes)
        {
            TempData["Error"] = "File exceeds the 5 MB maximum allowed size.";
            return RedirectToAction(nameof(Details), new { id = taskId });
        }

        // SR-08: Reject files whose extension is not on the allowlist
        var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
        if (!AllowedExtensions.Contains(ext))
        {
            TempData["Error"] = $"File type '{ext}' is not permitted. " +
                                $"Allowed types: {string.Join(", ", AllowedExtensions)}";
            return RedirectToAction(nameof(Details), new { id = taskId });
        }

        // SR-09: Store files in secure-uploads/ (outside wwwroot) so they are
        //        never directly addressable as static assets
        var uploadsDir = Path.Combine(env.ContentRootPath, "secure-uploads");
        Directory.CreateDirectory(uploadsDir);

        var storedFileName = $"{Guid.NewGuid()}{ext}";
        var filePath       = Path.Combine(uploadsDir, storedFileName);

        using (var stream = System.IO.File.Create(filePath))
        {
            await file.CopyToAsync(stream);
        }

        var attachment = new TaskAttachment
        {
            TaskItemId       = taskId,
            UploadedById     = userManager.GetUserId(User),
            // SR-08: Strip any path component an attacker may embed in the filename
            OriginalFileName = Path.GetFileName(file.FileName),
            StoredFileName   = storedFileName,
            ContentType      = file.ContentType,
            FileSizeBytes    = file.Length,
            UploadedAtUtc    = DateTime.UtcNow
        };
        db.TaskAttachments.Add(attachment);
        await db.SaveChangesAsync();

        // SR-16: Structured audit log — file uploaded
        logger.LogInformation(
            "AUDIT | Action=AttachmentUploaded | UserId={UserId} | TaskId={TaskId} | AttachmentId={AttachmentId} | FileName={FileName}",
            attachment.UploadedById, taskId, attachment.Id, attachment.OriginalFileName);

        return RedirectToAction(nameof(Details), new { id = taskId });
    }

    // ---- DOWNLOAD ATTACHMENT ----

    /// <summary>
    /// SR-09: Replaces the old direct /uploads/&lt;guid&gt; static URL.
    /// Authorization is checked before any bytes are returned, and a
    /// path-traversal guard ensures the resolved path stays within secure-uploads/.
    /// </summary>
    public async Task<IActionResult> DownloadAttachment(int id)
    {
        var attachment = await db.TaskAttachments
            .Include(a => a.TaskItem)
            .FirstOrDefaultAsync(a => a.Id == id);

        if (attachment is null) return NotFound();
        if (attachment.TaskItem is null || !CanAccess(attachment.TaskItem)) return Forbid();

        // SR-09: Path-traversal guard — verify the resolved path stays inside secure-uploads/
        var uploadsBase = Path.GetFullPath(Path.Combine(env.ContentRootPath, "secure-uploads"));
        var filePath    = Path.GetFullPath(Path.Combine(uploadsBase, attachment.StoredFileName));

        if (!filePath.StartsWith(uploadsBase + Path.DirectorySeparatorChar, StringComparison.OrdinalIgnoreCase))
            return BadRequest();

        if (!System.IO.File.Exists(filePath))
            return NotFound();

        // SR-16: Structured audit log — file downloaded
        logger.LogInformation(
            "AUDIT | Action=AttachmentDownloaded | UserId={UserId} | AttachmentId={AttachmentId}",
            userManager.GetUserId(User), id);

        var stream = System.IO.File.OpenRead(filePath);
        return File(stream, attachment.ContentType, attachment.OriginalFileName);
    }

    // ---- HELPERS ----

    private bool CanAccess(TaskItem task) =>
        User.IsInRole("Admin") || task.OwnerId == userManager.GetUserId(User);
}
