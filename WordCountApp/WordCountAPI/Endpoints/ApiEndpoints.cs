using Microsoft.AspNetCore.Antiforgery;
using Shared;
using WordCountAPI.Services;

namespace WordCountAPI.Endpoints;

public static class ApiEndpoints
{
    public static void AddWordCountEndpoints(this WebApplication app)
    {
        var api = app.MapGroup("/api");
        
        api.MapPost("/wordcount", async (IFormFileCollection file, IWordCountService wordCountService, HttpContext ctx) =>
        {
            var result =  await wordCountService.LoadFile(file, ctx.RequestAborted);

            if (result.IsFailure)
            {
                return Results.Problem(title: result.Error.Message, statusCode:400);
            }

            return Results.Ok(result.Value);
        })
        .RequireAuthorization()
        .WithName("UploadFile");
        
        api.MapGet("/antiforgery/token", (HttpContext ctx, IAntiforgery antiforgery) =>
        {
            var tokens = antiforgery.GetAndStoreTokens(ctx);
            ctx.Response.Cookies.Append(Constants.HeaderKeys.CookieName, tokens.RequestToken!, new CookieOptions
            {
                HttpOnly = false,
                SameSite = SameSiteMode.Strict
            });
            
            return Results.Ok();
        });
    }
}