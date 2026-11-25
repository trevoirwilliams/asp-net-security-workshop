using System.ComponentModel.DataAnnotations;

namespace SecureReviewPortal.ViewModels;

public class ReviewChecklistItemViewModel
{
    public int Id { get; set; }

    [Required]
    [StringLength(80)]
    public string Category { get; set; } = string.Empty;

    [Required]
    [Display(Name = "Review question")]
    [StringLength(300)]
    public string ReviewQuestion { get; set; } = string.Empty;

    [Required]
    [Display(Name = "Expected protection")]
    [StringLength(300)]
    public string ExpectedProtection { get; set; } = string.Empty;

    [Range(1, 500)]
    public int SortOrder { get; set; } = 1;
}
