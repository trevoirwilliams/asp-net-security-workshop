using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace AspNetCoreSecuritySection3.ViewModels;

public class SupportTicketInputModel
{
    [Required]
    [Display(Name = "Full Name")]
    public string FullName { get; set; } = string.Empty;

    [Required]
    public string Email { get; set; } = string.Empty;

    public string Category { get; set; } = string.Empty;

    [Required]
    public string Title { get; set; } = string.Empty;

    [Required]
    public string Description { get; set; } = string.Empty;

    // COURSE NOTE: harden file upload in Video 3.5
    public IFormFile? Attachment { get; set; }
}
