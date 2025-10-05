using FluentValidation;
using System.Net;
using System.Text.Json;
using StackShare.Domain.Exceptions;

namespace StackShare.API.Middleware;

public class GlobalExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionMiddleware> _logger;

    public GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unhandled exception occurred");
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";
        
        object response;

        switch (exception)
        {
            case ValidationException validationEx:
                _logger.LogWarning("Validation failed for request {RequestPath}: {ValidationErrors}", 
                    context.Request.Path, string.Join(", ", validationEx.Errors.Select(e => e.ErrorMessage)));
                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                var errors = validationEx.Errors.ToDictionary(
                    error => error.PropertyName,
                    error => error.ErrorMessage);
                response = new { message = "Validation failed", errors };
                break;

            case UnauthorizedAccessException:
                _logger.LogWarning("Unauthorized access attempt for request {RequestPath}", context.Request.Path);
                context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                response = new { message = exception.Message };
                break;

            case TechnologyAlreadyExistsException:
                _logger.LogWarning("Technology already exists: {Message}", exception.Message);
                context.Response.StatusCode = (int)HttpStatusCode.Conflict;
                response = new { message = exception.Message };
                break;

            case NotFoundException:
                _logger.LogWarning("Resource not found for request {RequestPath}: {Message}", context.Request.Path, exception.Message);
                context.Response.StatusCode = (int)HttpStatusCode.NotFound;
                response = new { message = exception.Message };
                break;

            case InvalidOperationException:
                _logger.LogWarning("Invalid operation for request {RequestPath}: {Message}", context.Request.Path, exception.Message);
                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                response = new { message = exception.Message };
                break;

            default:
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                response = new { message = "An internal server error occurred" };
                break;
        }

        var jsonResponse = JsonSerializer.Serialize(response, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        await context.Response.WriteAsync(jsonResponse);
    }
}