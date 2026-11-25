using System.Security.Cryptography;
using System.Text;
using AspNetCoreSecuritySection5.Options;
using Microsoft.Extensions.Options;

namespace AspNetCoreSecuritySection5.Services;

public class WebhookSignatureService : IWebhookSignatureService
{
    private readonly byte[] _key;

    public WebhookSignatureService(IOptions<SecureDocumentOptions> options)
    {
        var base64Key = options.Value.WebhookSigningKeyBase64;
        if (string.IsNullOrWhiteSpace(base64Key))
        {
            throw new InvalidOperationException("SecureDocument:WebhookSigningKeyBase64 is not configured.");
        }

        try
        {
            _key = Convert.FromBase64String(base64Key);
        }
        catch (FormatException ex)
        {
            throw new InvalidOperationException(
                "SecureDocument:WebhookSigningKeyBase64 must be valid Base64.",
                ex);
        }
    }

    public string ComputeSignature(string payload)
    {
        payload ??= string.Empty;

        using var hmac = new HMACSHA256(_key);
        var payloadBytes = Encoding.UTF8.GetBytes(payload);
        var hashBytes = hmac.ComputeHash(payloadBytes);

        return Convert.ToBase64String(hashBytes);
    }

    public bool VerifySignature(string payload, string providedSignature)
    {
        payload ??= string.Empty;

        if (string.IsNullOrWhiteSpace(providedSignature))
        {
            return false;
        }

        byte[] providedBytes;
        try
        {
            providedBytes = Convert.FromBase64String(providedSignature);
        }
        catch (FormatException)
        {
            return false;
        }

        using var hmac = new HMACSHA256(_key);
        var expectedBytes = hmac.ComputeHash(Encoding.UTF8.GetBytes(payload));

        return CryptographicOperations.FixedTimeEquals(expectedBytes, providedBytes);
    }
}
