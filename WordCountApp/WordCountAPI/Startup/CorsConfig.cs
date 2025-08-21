using WordCountAPI.Config;

namespace WordCountAPI.Startup;

public static class CorsConfig
{
    public static void AddCors(this WebApplicationBuilder builder)
    {
        builder.Services.AddCors(options =>
        {
            options.AddDefaultPolicy(policy =>
            {
                var appSettings = builder.Configuration.GetSection(Constants.Config.AppSettingsSectionKey).Get<AppSettings>();
                policy.WithOrigins(appSettings.AccessControlAllowOrigin.ToArray())
                    .SetIsOriginAllowedToAllowWildcardSubdomains()
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .AllowCredentials();
            });
        });
    }
    
    public static void UseCorsConfig(this WebApplication app)
    {
        app.UseRouting();
        app.UseCors();
    }
}