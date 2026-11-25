namespace AspNetCoreSecuritySection5.Options;

public class SecureDocumentOptions
{
    public string KeyRingPath { get; set; } = string.Empty;

    public string ApplicationName { get; set; } = string.Empty;

    public string EncryptionKeyBase64 { get; set; } = string.Empty;

    public string WebhookSigningKeyBase64 { get; set; } = string.Empty;
}
