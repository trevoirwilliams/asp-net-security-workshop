using AspNetCoreSecuritySection3.Data;
using AspNetCoreSecuritySection3.Models;
using AspNetCoreSecuritySection3.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace AspNetCoreSecuritySection3.Controllers;

public class SupportTicketController : Controller
{
    private readonly AppDbContext _db;
    private readonly IWebHostEnvironment _env;

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

            var uploadsDir = Path.Combine(_env.WebRootPath, "uploads");
            Directory.CreateDirectory(uploadsDir);

            var fileName = model.Attachment.FileName;
            var fullPath = Path.Combine(uploadsDir, fileName);

            using var stream = new FileStream(fullPath, FileMode.Create);
            await model.Attachment.CopyToAsync(stream);

            attachmentPath = $"/uploads/{fileName}";
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
}
