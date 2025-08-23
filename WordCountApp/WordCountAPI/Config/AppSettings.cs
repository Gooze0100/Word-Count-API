namespace WordCountAPI.Config;

public class AppSettings
{
    public List<string> AccessControlAllowOrigin { get; set; }
    public string FileStorage { get; set; }
    public string[] AllowedFileExtensions{ get; set; }
    public string Token { get; set; }
}