using System.ComponentModel.DataAnnotations;

namespace AspNetCoreSecuritySection3.ViewModels;

// Used by the POST /api/surveys endpoint
public class ApiSurveyRequest
{
    [Required]
    public string Name { get; set; } = string.Empty;

    [Required]
    public string Email { get; set; } = string.Empty;

    // COURSE NOTE: strengthen validation in Video 3.3
    [Required]
    public int Score { get; set; }

    public string? Comment { get; set; }
}
