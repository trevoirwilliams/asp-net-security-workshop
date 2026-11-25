using System;

namespace AspNetCoreSecuritySection5.Models;

public sealed class EncryptedNoteResult
{
    public string CiphertextBase64 { get; init; } = string.Empty;
    public string NonceBase64 { get; init; } = string.Empty;
    public string TagBase64 { get; init; } = string.Empty;
}
