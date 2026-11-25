using System.ComponentModel.DataAnnotations;
using SecureReviewPortal.Models;

namespace SecureReviewPortal.ViewModels;

public class ReviewFindingViewModel
{
    public int SecurityReviewRequestId { get; set; }

    [Required]
    [StringLength(180)]
    public string Title { get; set; } = string.Empty;

    [Required]
    [StringLength(4000)]
    public string Description { get; set; } = string.Empty;

    public FindingSeverity Severity { get; set; } = FindingSeverity.Medium;

    public FindingSource Source { get; set; } = FindingSource.ManualReview;

    public FindingStatus Status { get; set; } = FindingStatus.Open;

    [StringLength(2000)]
    public string? Evidence { get; set; }

    [Display(Name = "Recommended action")]
    [StringLength(2000)]
    public string? RecommendedAction { get; set; }
}
