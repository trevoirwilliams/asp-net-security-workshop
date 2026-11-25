using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace SecureReviewPortal.ViewModels;

public class UploadAttachmentViewModel
{
    public int SecurityReviewRequestId { get; set; }

    [Required]
    [Display(Name = "Attachment")]
    public IFormFile? File { get; set; }
}
