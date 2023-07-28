using System.Diagnostics;
using System.Net;
using System.Text.Json;
using Contracts;

namespace Api.Middleware;

public sealed class ExceptionHandlingMiddleware
{
    private static readonly Action<ILogger, Exception> LogError =
        LoggerMessage.Define(LogLevel.Error, new EventId(1), "Exception occurred");

    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext httpContext)
    {
        try
        {
            await _next(httpContext)
                .ConfigureAwait(false);
        }
#pragma warning disable CA1031
        catch (Exception ex)
#pragma warning restore CA1031
        {
            Debug.Assert(httpContext is not null, $"{nameof(httpContext)} != null");
            LogError(_logger, ex);
            await HandleExceptionAsync(httpContext, ex)
                .ConfigureAwait(false);
        }
    }

    private static Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

        var response = JsonSerializer.Serialize(new ErrorResponse(exception.Message));

        return context.Response.WriteAsync(response);
    }
}