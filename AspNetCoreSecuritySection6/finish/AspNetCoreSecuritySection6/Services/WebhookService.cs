using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace AspNetCoreSecuritySection6.Services;

/// <summary>
/// EXTERNAL INTEGRATION SURFACE: Sends an outbound HTTP POST notification to a
/// configurable webhook URL whenever a task is marked complete.
///
/// SR-13 — Outbound Webhook Trust Controls
///   1. Destination hostname must appear in WebhookSettings:AllowedHostnames.
///      If the allowlist is empty or the URL is not listed, the call is suppressed.
///   2. Each request is signed with HMAC-SHA256 (X-Webhook-Signature header)
///      so the receiver can verify the payload originated from this application.
/// </summary>
public class WebhookService(
    IHttpClientFactory httpClientFactory,
    IConfiguration configuration,
    ILogger<WebhookService> logger)
{
    private readonly string? _webhookUrl    = configuration["WebhookSettings:CompletionUrl"];
    private readonly string? _signingSecret = configuration["WebhookSettings:SigningSecret"];

    // SR-13: Build allowlist once at construction time from configuration
    private readonly HashSet<string> _allowedHostnames =
        new(
            configuration.GetSection("WebhookSettings:AllowedHostnames").Get<string[]>() ?? [],
            StringComparer.OrdinalIgnoreCase);

    public async Task NotifyTaskCompletedAsync(int taskId, string taskTitle)
    {
        if (string.IsNullOrWhiteSpace(_webhookUrl))
        {
            logger.LogInformation(
                "Webhook URL not configured; skipping notification for task {TaskId}.", taskId);
            return;
        }

        // SR-13: Validate destination against allowlist before any outbound call
        if (!IsAllowedDestination(_webhookUrl))
        {
            logger.LogWarning(
                "Webhook destination is not in the allowed hostnames list. " +
                "Notification suppressed for task {TaskId}.", taskId);
            return;
        }

        var payload = new
        {
            @event         = "task.completed",
            taskId,
            taskTitle,
            completedAtUtc = DateTime.UtcNow
        };

        try
        {
            var json    = JsonSerializer.Serialize(payload);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var request = new HttpRequestMessage(HttpMethod.Post, _webhookUrl) { Content = content };

            // SR-13: Sign the serialised payload so the receiver can verify authenticity
            if (!string.IsNullOrWhiteSpace(_signingSecret))
            {
                var signature = ComputeHmacSignature(json, _signingSecret);
                request.Headers.Add("X-Webhook-Signature", $"sha256={signature}");
            }

            var client   = httpClientFactory.CreateClient("webhook");
            var response = await client.SendAsync(request);

            logger.LogInformation(
                "Webhook notification sent for task {TaskId}. Status: {Status}",
                taskId, response.StatusCode);
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Webhook notification failed for task {TaskId}.", taskId);
        }
    }

    // SR-13: Returns true only when the URL's host appears in the configured allowlist
    private bool IsAllowedDestination(string url)
    {
        if (_allowedHostnames.Count == 0)
            return false; // no allowlist configured → secure-by-default: block all

        if (!Uri.TryCreate(url, UriKind.Absolute, out var uri))
            return false;

        return _allowedHostnames.Contains(uri.Host);
    }

    // SR-13: HMAC-SHA256 over the serialised JSON payload body
    private static string ComputeHmacSignature(string payload, string secret)
    {
        var keyBytes     = Encoding.UTF8.GetBytes(secret);
        var payloadBytes = Encoding.UTF8.GetBytes(payload);
        using var hmac   = new HMACSHA256(keyBytes);
        return Convert.ToHexString(hmac.ComputeHash(payloadBytes)).ToLowerInvariant();
    }
}
