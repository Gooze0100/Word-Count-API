using WordCountAPI.Models.WordCount;
using WordCountAPI.Services;

namespace WordCountAPI.Endpoints;

public static class ApiEndpoints
{
    public static void AddWordCountEndpoints(this WebApplication app)
    {
        var api = app.MapGroup("/api");
        
        api.MapPost("/wordcount", async (IWordCountService wordCountService, HttpContext ctx) =>
        {
            var form = await ctx.Request.ReadFormAsync();

            var req = new WordCountReq()
            {
                FileChunk = form.Files["filechunk"],
                FileName = form["fileName"].ToString(),
                UploadId = new Guid(form["uploadId"].ToString()),
                ChunkIndex = int.Parse(form["chunkIndex"].ToString()),
                TotalChunks = int.Parse(form["totalChunks"].ToString())
            };

            return await wordCountService.UploadFileChunk(req, ctx.RequestAborted);
        });
    }
}