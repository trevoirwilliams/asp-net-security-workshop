using System.ComponentModel.DataAnnotations;

namespace SecureReviewPortal.Models;

public class SecurityReviewRequest
{
    public int Id { get; set; }

    [Required]
    [StringLength(150)]
    public string Title { get; set; } = string.Empty;

    [Required]
    [StringLength(4000)]
    public string Description { get; set; } = string.Empty;

    [Required]
    [StringLength(2000)]
    public string BusinessJustification { get; set; } = string.Empty;

    [Required]
    [StringLength(150)]
    public string SystemName { get; set; } = string.Empty;

    public DataClassification DataClassification { get; set; }

    public SecurityReviewRequestStatus Status { get; set; } = SecurityReviewRequestStatus.Draft;

    [Required]
    public string OwnerUserId { get; set; } = string.Empty;

    public ApplicationUser? OwnerUser { get; set; }

    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;

    public DateTime? SubmittedAtUtc { get; set; }

    public DateTime LastUpdatedAtUtc { get; set; } = DateTime.UtcNow;

    public ICollection<ReviewAttachment> Attachments { get; set; } = new List<ReviewAttachment>();

    public ICollection<ReviewFinding> Findings { get; set; } = new List<ReviewFinding>();

    public ICollection<ReviewNote> Notes { get; set; } = new List<ReviewNote>();
}
