using Microsoft.AspNetCore.Mvc.RazorPages;
using AspNetCoreSecuritySection5.Models;
using AspNetCoreSecuritySection5.Services;

namespace AspNetCoreSecuritySection5.Pages.SecureNotes;

public class ResultModel : PageModel
{
    private readonly ISecureNoteStore _noteStore;

    public StoredSecureNote? Note { get; set; }

    public ResultModel(ISecureNoteStore noteStore)
    {
        _noteStore = noteStore;
    }

    public async Task OnGetAsync(Guid? id)
    {
        if (id.HasValue)
        {
            Note = await _noteStore.GetAsync(id.Value);
        }
    }
}
