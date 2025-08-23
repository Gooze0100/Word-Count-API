using Shared;

namespace WordCountAPI.Middlewares;

public class RequestHeaderMiddleware : IMiddleware
{
    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        context.Request.Headers[Constants.HeaderKeys.HeaderName] = context.Request.Cookies[Constants.HeaderKeys.CookieName];

        await next(context);
    }
}