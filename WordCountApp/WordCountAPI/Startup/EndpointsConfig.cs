using WordCountAPI.Endpoints;

namespace WordCountAPI.Startup;

public static class EndpointsConfig
{
    public static void UseEndpointsConfig(this WebApplication app)
    {
        app.MapAllHealthChecks();
        app.AddWordCountEndpoints();
    }
}