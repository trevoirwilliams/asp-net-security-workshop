using System.ComponentModel.DataAnnotations;

namespace SecureReviewPortal.Models;

public class SecurityReviewChecklistItem
{
    public int Id { get; set; }

    [Required]
    [StringLength(80)]
    public string Category { get; set; } = string.Empty;

    [Required]
    [StringLength(300)]
    public string ReviewQuestion { get; set; } = string.Empty;

    [Required]
    [StringLength(300)]
    public string ExpectedProtection { get; set; } = string.Empty;

    public int SortOrder { get; set; }
}
