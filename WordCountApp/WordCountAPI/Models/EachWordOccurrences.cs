namespace WordCountAPI.Models;

public struct EachWordOccurrences
{
    public int Id { get; set; }
    public string FileName { get; set; }
    public Dictionary<string, int> WordOccurrences { get; set; }
}