using Microsoft.AspNetCore.Http.Features;
using WordCountAPI.Config;

namespace WordCountAPI.Startup;

public static class Config
{
    public static void AddConfiguration(this WebApplicationBuilder builder)
    {
        var separator = Path.DirectorySeparatorChar;

        builder.Configuration
            .AddJsonFile($"Config{separator}appsettings.json", optional: false, reloadOnChange: true)
            .AddJsonFile($"Config{separator}appsettings.{builder.Environment.EnvironmentName}.json", true);
    }

    public static void AddSettings(this WebApplicationBuilder builder)
    {
        builder.Services.AddOptions();

        builder.Services.AddOptions<AppSettings>()
            .BindConfiguration(Constants.Config.AppSettingsSectionKey)
            .Validate(config => !string.IsNullOrWhiteSpace(config.FileStorage), "Invalid file storage path.")
            .Validate(config => config.AllowedFileExtensions.Length > 0, "No allowed file extensions specified.")
            .ValidateOnStart();
    }
    
    public static void AddAuth(this WebApplicationBuilder builder)
    {
        builder.Services.AddAuthentication().AddJwtBearer();
        
        builder.Services.AddAuthorization();
        
        builder.Services.AddAntiforgery(options =>
        {
            // options.Cookie.Name = Constants.HeaderKeys.CookieName;
            // options.FormFieldName = Constants.HeaderKeys.FormFieldName;
            options.HeaderName = Constants.HeaderKeys.HeaderName;
            // options.SuppressXFrameOptionsHeader = false;
        });
    }

    public static void AddOther(this WebApplicationBuilder builder)
    {
        builder.Services.Configure<FormOptions>(options =>
        {
            options.MultipartBodyLengthLimit = 1024 * 1024 * 5;
        });
        
        builder.Services
            .AddMemoryCache()
            .AddOutputCache(options =>
            {
                options.DefaultExpirationTimeSpan = TimeSpan.FromHours(2);
            });
    }
}