using AspNetCoreSecuritySection5.Options;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Options;

namespace AspNetCoreSecuritySection5.Pages.Security;

public class ConfigurationCheckModel(
    IOptions<SecureDocumentOptions> options,
    IWebHostEnvironment environment) : PageModel
{
    private readonly SecureDocumentOptions _options = options.Value;
    private readonly IWebHostEnvironment _environment = environment;

    public bool EncryptionKeyConfigured { get; private set; }
    public bool WebhookSigningKeyConfigured { get; private set; }
    public bool KeyRingPathExists { get; private set; }
    public bool ApplicationNameConfigured { get; private set; }

    public void OnGet()
    {
        EncryptionKeyConfigured = !string.IsNullOrWhiteSpace(_options.EncryptionKeyBase64);
        WebhookSigningKeyConfigured = !string.IsNullOrWhiteSpace(_options.WebhookSigningKeyBase64);
        ApplicationNameConfigured = !string.IsNullOrWhiteSpace(_options.ApplicationName);

        var keyRingPath = _options.KeyRingPath;
        if (string.IsNullOrWhiteSpace(keyRingPath))
        {
            KeyRingPathExists = false;
            return;
        }

        var fullKeyRingPath = Path.IsPathRooted(keyRingPath)
            ? keyRingPath
            : Path.Combine(_environment.ContentRootPath, keyRingPath);

        KeyRingPathExists = Directory.Exists(fullKeyRingPath);
    }
}
