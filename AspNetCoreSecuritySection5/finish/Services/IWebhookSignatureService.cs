using System;

namespace AspNetCoreSecuritySection5.Services;

public interface IWebhookSignatureService
{
    string ComputeSignature(string payload);
    bool VerifySignature(string payload, string providedSignature);
}
