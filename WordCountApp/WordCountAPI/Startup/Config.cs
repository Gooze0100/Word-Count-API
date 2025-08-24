using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.IdentityModel.Tokens;
using Scalar.AspNetCore;
using Shared;
using WordCountAPI.Config;
using WordCountAPI.Middlewares;

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
    
    public static void AddOpenApiServices(this IServiceCollection services)
    {
        services.AddOpenApi(options =>
        {
            options.AddDocumentTransformer<BearerSecuritySchemeTransformer>();
        });
    }

    public static void UseOpenApi(this WebApplication app)
    {
        if (app.Environment.IsDevelopment())
        {
            var appSettings = app.Configuration.GetSection("AppSettings").Get<AppSettings>();
            app.MapOpenApi();
            app.MapScalarApiReference(options =>
            {
                options.Title = "Word Count API";
                options.Theme = ScalarTheme.BluePlanet;
                options.Layout = ScalarLayout.Modern;
                options.HideClientButton = true;

                if (!string.IsNullOrWhiteSpace(appSettings.Token))
                {
                    options.AddPreferredSecuritySchemes("Bearer")
                        .AddHttpAuthentication("Bearer", auth =>
                        {
                            auth.WithToken(appSettings.Token);
                        });
                }
            });
        }
    }
    
    public static void AddAuth(this WebApplicationBuilder builder)
    {
        builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(a =>
        {
            a.TokenValidationParameters = new TokenValidationParameters
            {
                IssuerSigningKey = new SymmetricSecurityKey("GreatNowYouJustNeedToStoreAndLoadThisSecurely"u8.ToArray()),
                ValidIssuer = "https://id.localhost:5081",
                ValidAudience = "https://localhost:5081",
                ValidateIssuerSigningKey = true,
                ValidateLifetime = true,
                ValidateIssuer = true,
                ValidateAudience = true
            };
        });
        
        builder.Services.AddAuthorization();
        
        builder.Services.AddAntiforgery(options => options.HeaderName = Constants.HeaderKeys.HeaderName);
    }
    
    public static void UseAuth(this WebApplication app)
    {
        app.UseAuthentication();
        app.UseAuthorization();

        // Add header "X-XSRF-TOKEN" from cookies to requests for development and testing purpose
        if (app.Environment.IsDevelopment() || app.Environment.IsEnvironment("Testing"))
        {
            app.UseMiddleware<RequestHeaderMiddleware>();
        }
        
        app.UseAntiforgery();
        
        app.UseMiddleware<AntiforgeryMiddleware>();

        var antiforgery = app.Services.GetRequiredService<IAntiforgery>();
        app.Use(async (context, next) =>
        {
            if (HttpMethods.IsGet(context.Request.Method) && 
                (context.Request.Path == "/api/antiforgery/token" || context.Request.Path.StartsWithSegments("/index.html")))
            {
                var tokens = antiforgery.GetAndStoreTokens(context);
                context.Response.Cookies.Append(Constants.HeaderKeys.CookieName, tokens.RequestToken!,
                    new CookieOptions { HttpOnly = false, SameSite = SameSiteMode.Strict });
            }
            await next();
        });

        app.UseMiddleware<RateLimitingMiddleware>();
    }
    
    public static void AddOther(this WebApplicationBuilder builder)
    {
        builder.Services.Configure<FormOptions>(options =>
        {
            options.MultipartBodyLengthLimit = 1024 * 1024 * 50; // 50MB
            options.ValueLengthLimit = 1024 * 1024 * 10;
        });
        
        // Use to endpoints where data is not changing often
        builder.Services
            .AddMemoryCache()
            .AddOutputCache(options =>
            {
                options.DefaultExpirationTimeSpan = TimeSpan.FromHours(2);
            });
    }
}