using System.Net;
using System.Text.Json;
using System.Web;
using BigBrother.Domain.Entities.Exceptions;
using BigBrother.WebApp.Dtos;

namespace BigBrother.Middlewares;

public class ExceptionMiddleware
{
    private readonly RequestDelegate _next;

    public ExceptionMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (BadRequestException ex)
        {
            Console.WriteLine(ex.Message);
            
            var error = new ErrorDto
            {
                Code = ex.Code,
                Message = ex.Message
            };
            
            context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
            context.Response.ContentType = "application/json";
            
            await JsonSerializer.SerializeAsync(context.Response.Body, error);
        }
    }
}

public static class ExceptionMiddlewareExtensions
{
    public static IApplicationBuilder UseExceptionMiddleware(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<ExceptionMiddleware>();
    }
}