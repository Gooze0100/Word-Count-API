namespace AuthAPI.Startup;

public static class DependenciesConfig
{
    public static void AddDependencies(this WebApplicationBuilder builder)
    {
        builder.Services
            .AddSingleton<TokenGenerator>();
    }
}