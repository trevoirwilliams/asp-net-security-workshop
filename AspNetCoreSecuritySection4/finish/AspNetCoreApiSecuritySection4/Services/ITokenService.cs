using Microsoft.AspNetCore.Identity;

namespace AspNetCoreApiSecuritySection4.Services;

public interface ITokenService
{
    Task<(string Token, DateTime ExpiresAtUtc)> CreateTokenAsync(IdentityUser user);
}

