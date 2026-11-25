using System;
using AspNetCoreSecuritySection5.Models;

namespace AspNetCoreSecuritySection5.Services;

public interface IProtectedLinkService
{
    string CreateToken(ProtectedAccessPayload payload, TimeSpan lifetime);
    bool TryReadToken(string token, out ProtectedAccessPayload? payload, out string? error);
}
