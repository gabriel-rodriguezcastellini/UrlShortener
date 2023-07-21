using Common.Exceptions;
using System.Net;

namespace WebAPI.CustomExceptionMiddleware;

public class ExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionMiddleware> _logger;

    public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
    {
        _logger = logger;
        _next = next;
    }

    public async Task InvokeAsync(HttpContext httpContext)
    {
        try
        {
            await _next(httpContext);
        }
        catch (UnableToDeleteUrlException unableToDeleteUrlException)
        {
            _logger.LogError(unableToDeleteUrlException, "It wasn't possible to delete the URL.");
            await HandleExceptionAsync(httpContext, unableToDeleteUrlException, HttpStatusCode.BadRequest);
        }
        catch (ShortUrlExistsException shortUrlExistsException)
        {
            _logger.LogError(shortUrlExistsException, $"Short URL already exists.");
            await HandleExceptionAsync(httpContext, shortUrlExistsException, HttpStatusCode.BadRequest);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Something went wrong: {ex}");
            await HandleExceptionAsync(httpContext, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception, HttpStatusCode httpStatusCode = HttpStatusCode.InternalServerError)
    {
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)httpStatusCode;
        var message = exception switch
        {
            UnableToDeleteUrlException => "It wasn't possible to delete the URL.",
            ShortUrlExistsException => "Short URL already exists.",
            _ => "Internal Server Error."
        };
        await context.Response.WriteAsync(new ErrorDetails()
        {
            StatusCode = context.Response.StatusCode,
            Message = message
        }.ToString());
    }
}
