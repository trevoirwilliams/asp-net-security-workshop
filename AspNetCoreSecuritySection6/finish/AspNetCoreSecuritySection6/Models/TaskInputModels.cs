using System.ComponentModel.DataAnnotations;

namespace AspNetCoreSecuritySection6.Models;

/// <summary>
/// SR-10 — Mass-Assignment Prevention
/// Dedicated input DTO for task creation.
/// Only explicitly declared fields can ever be bound from the HTTP request.
/// Internal state fields (IsCompleted, CompletedAtUtc, OwnerId, CreatedAtUtc)
/// are absent from this type, so the model binder cannot touch them even if an
/// attacker posts them.
/// </summary>
public class CreateTaskInput
{
    [Required]
    [StringLength(200, MinimumLength = 1)]
    public string Title { get; set; } = string.Empty;

    [StringLength(4000)]
    public string? Description { get; set; }

    public TaskStatus Status { get; set; } = TaskStatus.Open;

    public DateTime? DueDate { get; set; }
}

/// <summary>
/// SR-10 — Mass-Assignment Prevention
/// Dedicated input DTO for task editing. Same principle as above.
/// The Id is passed via the route, not the body, so it cannot be spoofed
/// by a form field either.
/// </summary>
public class EditTaskInput
{
    [Required]
    [StringLength(200, MinimumLength = 1)]
    public string Title { get; set; } = string.Empty;

    [StringLength(4000)]
    public string? Description { get; set; }

    public TaskStatus Status { get; set; } = TaskStatus.Open;

    public DateTime? DueDate { get; set; }
}
