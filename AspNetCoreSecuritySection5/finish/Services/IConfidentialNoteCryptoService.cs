using System;
using AspNetCoreSecuritySection5.Models;

namespace AspNetCoreSecuritySection5.Services;

public interface IConfidentialNoteCryptoService
{
    EncryptedNoteResult Encrypt(string plaintext);
    string Decrypt(string ciphertextBase64, string nonceBase64, string tagBase64);
}
