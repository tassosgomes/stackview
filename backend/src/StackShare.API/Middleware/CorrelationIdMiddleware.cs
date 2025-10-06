using System.Diagnostics;
using Serilog.Context;

namespace StackShare.API.Middleware;

/// <summary>
/// Middleware para adicionar correlation ID aos requests e responses
/// </summary>
public class CorrelationIdMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<CorrelationIdMiddleware> _logger;
    private const string CorrelationIdHeaderName = "X-Correlation-ID";

    public CorrelationIdMiddleware(RequestDelegate next, ILogger<CorrelationIdMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var correlationId = GetOrCreateCorrelationId(context);
        
        // Add correlation ID to response headers
        context.Response.Headers[CorrelationIdHeaderName] = correlationId;
        
        // Add correlation ID to current activity (OpenTelemetry)
        Activity.Current?.SetTag("correlation_id", correlationId);
        
        // Add correlation ID to Serilog context
        using (LogContext.PushProperty("CorrelationId", correlationId))
        {
            _logger.LogInformation("Processing request {Method} {Path} with correlation ID {CorrelationId}", 
                context.Request.Method, context.Request.Path, correlationId);
            
            await _next(context);
            
            _logger.LogInformation("Completed request {Method} {Path} with status {StatusCode} and correlation ID {CorrelationId}", 
                context.Request.Method, context.Request.Path, context.Response.StatusCode, correlationId);
        }
    }

    private string GetOrCreateCorrelationId(HttpContext context)
    {
        // Try to get correlation ID from request header first
        if (context.Request.Headers.TryGetValue(CorrelationIdHeaderName, out var correlationIdFromHeader) &&
            !string.IsNullOrEmpty(correlationIdFromHeader.FirstOrDefault()))
        {
            return correlationIdFromHeader.First()!;
        }

        // If not present, use TraceIdentifier or create new GUID
        return !string.IsNullOrEmpty(context.TraceIdentifier) ? context.TraceIdentifier : Guid.NewGuid().ToString();
    }
}