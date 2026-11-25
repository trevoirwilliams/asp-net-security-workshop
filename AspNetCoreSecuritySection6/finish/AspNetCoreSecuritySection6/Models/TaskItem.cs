using System.ComponentModel.DataAnnotations;

namespace AspNetCoreSecuritySection6.Models;

public enum TaskStatus
{
    Open,
    InProgress,
    Done
}

public class TaskItem
{
    public int Id { get; set; }

    // SR-11: Server-side length and required constraints — prevents storage abuse
    [Required]
    [StringLength(200, MinimumLength = 1)]
    public string Title { get; set; } = string.Empty;

    [StringLength(4000)]
    public string? Description { get; set; }
    public TaskStatus Status { get; set; } = TaskStatus.Open;
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    public DateTime? DueDate { get; set; }

    // Owner
    public string? OwnerId { get; set; }
    public ApplicationUser? Owner { get; set; }

    public bool IsCompleted { get; set; }
    public DateTime? CompletedAtUtc { get; set; }

    // Navigation
    public ICollection<TaskComment> Comments { get; set; } = [];
    public ICollection<TaskAttachment> Attachments { get; set; } = [];
}
