using System.ComponentModel.DataAnnotations;

namespace SecureReviewPortal.Models;

public class ReviewAttachment
{
    public int Id { get; set; }

    public int SecurityReviewRequestId { get; set; }

    public SecurityReviewRequest? SecurityReviewRequest { get; set; }

    [Required]
    [StringLength(255)]
    public string OriginalFileName { get; set; } = string.Empty;

    [Required]
    [StringLength(255)]
    public string StoredFileName { get; set; } = string.Empty;

    [Required]
    [StringLength(120)]
    public string ContentType { get; set; } = string.Empty;

    public long Length { get; set; }

    [Required]
    public string UploadedByUserId { get; set; } = string.Empty;

    public DateTime UploadedAtUtc { get; set; } = DateTime.UtcNow;
}
