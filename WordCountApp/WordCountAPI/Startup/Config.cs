using Microsoft.AspNetCore.DataProtection;
using WordCountAPI.Config;

namespace WordCountAPI.Startup;

public static class Config
{
    public static void RegisterConfiguration(this WebApplicationBuilder builder)
    {
        var separator = Path.DirectorySeparatorChar;

        builder.Configuration
            .AddJsonFile($"Config{separator}appsettings.json", optional: false, reloadOnChange: true)
            .AddJsonFile($"Config{separator}appsettings.{builder.Environment.EnvironmentName}.json", true)
            .AddEnvironmentVariables();
    }


    public static void RegisterSettings(this WebApplicationBuilder builder)
    {
        builder.Services.AddOptions();

        builder.Services.AddOptions<AppSettings>()
            .BindConfiguration(Constants.Config.AppSettingsSectionKey)
            .Validate(config => !string.IsNullOrWhiteSpace(config.FileStorage), "Invalid file storage path.")
            .ValidateOnStart();
    }
}