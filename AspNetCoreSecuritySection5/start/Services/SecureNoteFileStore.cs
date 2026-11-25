using AspNetCoreSecuritySection5.Models;
using System.Text.Json;

namespace AspNetCoreSecuritySection5.Services;

public class SecureNoteFileStore : ISecureNoteStore
{
    private readonly string _notesDirectory;

    public SecureNoteFileStore(IWebHostEnvironment environment)
    {
        _notesDirectory = Path.Combine(environment.ContentRootPath, "App_Data", "SecureNotes");
        Directory.CreateDirectory(_notesDirectory);
    }

    public async Task SaveAsync(StoredSecureNote note)
    {
        var filePath = Path.Combine(_notesDirectory, $"{note.Id}.json");
        var json = JsonSerializer.Serialize(note, new JsonSerializerOptions { WriteIndented = true });
        await File.WriteAllTextAsync(filePath, json);
    }

    public async Task<StoredSecureNote?> GetAsync(Guid id)
    {
        var filePath = Path.Combine(_notesDirectory, $"{id}.json");
        if (!File.Exists(filePath))
        {
            return null;
        }

        var json = await File.ReadAllTextAsync(filePath);
        return JsonSerializer.Deserialize<StoredSecureNote>(json);
    }
}
