namespace Shared;

public static class Constants
{
    public struct Config
    {
        public const string AppSettingsSectionKey = "AppSettings";
        public const string JwtSectionKey = "AppSettings.JwtSettings";
    }
    
    public struct HeaderKeys
    {
        public const string CookieName = "XSRF-TOKEN";
        public const string HeaderName = "X-XSRF-TOKEN";
    }
}