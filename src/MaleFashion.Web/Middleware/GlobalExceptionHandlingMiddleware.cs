using FluentValidation;
using MaleFashion.Application.Common.Exceptions;

namespace MaleFashion.Web.Middleware;

public class GlobalExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionHandlingMiddleware> _logger;

    public GlobalExceptionHandlingMiddleware(RequestDelegate next, ILogger<GlobalExceptionHandlingMiddleware> logger)
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
        catch (NotFoundException ex)
        {
            _logger.LogWarning(ex, "Resource not found for {Path}", context.Request.Path);
            await WriteResponseAsync(context, StatusCodes.Status404NotFound, "The requested resource was not found.");
        }
        catch (ConflictException ex)
        {
            _logger.LogWarning(ex, "Conflict for {Path}: {Message}", context.Request.Path, ex.Message);
            await WriteResponseAsync(context, StatusCodes.Status409Conflict, ex.Message);
        }
        catch (PaymentFailedException ex)
        {
            _logger.LogWarning("Payment failed for {Path}: {Reason}", context.Request.Path, ex.Message);
            await WriteResponseAsync(context, StatusCodes.Status402PaymentRequired, ex.Message);
        }
        catch (ValidationException ex)
        {
            _logger.LogWarning("Validation failed for {Path}: {Errors}",
                context.Request.Path, string.Join("; ", ex.Errors.Select(e => e.ErrorMessage)));
            await WriteResponseAsync(context, StatusCodes.Status400BadRequest, "Invalid request data.");
        }
        catch (Exception ex)
        {
            // Unexpected/unhandled exception — log full server-side detail via
            // Serilog, but never reflect ex.Message or stack trace back to the
            // client in the response body. Serilog's own sinks (console/file)
            // are configured below to never receive password/card/token/secret
            // values, since none of this application's exceptions carry them.
            _logger.LogError(ex, "Unhandled exception for {Path}", context.Request.Path);
            await WriteResponseAsync(context, StatusCodes.Status500InternalServerError,
                "An unexpected error occurred. Please try again later.");
        }
    }

    private static async Task WriteResponseAsync(HttpContext context, int statusCode, string message)
    {
        if (context.Response.HasStarted)
        {
            // Response already partially written (e.g. streaming) — nothing
            // safe left to do but let it propagate; avoids a
            // "headers already sent" secondary exception.
            return;
        }

        context.Response.Clear();
        context.Response.StatusCode = statusCode;

        var acceptsHtml = context.Request.Headers.Accept.Any(h => h != null && h.Contains("text/html"));

        if (acceptsHtml)
        {
            context.Response.Redirect($"/Home/Error?statusCode={statusCode}");
        }
        else
        {
            context.Response.ContentType = "application/json";
            await context.Response.WriteAsJsonAsync(new { error = message });
        }
    }
}