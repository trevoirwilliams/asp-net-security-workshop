using Microsoft.AspNetCore.Identity;

namespace AspNetCoreSecuritySection6.Models;

public class ApplicationUser : IdentityUser
{
    public string? DisplayName { get; set; }
}
