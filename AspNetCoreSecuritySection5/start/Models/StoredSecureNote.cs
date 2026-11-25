using System.ComponentModel.DataAnnotations;

namespace AspNetCoreSecuritySection5.Models;

public class StoredSecureNote
{
    [Required]
    public Guid Id { get; set; }

    [Required]
    [StringLength(200)]
    public string Title { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    public string RecipientEmail { get; set; } = string.Empty;

    [Required]
    public string PlainTextNote { get; set; } = string.Empty;

    [Required]
    public DateTime CreatedUtc { get; set; }

    [Required]
    public string CreatedByUserId { get; set; } = string.Empty;
}
