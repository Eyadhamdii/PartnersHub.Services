using System.Net;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;

namespace PartnersHub.InfraBase.Apis.Common;

/// <summary>
/// Global exception handling middleware
/// </summary>
public class GlobalExceptionMiddleware {
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionMiddleware> _logger;
    private readonly IHostEnvironment _environment;

    public GlobalExceptionMiddleware(
        RequestDelegate next,
        ILogger<GlobalExceptionMiddleware> logger,
        IHostEnvironment environment) {
        _next = next;
        _logger = logger;
        _environment = environment;
    }

    public async Task InvokeAsync(HttpContext context) {
        try {
            await _next(context);
        } catch (Exception ex) {
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception) {
        _logger.LogError(exception, "Unhandled exception occurred: {Message} | Path: {Path}",
            exception.Message, context.Request.Path);

        context.Response.ContentType = "application/json";

        var (statusCode, message, shouldExposeDetails) = GetExceptionDetails(exception);
        context.Response.StatusCode = statusCode;

        // Build error object
        var errorDetails = new {
            Message = message,
            Details = shouldExposeDetails || _environment.IsDevelopment() ? exception.Message : null,
            StackTrace = _environment.IsDevelopment() ? exception.StackTrace : null,
            InnerException = _environment.IsDevelopment() && exception.InnerException != null
                ? exception.InnerException.Message
                : null
        };

        var response = ApiResponse.Failure(errorDetails, statusCode);

        var options = new JsonSerializerOptions {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
        };

        var json = JsonSerializer.Serialize(response, options);
        await context.Response.WriteAsync(json);
    }

    private (int StatusCode, string Message, bool ExposeDetails) GetExceptionDetails(Exception exception) {
        return exception switch {
            // Business rule violations
            InvalidOperationException =>
                (StatusCodes.Status400BadRequest, "Business rule violation", true),

            // Validation errors
            ArgumentException or ArgumentNullException =>
                (StatusCodes.Status400BadRequest, "Invalid request data", true),

            // Not found
            KeyNotFoundException =>
                (StatusCodes.Status404NotFound, "Resource not found", true),

            // Unauthorized
            UnauthorizedAccessException =>
                (StatusCodes.Status401Unauthorized, "Unauthorized access", false),

            // Database concurrency
            DbUpdateConcurrencyException =>
                (StatusCodes.Status409Conflict, "Concurrency conflict - resource was modified", false),

            // Database general errors
            DbUpdateException =>
                (StatusCodes.Status400BadRequest, "Database update failed", false),

            // Generic server error
            _ => (StatusCodes.Status500InternalServerError, "An error occurred while processing your request", false)
        };
    }
}