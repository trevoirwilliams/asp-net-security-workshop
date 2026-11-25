using AspNetCoreSecuritySection5.Models;

namespace AspNetCoreSecuritySection5.Services;

public interface ISecureNoteStore
{
    Task SaveAsync(StoredSecureNote note);
    Task<StoredSecureNote?> GetAsync(Guid id);
}
