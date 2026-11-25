using System.Security.Cryptography;
using System.Text.Json;
using AspNetCoreSecuritySection5.Models;
using Microsoft.AspNetCore.DataProtection;

namespace AspNetCoreSecuritySection5.Services;

public class ProtectedLinkService(IDataProtectionProvider dataProtectionProvider) : IProtectedLinkService
{
    private const string Purpose = "SecureDocumentDeliveryPortal.SecureNotes.Access.v1";
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    public string CreateToken(ProtectedAccessPayload payload, TimeSpan lifetime)
    {
        payload.ExpiresUtc = DateTime.UtcNow.Add(lifetime);

        var json = JsonSerializer.Serialize(payload, JsonOptions);

        var protector = dataProtectionProvider
            .CreateProtector(Purpose)
            .ToTimeLimitedDataProtector();

        return protector.Protect(json, lifetime);
    }

    public bool TryReadToken(string token, out ProtectedAccessPayload? payload, out string? error)
    {
        payload = null;
        error = null;

        if (string.IsNullOrWhiteSpace(token))
        {
            error = "Token is required.";
            return false;
        }

        try
        {
            var protector = dataProtectionProvider
                .CreateProtector(Purpose)
                .ToTimeLimitedDataProtector();

            var json = protector.Unprotect(token, out var expiresAt);

            payload = JsonSerializer.Deserialize<ProtectedAccessPayload>(json, JsonOptions);
            if (payload is null)
            {
                error = "Token payload is invalid.";
                return false;
            }

            // Defense in depth: ensure payload and cryptographic expiry both indicate validity.
            if (payload.ExpiresUtc <= DateTime.UtcNow || expiresAt.UtcDateTime <= DateTime.UtcNow)
            {
                error = "Token has expired.";
                payload = null;
                return false;
            }

            return true;
        }
        catch (CryptographicException)
        {
            error = "Token is invalid or expired.";
            return false;
        }
        catch (JsonException)
        {
            error = "Token payload is invalid.";
            return false;
        }
    }
}