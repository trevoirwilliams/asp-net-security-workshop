namespace AspNetCoreSecuritySection6.Models;

public class TaskComment
{
    public int Id { get; set; }

    public int TaskItemId { get; set; }
    public TaskItem? TaskItem { get; set; }

    public string? AuthorId { get; set; }
    public ApplicationUser? Author { get; set; }

    public string Body { get; set; } = string.Empty;
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
}
