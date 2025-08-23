using AuthAPI.Endpoints;

namespace AuthAPI.Startup;

public static class EndpointsConfig
{
    public static void UseEndpointsConfig(this WebApplication app)
    {
        app.AddLoginEndpoints();
    }
}