using System.ComponentModel.DataAnnotations;

namespace AspNetCoreSecuritySection3.ViewModels;

public class SurveyInputModel : IValidatableObject
{
    [Required]
    public string Name { get; set; } = string.Empty;

    // COURSE NOTE: strengthen validation in Video 3.3
    [Required]
    [EmailAddress]
    [StringLength(100)]
    public string Email { get; set; } = string.Empty;

    // COURSE NOTE: strengthen validation in Video 3.3
    [Range(1,5)]
    public int Score { get; set; }

    [StringLength(500)]
    public string? Comment { get; set; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (Score <= 3 && string.IsNullOrWhiteSpace(Comment))
        {
            yield return new ValidationResult(
                "A comment is required when the score is 3 or lower.",
                new[] { nameof(Comment) });
        }
    }
}
