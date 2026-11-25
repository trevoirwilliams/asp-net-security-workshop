namespace AspNetCoreSecuritySection3.Models;

public class SupportTicket
{
    public int Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string? AttachmentPath { get; set; }
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
}
