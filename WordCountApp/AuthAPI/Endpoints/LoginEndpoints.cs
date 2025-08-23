using Microsoft.AspNetCore.Identity.Data;

namespace AuthAPI.Endpoints;

public static class LoginEndpoints
{
    public static void AddLoginEndpoints(this WebApplication app)
    {
        app.MapPost("/login", (LoginRequest req, TokenGenerator tokenGenerator) =>
        new
        {
            access_token = tokenGenerator.GenerateToken(req.Email)
        });
    }
}