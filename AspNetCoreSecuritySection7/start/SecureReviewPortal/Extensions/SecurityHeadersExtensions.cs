namespace SecureReviewPortal.Extensions;

public static class SecurityHeadersExtensions
{
    public static IApplicationBuilder UseBasicSecurityHeaders(this IApplicationBuilder app)
    {
        return app.Use(async (context, next) =>
        {
            context.Response.Headers.TryAdd("X-Content-Type-Options", "nosniff");
            context.Response.Headers.TryAdd("X-Frame-Options", "DENY");
            context.Response.Headers.TryAdd("Referrer-Policy", "no-referrer");
            context.Response.Headers.TryAdd("X-Permitted-Cross-Domain-Policies", "none");
            context.Response.Headers.TryAdd("Content-Security-Policy", "default-src 'self'; img-src 'self' data:; style-src 'self' 'unsafe-inline'; script-src 'self';");

            await next();
        });
    }
}
