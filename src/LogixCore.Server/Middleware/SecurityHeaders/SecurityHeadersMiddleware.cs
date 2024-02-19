namespace LogixCore.Server.Middleware.SecurityHeader;

public class SecurityHeadersMiddleware
{
    private readonly RequestDelegate next;

    public SecurityHeadersMiddleware(RequestDelegate next)
    {
        this.next = next;
    }

    public Task Invoke(HttpContext httpContext)
    {
        // X-Frame-Options
        if (!httpContext.Response.Headers.ContainsKey("X-Frame-Options"))
        {
            httpContext.Response.Headers.Append("X-Frame-Options", "DENY");
        }

        // X-Xss-Protection
        if (!httpContext.Response.Headers.ContainsKey("X-XSS-Protection"))
        {
            httpContext.Response.Headers.Append("X-XSS-Protection", "1; mode=block");
        }

        // X-Content-Type-Options
        if (!httpContext.Response.Headers.ContainsKey("X-Content-Type-Options"))
        {
            httpContext.Response.Headers.Append("X-Content-Type-Options", "nosniff");
        }

        // Referrer-Policy
        if (!httpContext.Response.Headers.ContainsKey("Referrer-Policy"))
        {
            httpContext.Response.Headers.Append("Referrer-Policy", "no-referrer");
        }

        // X-Permitted-Cross-Domain-Policies
        if (!httpContext.Response.Headers.ContainsKey("X-Permitted-Cross-Domain-Policies"))
        {
            httpContext.Response.Headers.Append("X-Permitted-Cross-Domain-Policies", "none");
        }

        // Permissions-Policy
        if (!httpContext.Response.Headers.ContainsKey("Permissions-Policy"))
        {
            httpContext.Response.Headers.Append("Permissions-Policy", "accelerometer 'none'; camera 'none'; geolocation 'none'; gyroscope 'none'; magnetometer 'none'; microphone 'none'; payment 'none'; usb 'none'");
        }

        // Content-Security-Policy
        if (!httpContext.Response.Headers.ContainsKey("Content-Security-Policy"))
        {
            httpContext.Response.Headers.Append("Content-Security-Policy", "form-action 'self'; script-src 'self' 'unsafe-inline' 'unsafe-eval' https://www.google.com https://code.jquery.com; style-src 'self' 'unsafe-inline' https://fonts.googleapis.com https://fonts.gstatic.com https://cdn.jsdelivr.net");
        }

        return this.next.Invoke(httpContext);
    }
}
