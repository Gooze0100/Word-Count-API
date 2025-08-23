using AuthAPI.Config;
using Scalar.AspNetCore;
using Shared;

namespace AuthAPI.Startup;

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
            .Validate(config => !string.IsNullOrWhiteSpace(config.JwtSettings.Key), "No key assigned")
            .Validate(config => !string.IsNullOrWhiteSpace(config.JwtSettings.Issuer), "No issuer assigned")
            .Validate(config => !string.IsNullOrWhiteSpace(config.JwtSettings.Audience), "No audience assigned")
            .ValidateOnStart();
    }
    
    public static void UseOpenApi(this WebApplication app)
    {
        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();
            app.MapScalarApiReference(options =>
            {
                options.Title = "Auth API";
                options.Theme = ScalarTheme.BluePlanet;
                options.Layout = ScalarLayout.Modern;
                options.HideClientButton = true;
            });
        }
    }
}