using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using AspNetCoreSecuritySection5.Models;
using AspNetCoreSecuritySection5.Services;
using System.ComponentModel.DataAnnotations;

namespace AspNetCoreSecuritySection5.Pages.SecureNotes;

[Authorize]
public class CreateModel : PageModel
{
    private readonly ISecureNoteStore _noteStore;
    private readonly UserManager<IdentityUser> _userManager;

    [BindProperty]
    public InputModel Input { get; set; } = new();

    public class InputModel
    {
        [Required]
        [StringLength(200)]
        public string Title { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        public string RecipientEmail { get; set; } = string.Empty;

        [Required]
        public string PlainTextNote { get; set; } = string.Empty;
    }

    public CreateModel(ISecureNoteStore noteStore, UserManager<IdentityUser> userManager)
    {
        _noteStore = noteStore;
        _userManager = userManager;
    }

    public void OnGet()
    {
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        var userId = _userManager.GetUserId(User);
        if (string.IsNullOrEmpty(userId))
        {
            return Redirect("/Identity/Account/Login");
        }

        var note = new StoredSecureNote
        {
            Id = Guid.NewGuid(),
            Title = Input.Title,
            RecipientEmail = Input.RecipientEmail,
            PlainTextNote = Input.PlainTextNote,
            CreatedUtc = DateTime.UtcNow,
            CreatedByUserId = userId
        };

        await _noteStore.SaveAsync(note);

        return RedirectToPage("Result", new { id = note.Id });
    }
}
