namespace AspNetCoreSecuritySection3.Models;

public class ProductReview
{
    public int Id { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public string ReviewerName { get; set; } = string.Empty;
    public int Rating { get; set; }
    public string ReviewText { get; set; } = string.Empty;
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
}
