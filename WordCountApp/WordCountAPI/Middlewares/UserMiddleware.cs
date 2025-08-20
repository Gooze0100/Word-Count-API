using Microsoft.AspNetCore.Antiforgery;

namespace WordCountAPI.Middlewares;

public class UserMiddleware :  IMiddleware
{
    private readonly IAntiforgery _antiforgery;
    private readonly ILogger<UserMiddleware> _logger;
    
    public UserMiddleware(IAntiforgery antiforgery, ILogger<UserMiddleware> logger)
    {
        _antiforgery = antiforgery;
        _logger = logger;
    }
    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        if (HttpMethods.IsPost(context.Request.Method) ||
            HttpMethods.IsPut(context.Request.Method) ||
            HttpMethods.IsDelete(context.Request.Method) ||
            HttpMethods.IsPatch(context.Request.Method))
        {
            try
            {
                await _antiforgery.ValidateRequestAsync(context);
            }
            catch (AntiforgeryValidationException ex)
            {
                _logger.LogWarning(ex, "Anti-forgery token validation failed.");
                context.Response.StatusCode = StatusCodes.Status400BadRequest;
                await context.Response.WriteAsync("Invalid anti-forgery token.");
                return;
            }
        }

        await next(context);
    }
}