using Microsoft.AspNetCore.Antiforgery;
using WordCountAPI.Services;

namespace WordCountAPI.Endpoints;

public static class ApiEndpoints
{
    public static void AddWordCountEndpoints(this WebApplication app)
    {
        var api = app.MapGroup("/api");
        
        api.MapPost("/wordcount", async (IFormFileCollection file, IWordCountService wordCountService, HttpContext ctx, IAntiforgery antiforgery) =>
        {
            // await antiforgery.ValidateRequestAsync(ctx);
            // if (!(ctx.User.Identity is { IsAuthenticated: true }))
            // {
            //     return Result.Failure<WordCountRes, Exception>(new Exception("Unauthorized"));
            // }

            var result =  await wordCountService.LoadFile(file, ctx.RequestAborted);

            if (result.IsFailure)
            {
                return Results.Problem(title: result.Error.Message, statusCode:400);
            }

            return Results.Ok(result.Value);
        })
        .DisableAntiforgery()
        .WithName("UploadFile");
    }
}