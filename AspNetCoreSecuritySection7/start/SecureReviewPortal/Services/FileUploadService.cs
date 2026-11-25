using Microsoft.AspNetCore.Http;
using SecureReviewPortal.Models;

namespace SecureReviewPortal.Services;

public class FileUploadService
{
    private static readonly string[] AllowedExtensions = [".pdf", ".docx", ".txt", ".png", ".jpg", ".jpeg"];
    private const long MaxFileSizeBytes = 5 * 1024 * 1024;

    private readonly IWebHostEnvironment _environment;

    public FileUploadService(IWebHostEnvironment environment)
    {
        _environment = environment;
    }

    public string UploadRootPath => Path.Combine(_environment.ContentRootPath, "App_Data", "uploads");

    public async Task<(ReviewAttachment? attachment, string? error)> SaveAsync(
        IFormFile file,
        int requestId,
        string uploadedByUserId,
        CancellationToken cancellationToken = default)
    {
        if (file is null || file.Length == 0)
        {
            return (null, "Please choose a file.");
        }

        if (file.Length > MaxFileSizeBytes)
        {
            return (null, "File exceeds 5 MB limit.");
        }

        var extension = Path.GetExtension(file.FileName);
        if (string.IsNullOrWhiteSpace(extension) || !AllowedExtensions.Contains(extension, StringComparer.OrdinalIgnoreCase))
        {
            return (null, "File type is not allowed.");
        }

        // TODO: Section 7 review item - extension and content-type checks are not complete file validation.
        Directory.CreateDirectory(UploadRootPath);

        var storedName = $"{Guid.NewGuid():N}{extension.ToLowerInvariant()}";
        var fullPath = Path.Combine(UploadRootPath, storedName);

        await using var stream = new FileStream(fullPath, FileMode.CreateNew, FileAccess.Write, FileShare.None);
        await file.CopyToAsync(stream, cancellationToken);

        var attachment = new ReviewAttachment
        {
            SecurityReviewRequestId = requestId,
            OriginalFileName = Path.GetFileName(file.FileName),
            StoredFileName = storedName,
            ContentType = file.ContentType,
            Length = file.Length,
            UploadedByUserId = uploadedByUserId,
            UploadedAtUtc = DateTime.UtcNow
        };

        return (attachment, null);
    }
}
