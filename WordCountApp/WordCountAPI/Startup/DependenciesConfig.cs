using WordCountAPI.Services;

namespace WordCountAPI.Startup;

public static class DependenciesConfig
{
    public static void AddDependencies(this WebApplicationBuilder builder)
    {
        builder.Services
            .AddScoped<IWordCountService, WordCountService>()
            ;
    }
}