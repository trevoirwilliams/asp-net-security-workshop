using Microsoft.AspNetCore.Http;
using SecureReviewPortal.Models;

namespace SecureReviewPortal.Services;

public class FileUploadService
{
    private static readonly string[] AllowedExtensions = [".pdf", ".docx", ".txt", ".png", ".jpg", ".jpeg"];
    private static readonly Dictionary<string, string> ExpectedContentTypes = new(StringComparer.OrdinalIgnoreCase)
    {
        [".pdf"] = "application/pdf",
        [".docx"] = "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
        [".txt"] = "text/plain",
        [".png"] = "image/png",
        [".jpg"] = "image/jpeg",
        [".jpeg"] = "image/jpeg"
    };

    private static readonly Dictionary<string, byte[][]> AllowedSignatures = new(StringComparer.OrdinalIgnoreCase)
    {
        [".pdf"] = [[0x25, 0x50, 0x44, 0x46, 0x2D]],
        [".png"] = [[0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A]],
        [".jpg"] = [[0xFF, 0xD8, 0xFF]],
        [".jpeg"] = [[0xFF, 0xD8, 0xFF]],
        [".docx"] = [[0x50, 0x4B, 0x03, 0x04], [0x50, 0x4B, 0x05, 0x06], [0x50, 0x4B, 0x07, 0x08]]
    };

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

        var originalFileName = Path.GetFileName(file.FileName);
        if (string.IsNullOrWhiteSpace(originalFileName) || originalFileName.Length > 255 || originalFileName.Any(char.IsControl))
        {
            return (null, "File name is invalid.");
        }

        var extension = Path.GetExtension(originalFileName);
        if (string.IsNullOrWhiteSpace(extension))
        {
            return (null, "File type is not allowed.");
        }

        var normalizedExtension = extension.ToLowerInvariant();
        if (!AllowedExtensions.Contains(normalizedExtension, StringComparer.OrdinalIgnoreCase))
        {
            return (null, "File type is not allowed.");
        }

        if (!ExpectedContentTypes.TryGetValue(normalizedExtension, out var expectedContentType))
        {
            return (null, "File type is not allowed.");
        }

        if (!IsExpectedContentType(file.ContentType, expectedContentType))
        {
            return (null, "File content type does not match the allowed file type.");
        }

        var hasValidSignature = await HasValidSignatureAsync(file, normalizedExtension, cancellationToken);
        if (!hasValidSignature)
        {
            return (null, "File content does not match the allowed file type.");
        }

        Directory.CreateDirectory(UploadRootPath);

        var storedName = $"{Guid.NewGuid():N}{normalizedExtension}";
        var fullPath = Path.Combine(UploadRootPath, storedName);

        await using var stream = new FileStream(fullPath, FileMode.CreateNew, FileAccess.Write, FileShare.None);
        await file.CopyToAsync(stream, cancellationToken);

        var attachment = new ReviewAttachment
        {
            SecurityReviewRequestId = requestId,
            OriginalFileName = originalFileName,
            StoredFileName = storedName,
            ContentType = expectedContentType,
            Length = file.Length,
            UploadedByUserId = uploadedByUserId,
            UploadedAtUtc = DateTime.UtcNow
        };

        return (attachment, null);
    }

    private static bool IsExpectedContentType(string providedContentType, string expectedContentType)
    {
        if (string.IsNullOrWhiteSpace(providedContentType))
        {
            return false;
        }

        var normalized = providedContentType.Split(';', 2)[0].Trim();
        return string.Equals(normalized, expectedContentType, StringComparison.OrdinalIgnoreCase);
    }

    private static async Task<bool> HasValidSignatureAsync(IFormFile file, string extension, CancellationToken cancellationToken)
    {
        await using var stream = file.OpenReadStream();
        var buffer = new byte[16];
        var bytesRead = await stream.ReadAsync(buffer.AsMemory(0, buffer.Length), cancellationToken);

        if (bytesRead <= 0)
        {
            return false;
        }

        if (string.Equals(extension, ".txt", StringComparison.OrdinalIgnoreCase))
        {
            return IsLikelyText(buffer, bytesRead);
        }

        if (!AllowedSignatures.TryGetValue(extension, out var signatures))
        {
            return false;
        }

        return signatures.Any(signature => bytesRead >= signature.Length && buffer.AsSpan(0, signature.Length).SequenceEqual(signature));
    }

    private static bool IsLikelyText(byte[] buffer, int bytesRead)
    {
        for (var i = 0; i < bytesRead; i++)
        {
            if (buffer[i] == 0)
            {
                return false;
            }
        }

        return true;
    }
}
