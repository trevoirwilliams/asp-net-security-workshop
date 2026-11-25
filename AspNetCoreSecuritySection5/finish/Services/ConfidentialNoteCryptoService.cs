using System.Security.Cryptography;
using System.Text;
using AspNetCoreSecuritySection5.Models;
using AspNetCoreSecuritySection5.Options;
using Microsoft.Extensions.Options;

namespace AspNetCoreSecuritySection5.Services;

public class ConfidentialNoteCryptoService : IConfidentialNoteCryptoService
{
    private const int NonceSize = 12;
    private const int TagSize = 16;
    private readonly byte[] _key;

    public ConfidentialNoteCryptoService(IOptions<SecureDocumentOptions> options)
    {
        var keyBase64 = options.Value.EncryptionKeyBase64;
        if (string.IsNullOrWhiteSpace(keyBase64))
        {
            throw new InvalidOperationException("SecureDocument:EncryptionKeyBase64 is not configured.");
        }

        try
        {
            _key = Convert.FromBase64String(keyBase64);
        }
        catch (FormatException ex)
        {
            throw new InvalidOperationException("SecureDocument:EncryptionKeyBase64 must be valid Base64.", ex);
        }

        if (_key.Length != 32)
        {
            throw new InvalidOperationException("SecureDocument:EncryptionKeyBase64 must decode to 32 bytes for this lesson.");
        }
    }

    public EncryptedNoteResult Encrypt(string plaintext)
    {
        plaintext ??= string.Empty;

        var nonce = new byte[NonceSize];
        RandomNumberGenerator.Fill(nonce);

        var plaintextBytes = Encoding.UTF8.GetBytes(plaintext);
        var ciphertext = new byte[plaintextBytes.Length];
        var tag = new byte[TagSize];

        using var aesGcm = new AesGcm(_key, TagSize);
        aesGcm.Encrypt(nonce, plaintextBytes, ciphertext, tag);

        return new EncryptedNoteResult
        {
            CiphertextBase64 = Convert.ToBase64String(ciphertext),
            NonceBase64 = Convert.ToBase64String(nonce),
            TagBase64 = Convert.ToBase64String(tag)
        };
    }

    public string Decrypt(string ciphertextBase64, string nonceBase64, string tagBase64)
    {
        var ciphertext = Convert.FromBase64String(ciphertextBase64);
        var nonce = Convert.FromBase64String(nonceBase64);
        var tag = Convert.FromBase64String(tagBase64);

        if (nonce.Length != NonceSize)
        {
            throw new CryptographicException("Invalid nonce.");
        }

        if (tag.Length != TagSize)
        {
            throw new CryptographicException("Invalid authentication tag.");
        }

        var plaintextBytes = new byte[ciphertext.Length];

        using var aesGcm = new AesGcm(_key, TagSize);
        aesGcm.Decrypt(nonce, ciphertext, tag, plaintextBytes);

        return Encoding.UTF8.GetString(plaintextBytes);
    }
}
