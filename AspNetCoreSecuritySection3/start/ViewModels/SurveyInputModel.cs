using System.ComponentModel.DataAnnotations;

namespace AspNetCoreSecuritySection3.ViewModels;

public class SurveyInputModel
{
    [Required]
    public string Name { get; set; } = string.Empty;

    // COURSE NOTE: strengthen validation in Video 3.3
    [Required]
    public string Email { get; set; } = string.Empty;

    // COURSE NOTE: strengthen validation in Video 3.3
    public int Score { get; set; }

    public string? Comment { get; set; }
}
