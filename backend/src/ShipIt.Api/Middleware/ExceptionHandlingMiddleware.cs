using Microsoft.AspNetCore.Mvc;

namespace ShipIt.Api.Middleware;

public sealed class ExceptionHandlingMiddleware(
    RequestDelegate next,
    ILogger<ExceptionHandlingMiddleware> logger)
{
    private const string ProblemDetailsContentType = "application/problem+json";

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (OperationCanceledException) when (context.RequestAborted.IsCancellationRequested)
        {
            logger.LogInformation(
                "Request {Method} {Path} was aborted by the client",
                context.Request.Method,
                context.Request.Path);
        }
        catch (Exception exception)
        {
            await HandleUnexpectedExceptionAsync(context, exception);
        }
    }

    private async Task HandleUnexpectedExceptionAsync(HttpContext context, Exception exception)
    {
        logger.LogError(
            exception,
            "Unexpected exception while processing {Method} {Path}",
            context.Request.Method,
            context.Request.Path);

        if (context.Response.HasStarted)
        {
            logger.LogWarning("The response has already started; cannot write problem details");
            return;
        }

        context.Response.Clear();
        context.Response.StatusCode = StatusCodes.Status500InternalServerError;
        context.Response.ContentType = ProblemDetailsContentType;

        var problemDetails = new ProblemDetails
        {
            Status = StatusCodes.Status500InternalServerError,
            Title = "Unexpected server error",
            Detail = "An unexpected error occurred while processing the request."
        };

        await context.Response.WriteAsJsonAsync(problemDetails);
    }
}

public static class ExceptionHandlingMiddlewareExtensions
{
    public static IApplicationBuilder UseGlobalExceptionHandling(this IApplicationBuilder app)
    {
        return app.UseMiddleware<ExceptionHandlingMiddleware>();
    }
}
