using System;

namespace AspNetCoreSecuritySection5.Models;

public class ProtectedAccessPayload
{
    public Guid NoteId { get; set; }
    public string RecipientEmail { get; set; } = string.Empty;
    public DateTime ExpiresUtc { get; set; }
}
