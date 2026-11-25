namespace AspNetCoreSecuritySection3.ViewModels;

public class SearchViewModel
{
    public string? Query { get; set; }
    public string? Category { get; set; }
    public List<AspNetCoreSecuritySection3.Models.SupportTicket> Results { get; set; } = [];
}
