//Module 4 exercise 1B

using System.Diagnostics;

namespace TmsApi.Api.Middlewares;
public class RequestLoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<RequestLoggingMiddleware> _logger;

    public RequestLoggingMiddleware(
        RequestDelegate next,
        ILogger<RequestLoggingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // Generate a short correlation ID
        var correlationId = Guid.NewGuid().ToString("N")[..8];

        // Add the header before calling the next middleware
        context.Response.Headers["X-Correlation-Id"] = correlationId;

        // Start the timer
        var stopwatch = Stopwatch.StartNew();

        // Log request entry
        _logger.LogInformation(
            "Request Started: {Method} {Path} CorrelationId={CorrelationId}",
            context.Request.Method,
            context.Request.Path,
            correlationId);

        // Continue to the next middleware
        await _next(context);

        // Stop the timer
        stopwatch.Stop();

        // Log request exit
        _logger.LogInformation(
            "Request Finished: StatusCode={StatusCode} Elapsed={Elapsed}ms CorrelationId={CorrelationId}",
            context.Response.StatusCode,
            stopwatch.ElapsedMilliseconds,
            correlationId);
    }
}