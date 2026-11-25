using System.ComponentModel.DataAnnotations;
using SecureReviewPortal.Models;

namespace SecureReviewPortal.ViewModels;

public class EditReviewRequestViewModel
{
    public int Id { get; set; }

    [Required]
    [StringLength(150)]
    public string Title { get; set; } = string.Empty;

    [Required]
    [StringLength(4000)]
    public string Description { get; set; } = string.Empty;

    [Required]
    [Display(Name = "Business justification")]
    [StringLength(2000)]
    public string BusinessJustification { get; set; } = string.Empty;

    [Required]
    [Display(Name = "System name")]
    [StringLength(150)]
    public string SystemName { get; set; } = string.Empty;

    [Display(Name = "Data classification")]
    public DataClassification DataClassification { get; set; }

    public SecurityReviewRequestStatus Status { get; set; }
}
