using System.Security.Cryptography;
using AspNetCoreSecuritySection5.Models;
using AspNetCoreSecuritySection5.Services;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AspNetCoreSecuritySection5.Pages.SecureNotes;

public class AccessModel : PageModel
{
    private readonly IProtectedLinkService _protectedLinkService;
    private readonly ISecureNoteStore _noteStore;
    private readonly IConfidentialNoteCryptoService _cryptoService;

    public StoredSecureNote? Note { get; private set; }
    public string? DecryptedNoteBody { get; private set; }
    public string? ErrorMessage { get; private set; }
    public bool IsExpiredToken { get; private set; }

    public AccessModel(
        IProtectedLinkService protectedLinkService,
        ISecureNoteStore noteStore,
        IConfidentialNoteCryptoService cryptoService)
    {
        _protectedLinkService = protectedLinkService;
        _noteStore = noteStore;
        _cryptoService = cryptoService;
    }

    public async Task OnGetAsync(string? token)
    {
        if (string.IsNullOrWhiteSpace(token))
        {
            ErrorMessage = "This link is invalid.";
            return;
        }

        if (!_protectedLinkService.TryReadToken(token, out var payload, out var error) || payload is null)
        {
            if (!string.IsNullOrWhiteSpace(error) &&
                error.Contains("expired", StringComparison.OrdinalIgnoreCase))
            {
                IsExpiredToken = true;
                ErrorMessage = "This secure link has expired.";
                return;
            }

            ErrorMessage = "This link is invalid.";
            return;
        }

        var note = await _noteStore.GetAsync(payload.NoteId);
        if (note is null)
        {
            ErrorMessage = "The requested note could not be found.";
            return;
        }

        if (!string.Equals(note.RecipientEmail, payload.RecipientEmail, StringComparison.OrdinalIgnoreCase))
        {
            ErrorMessage = "This link is invalid.";
            return;
        }

        try
        {
            DecryptedNoteBody = _cryptoService.Decrypt(
                note.CiphertextBase64,
                note.NonceBase64,
                note.TagBase64);
        }
        catch (FormatException)
        {
            ErrorMessage = "Unable to read this note.";
            return;
        }
        catch (CryptographicException)
        {
            ErrorMessage = "Unable to read this note.";
            return;
        }

        Note = note;
    }
}