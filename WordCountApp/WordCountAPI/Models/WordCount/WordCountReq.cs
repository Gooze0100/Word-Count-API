namespace WordCountAPI.Models.WordCount;

public class WordCountReq
{
    public string UserName { get; set; } = "userName";
    public IFormFile File { get; set; }
    public string FileName { get; set; }
    public Guid UploadId { get; set; }
    public int ChunkIndex { get; set; }
    public int TotalChunks { get; set; }
}