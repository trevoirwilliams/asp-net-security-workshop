using System.ComponentModel.DataAnnotations;

namespace SecureReviewPortal.Models;

public class ReviewNote
{
    public int Id { get; set; }

    public int SecurityReviewRequestId { get; set; }

    public SecurityReviewRequest? SecurityReviewRequest { get; set; }

    [Required]
    public string AuthorUserId { get; set; } = string.Empty;

    [Required]
    [StringLength(2000)]
    public string NoteText { get; set; } = string.Empty;

    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
}
