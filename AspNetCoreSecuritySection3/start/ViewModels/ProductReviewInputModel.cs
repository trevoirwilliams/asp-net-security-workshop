using System.ComponentModel.DataAnnotations;

namespace AspNetCoreSecuritySection3.ViewModels;

public class ProductReviewInputModel
{
    [Required]
    [Display(Name = "Product Name")]
    public string ProductName { get; set; } = string.Empty;

    [Display(Name = "Your Name")]
    public string ReviewerName { get; set; } = string.Empty;

    // COURSE NOTE: strengthen validation in Video 3.3
    [Range(1, 5)]
    public int Rating { get; set; }

    [Display(Name = "Review")]
    public string ReviewText { get; set; } = string.Empty;
}
