using System.Net;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using RetroGaming.Domain.Exceptions;

namespace RetroGaming.Application.Middleware;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(RequestDelegate _next, ILogger<ExceptionHandlingMiddleware> logger)
    {
        this._next = _next;
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
        var statusCode = exception switch
        {
            EntityNotFoundException => (int)HttpStatusCode.NotFound,
            DomainValidationException => (int)HttpStatusCode.BadRequest,
            InvalidDomainOperationException => (int)HttpStatusCode.Conflict,
            ArgumentException => (int)HttpStatusCode.BadRequest,
            _ => (int)HttpStatusCode.InternalServerError
        };

        var title = exception switch
        {
            EntityNotFoundException => "Resource Not Found",
            DomainValidationException => "Validation Error",
            InvalidDomainOperationException => "Domain Operation Conflict",
            ArgumentException => "Invalid Argument",
            _ => "An unexpected error occurred"
        };

        var problemDetails = new ProblemDetails
        {
            Status = statusCode,
            Title = title,
            Detail = exception.Message,
            Instance = context.Request.Path
        };

        context.Response.ContentType = "application/problem+json";
        context.Response.StatusCode = statusCode;

        var json = JsonSerializer.Serialize(problemDetails);
        await context.Response.WriteAsync(json);
    }
}
