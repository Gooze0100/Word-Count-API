using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using AuthAPI.Config;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using JwtRegisteredClaimNames = Microsoft.IdentityModel.JsonWebTokens.JwtRegisteredClaimNames;

namespace AuthAPI;

public class TokenGenerator
{
    private readonly IOptions<AppSettings> _settings;
    public TokenGenerator(IOptions<AppSettings> settings)
    {
        _settings = settings;
    }
    public string GenerateToken(string email)
    {
        JwtSecurityTokenHandler tokenHandler = new();
        var key = System.Text.Encoding.UTF8.GetBytes(_settings.Value.JwtSettings.Key);

        List<Claim> claims =
        [
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new(JwtRegisteredClaimNames.Sub, email),
            new(JwtRegisteredClaimNames.Email, email)
        ];

        SecurityTokenDescriptor tokenDescriptor = new()
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddHours(8),
            Issuer = _settings.Value.JwtSettings.Issuer,
            Audience = _settings.Value.JwtSettings.Audience,
            SigningCredentials =
                new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };
        
        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }
}