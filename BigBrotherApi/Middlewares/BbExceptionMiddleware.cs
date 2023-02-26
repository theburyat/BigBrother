using System.Net;
using Entities.Exceptions;

namespace BigBrother.Middlewares;

public class BbExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<BbExceptionMiddleware> _logger;

    public BbExceptionMiddleware(RequestDelegate next, ILogger<BbExceptionMiddleware> logger)
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
        catch (UnauthorizedAccessException)
        {
            context.Response.StatusCode = (int) HttpStatusCode.Unauthorized;
        }
        catch (BbException ex)
        {
            _logger.LogError(ex.Message);
            _logger.LogDebug(ex.StackTrace);

            if (ex.InnerException is not null)
            {
                var inner = ex.InnerException;
                _logger.LogDebug($"Inner exception: {inner.Message}");
                _logger.LogDebug($"Inner exception stacktrace: {inner.StackTrace}");
            }

            if (!context.Response.HasStarted)
            {
                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                context.Response.ContentType = "application/json";
            }
            
            await context.Response.WriteAsJsonAsync(ex.ErrorCode.ToString());
        }
        catch (Exception ex)
        {
            _logger.LogError($"Unhandled exception: {ex.Message}");
            _logger.LogDebug(ex.StackTrace);
        }
    }
}
