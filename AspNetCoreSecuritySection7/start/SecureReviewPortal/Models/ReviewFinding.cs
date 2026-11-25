using System.ComponentModel.DataAnnotations;

namespace SecureReviewPortal.Models;

public class ReviewFinding
{
    public int Id { get; set; }

    public int SecurityReviewRequestId { get; set; }

    public SecurityReviewRequest? SecurityReviewRequest { get; set; }

    [Required]
    [StringLength(180)]
    public string Title { get; set; } = string.Empty;

    [Required]
    [StringLength(4000)]
    public string Description { get; set; } = string.Empty;

    public FindingSeverity Severity { get; set; }

    public FindingSource Source { get; set; }

    public FindingStatus Status { get; set; } = FindingStatus.Open;

    [StringLength(2000)]
    public string? Evidence { get; set; }

    [StringLength(2000)]
    public string? RecommendedAction { get; set; }

    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
}
