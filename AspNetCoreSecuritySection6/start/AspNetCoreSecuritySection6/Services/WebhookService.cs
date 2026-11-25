namespace AspNetCoreSecuritySection6.Services;

/// <summary>
/// EXTERNAL INTEGRATION SURFACE: Sends an outbound HTTP POST notification to a
/// configurable webhook URL whenever a task is marked complete.
/// 
/// Webhook URL is read from appsettings.json: "WebhookSettings:CompletionUrl"
/// This is intentionally simple for threat modeling discussion.
/// </summary>
public class WebhookService(IHttpClientFactory httpClientFactory, IConfiguration configuration, ILogger<WebhookService> logger)
{
    private readonly string? _webhookUrl = configuration["WebhookSettings:CompletionUrl"];

    public async Task NotifyTaskCompletedAsync(int taskId, string taskTitle)
    {
        if (string.IsNullOrWhiteSpace(_webhookUrl))
        {
            logger.LogInformation("Webhook URL not configured; skipping notification for task {TaskId}.", taskId);
            return;
        }

        var payload = new
        {
            @event = "task.completed",
            taskId,
            taskTitle,
            completedAtUtc = DateTime.UtcNow
        };

        try
        {
            var client = httpClientFactory.CreateClient("webhook");
            // THREAT MODEL NOTE: No authentication on outbound request; URL is caller-controlled.
            var response = await client.PostAsJsonAsync(_webhookUrl, payload);
            logger.LogInformation("Webhook notification sent for task {TaskId}. Status: {Status}", taskId, response.StatusCode);
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Webhook notification failed for task {TaskId}.", taskId);
        }
    }
}
