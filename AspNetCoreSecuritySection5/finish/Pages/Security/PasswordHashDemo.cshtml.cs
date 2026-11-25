using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AspNetCoreSecuritySection5.Pages.Security;

public class PasswordHashDemoModel(IPasswordHasher<IdentityUser> passwordHasher) : PageModel
{
    [BindProperty]
    public string? EnteredPassword { get; set; }

    public string? GeneratedHash { get; set; }
    public bool CorrectPasswordVerified { get; set; }
    public bool WrongPasswordVerified { get; set; }
    public bool HasProcessed { get; private set; }

    public void OnGet()
    {
    }

    public void OnPost()
    {
        if (string.IsNullOrWhiteSpace(EnteredPassword))
        {
            ModelState.AddModelError("EnteredPassword", "Password is required.");
            return;
        }

        var demoUser = new IdentityUser { UserName = "demo@example.com" };

        GeneratedHash = passwordHasher.HashPassword(demoUser, EnteredPassword);

        var correctVerification = passwordHasher.VerifyHashedPassword(
            demoUser,
            GeneratedHash,
            EnteredPassword);
        CorrectPasswordVerified = correctVerification == PasswordVerificationResult.Success;

        var wrongPasswordVerification = passwordHasher.VerifyHashedPassword(
            demoUser,
            GeneratedHash,
            EnteredPassword + "wrong");
        WrongPasswordVerified = wrongPasswordVerification == PasswordVerificationResult.Success;

        HasProcessed = true;
    }
}