using WordCountAPI.Middlewares;
using WordCountAPI.Services;

namespace WordCountAPI.Startup;

public static class DependenciesConfig
{
    public static void AddDependencies(this WebApplicationBuilder builder)
    {
        builder.Services
            .AddScoped<RequestHeaderMiddleware>()
            .AddScoped<AntiforgeryMiddleware>()
            .AddScoped<RateLimitingMiddleware>()
            .AddScoped<IWordCountService, WordCountService>()
            ;
    }
}