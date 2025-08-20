namespace WordCountAPI.Models.WordCount;

public class WordCountReq
{
    public IFormFile FileChunk { get; set; }
    public Guid? UploadId { get; set; }
    public string FileName { get; set; }
    public int ChunkIndex { get; set; }
    public int TotalChunks { get; set; }
}