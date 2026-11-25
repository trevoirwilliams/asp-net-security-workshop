using AspNetCoreSecuritySection3.Data;
using AspNetCoreSecuritySection3.Models;
using AspNetCoreSecuritySection3.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;

namespace AspNetCoreSecuritySection3.Controllers;

public class SupportTicketController : Controller
{
    private readonly AppDbContext _db;
    private readonly IWebHostEnvironment _env;

    private const long MaxAttachmentSizeBytes = 5 * 1024 * 1024; // 5 MB
    private static readonly HashSet<string> AllowedExtensions = new(StringComparer.OrdinalIgnoreCase)
    {
        ".pdf", ".png", ".jpg", ".jpeg", ".txt", ".doc", ".docx"
    };

    private static readonly HashSet<string> AllowedContentTypes = new(StringComparer.OrdinalIgnoreCase)
    {
        "application/pdf",
        "image/png",
        "image/jpeg",
        "text/plain",
        "application/msword",
        "application/vnd.openxmlformats-officedocument.wordprocessingml.document"
    };
    public SupportTicketController(AppDbContext db, IWebHostEnvironment env)
    {
        _db = db;
        _env = env;
    }

    [HttpGet]
    public IActionResult Create() => View(new SupportTicketInputModel());

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(SupportTicketInputModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        string? attachmentPath = null;

        if (model.Attachment is { Length: > 0 })
        {
            // COURSE NOTE: harden file upload in Video 3.5
            var validationError = ValidateAttachment(model.Attachment);
            if (validationError is not null)
            {
                ModelState.AddModelError(nameof(model.Attachment), validationError);
                return View(model);
            }

            var uploadsDir = Path.Combine(_env.WebRootPath, "uploads");
            Directory.CreateDirectory(uploadsDir);

            var extension = Path.GetExtension(model.Attachment.FileName);
            var storedFileName = FormattableString.Invariant($"{Guid.NewGuid():N}{extension.ToLowerInvariant()}");
            var fullPath = Path.Combine(uploadsDir, storedFileName);

            using var stream = new FileStream(fullPath, FileMode.Create);
            await model.Attachment.CopyToAsync(stream);

            attachmentPath = $"/uploads/{storedFileName}";
        }

        var ticket = new SupportTicket
        {
            FullName = model.FullName,
            Email = model.Email,
            Category = model.Category,
            Title = model.Title,
            Description = model.Description,
            AttachmentPath = attachmentPath,
            CreatedAtUtc = DateTime.UtcNow
        };

        _db.SupportTickets.Add(ticket);
        await _db.SaveChangesAsync();

        TempData["SuccessMessage"] = "Support ticket submitted successfully.";
        return RedirectToAction(nameof(Confirmation));
    }

    public IActionResult Confirmation() => View();

    private static string? ValidateAttachment(IFormFile attachment)
{
    if (attachment.Length > MaxAttachmentSizeBytes)
        return FormattableString.Invariant($"Attachment must be {MaxAttachmentSizeBytes / (1024 * 1024)} MB or less.");

        var extension = Path.GetExtension(attachment.FileName);
        if (string.IsNullOrWhiteSpace(extension) || !AllowedExtensions.Contains(extension))
            return "Attachment type is not allowed.";

        if (string.IsNullOrWhiteSpace(attachment.ContentType) || !AllowedContentTypes.Contains(attachment.ContentType))
            return "Attachment content type is not allowed.";

        return null;
    }
}
