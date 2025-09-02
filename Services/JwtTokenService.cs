using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using AGAMinigameApi.Helpers;
using Microsoft.IdentityModel.Tokens;

namespace SlotsApi.Services;

public interface IJwtTokenService
{
    string GenerateAccessToken(string memberId);
    string GenerateRefreshToken();
}
public class JwtTokenService : IJwtTokenService
{
    private readonly string _secret;
    private readonly string _issuer;
    private readonly int _expiryMinutes;

    public JwtTokenService(IConfiguration config)
    {
        _secret = config["JwtSettings:Secret"] ?? throw new Exception("JWT secret missing in config.");
        _issuer = config["JwtSettings:Issuer"] ?? throw new Exception("JWT issuer missing in config.");
        _expiryMinutes = int.Parse(config["JwtSettings:AccessTokenExpiryMinutes"] ?? "30");
    }

    public string GenerateAccessToken(string memberId)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.UTF8.GetBytes(_secret);
        var now = DateHelper.GetUtcNow();
        var expiresAt = DateHelper.GetUtcNow().AddMinutes(_expiryMinutes); // requires utc

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new List<Claim> {
                new Claim(JwtRegisteredClaimNames.Sub, memberId),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString("N")), // unique ID
                new Claim(JwtRegisteredClaimNames.Iat, new DateTimeOffset(now).ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64) // timestamp
            }),
            Expires = expiresAt,
            Issuer = _issuer,
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(key),
                SecurityAlgorithms.HmacSha256Signature
            )
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }

    public string GenerateRefreshToken()
    {
        return Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
    }
}
