namespace WordCountAPI.Endpoints;

public static class RootEndpoints
{
    public static void AddRootEndpoints(this WebApplication app)
    {
        app.MapGet("/", () => Results.Redirect("/scalar/v1"));
    }
}