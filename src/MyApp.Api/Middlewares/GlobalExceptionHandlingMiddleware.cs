using System.Net;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using MyApp.Application.Validations;
using MyApp.Domain.Errors;

namespace MyApp.Api.Middlewares;

public sealed class GlobalExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionHandlingMiddleware> _logger;

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = false
    };

    public GlobalExceptionHandlingMiddleware(
        RequestDelegate next,
        ILogger<GlobalExceptionHandlingMiddleware> logger)
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
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        _logger.LogError(exception, "Excepción capturada en Middleware: {Message}", exception.Message);

        (HttpStatusCode statusCode, ProblemDetails problemDetails) = exception switch
        {
            NotFoundException notFoundEx => (
                HttpStatusCode.NotFound,
                new ProblemDetails
                {
                    Status = (int)HttpStatusCode.NotFound,
                    Title = "Recurso no encontrado",
                    Type = "https://tools.ietf.org/html/rfc7231#section-6.5.4",
                    Detail = notFoundEx.Message,
                    Instance = context.Request.Path
                }),

            ApplicationValidationException validationEx => (
                HttpStatusCode.BadRequest,
                new ValidationProblemDetails(
                    validationEx.Errors.ToDictionary(
                        k => k.Key,
                        v => v.Value))
                {
                    Status = (int)HttpStatusCode.BadRequest,
                    Title = "Error de validación",
                    Type = "https://tools.ietf.org/html/rfc7231#section-6.5.1",
                    Detail = validationEx.Message,
                    Instance = context.Request.Path
                }),

            BadRequestException badRequestEx => (
                HttpStatusCode.BadRequest,
                new ProblemDetails
                {
                    Status = (int)HttpStatusCode.BadRequest,
                    Title = "Solicitud inválida",
                    Type = "https://tools.ietf.org/html/rfc7231#section-6.5.1",
                    Detail = badRequestEx.Message,
                    Instance = context.Request.Path
                }),

            _ => (
                HttpStatusCode.InternalServerError,
                new ProblemDetails
                {
                    Status = (int)HttpStatusCode.InternalServerError,
                    Title = "Error interno del servidor",
                    Type = "https://tools.ietf.org/html/rfc7231#section-6.6.1",
                    Detail = "Ocurrió un error inesperado al procesar la solicitud.",
                    Instance = context.Request.Path
                })
        };

        context.Response.ContentType = "application/problem+json";
        context.Response.StatusCode = (int)statusCode;

        await context.Response.WriteAsync(JsonSerializer.Serialize(problemDetails, JsonOptions));
    }
}
