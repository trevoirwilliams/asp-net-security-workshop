using AspNetCoreSecuritySection5.Models;
using AspNetCoreSecuritySection5.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AspNetCoreSecuritySection5.Pages.Security;

public class IntegrityDemoModel(IWebhookSignatureService webhookSignatureService) : PageModel
{
    [BindProperty]
    public IntegrityDemoRequest Input { get; set; } = new();

    public string? OriginalPayload { get; private set; }
    public string? ComputedSignature { get; private set; }
    public bool OriginalPayloadVerified { get; private set; }
    public string? TamperedPayload { get; private set; }
    public bool TamperedPayloadVerified { get; private set; }
    public bool HasProcessed { get; private set; }

    public void OnGet()
    {
        Input = new IntegrityDemoRequest
        {
            Payload = "{\"event\":\"invoice.paid\",\"amount\":100,\"status\":\"approved\"}"
        };
    }

    public void OnPost()
    {
        if (string.IsNullOrWhiteSpace(Input.Payload))
        {
            ModelState.AddModelError("Input.Payload", "Payload is required.");
            return;
        }

        OriginalPayload = Input.Payload;
        ComputedSignature = webhookSignatureService.ComputeSignature(OriginalPayload);

        OriginalPayloadVerified = webhookSignatureService.VerifySignature(
            OriginalPayload,
            ComputedSignature);

        TamperedPayload = CreateTamperedPayload(OriginalPayload);

        TamperedPayloadVerified = webhookSignatureService.VerifySignature(
            TamperedPayload,
            ComputedSignature);

        HasProcessed = true;
    }

    private static string CreateTamperedPayload(string payload)
    {
        if (payload.Contains("\"status\":\"approved\"", StringComparison.Ordinal))
        {
            return payload.Replace("\"status\":\"approved\"", "\"status\":\"declined\"", StringComparison.Ordinal);
        }

        if (payload.Contains("\"amount\":100", StringComparison.Ordinal))
        {
            return payload.Replace("\"amount\":100", "\"amount\":999", StringComparison.Ordinal);
        }

        return payload + " ";
    }
}
