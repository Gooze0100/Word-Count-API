using Microsoft.AspNetCore.Http.Features;
using WordCountAPI.Middlewares;
using WordCountAPI.Services;

namespace WordCountAPI.Startup;

public static class DependenciesConfig
{
    public static void AddDependencies(this WebApplicationBuilder builder)
    {
        builder.Services.AddOpenApiServices();
        builder.Services.AddCorsServices();
        builder.Services.AddAllHealthChecks();
        
        builder.Services.AddAntiforgery(options =>
        {
            options.Cookie.Name = Constants.HeaderKeys.CookieName;
            options.FormFieldName = Constants.HeaderKeys.FormFieldName;
            options.HeaderName = Constants.HeaderKeys.HeaderName;
            options.SuppressXFrameOptionsHeader = false;
        });
        
        builder.Services.Configure<FormOptions>(options =>
        {
            options.MultipartBodyLengthLimit = 1024 * 1024 * 5;
        });

        builder.Services.AddAuthorization();
        
        builder.Services
            .AddScoped<UserMiddleware>()
            .AddScoped<IWordCountService, WordCountService>()
            ;
    }
}