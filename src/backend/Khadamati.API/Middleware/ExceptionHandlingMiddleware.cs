using System.Net;
using System.Text.Json;
using FluentValidation;
using Khadamati.Application.Common;

namespace Khadamati.API.Middleware;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
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
            _logger.LogError(ex, "An unhandled exception occurred: {Message}", ex.Message);
            await HandleExceptionAsync(context, ex);
        }
    }

    private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var (statusCode, message, errors) = exception switch
        {
            Khadamati.Application.Common.ValidationException ve => (HttpStatusCode.BadRequest, exception.Message, ve.Errors),
            NotFoundException => (HttpStatusCode.NotFound, exception.Message, null),
            UnauthorizedException => (HttpStatusCode.Unauthorized, exception.Message, null),
            ForbiddenException => (HttpStatusCode.Forbidden, exception.Message, null),
            ConflictException => (HttpStatusCode.Conflict, exception.Message, null),
            InvalidOperationException => (HttpStatusCode.Conflict, exception.Message, null),
            _ => (HttpStatusCode.InternalServerError, "An internal server error occurred.", null)
        };

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)statusCode;

        var response = ApiResponse<object>.Fail(message, errors);
        await context.Response.WriteAsync(JsonSerializer.Serialize(response, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        }));
    }
}
