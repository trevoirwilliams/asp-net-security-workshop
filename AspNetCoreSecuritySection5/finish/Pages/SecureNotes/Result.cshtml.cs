using Microsoft.AspNetCore.Mvc.RazorPages;
using AspNetCoreSecuritySection5.Models;
using AspNetCoreSecuritySection5.Services;
using Microsoft.AspNetCore.Mvc;

namespace AspNetCoreSecuritySection5.Pages.SecureNotes;

public class ResultModel : PageModel
{
    private readonly ISecureNoteStore _noteStore;
    private readonly IProtectedLinkService _protectedLinkService;
    private static readonly TimeSpan LinkLifetime = TimeSpan.FromSeconds(15);
    public StoredSecureNote? Note { get; set; }
    public string? SecureAccessLink { get; private set; }
    public ResultModel(ISecureNoteStore noteStore, IProtectedLinkService protectedLinkService)
    {
        _noteStore = noteStore;
        _protectedLinkService = protectedLinkService;
    }

    public async Task OnGetAsync(Guid? id)
    {
        if (!id.HasValue)
        {
            return;
        }

        Note = await _noteStore.GetAsync(id.Value);
        if (Note is null)
        {
            return;
        }
        
        var payload = new ProtectedAccessPayload
        {
            NoteId = Note.Id,
            RecipientEmail = Note.RecipientEmail
        };

        var token = _protectedLinkService.CreateToken(payload, LinkLifetime);

        SecureAccessLink = Url.Page(
            "/SecureNotes/Access",
            pageHandler: null,
            values: new { token },
            protocol: Request.Scheme);
    }
}
