using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using ShortLink.Application.Services.Auth;


namespace ShortLink.Infrastructure.Services.Auth;

public class JwtTokenGenerator : IJwtTokenGenerator
{
    private readonly IConfiguration _confg;
    public JwtTokenGenerator(IConfiguration configuration)
    {
        _confg = configuration;
    }
    public string GenerateToken(TokenRequestDto tokenRequest)
    {
        var jwtSettings = _confg.GetSection("Jwt");
        var secretKey = _confg["Jwt:SecretKey"];
        var issuer = jwtSettings["Issuer"];
        var audience = jwtSettings["Audience"];
        var expiryMinutes = int.Parse(jwtSettings["ExpiryMinutes"]!);

        var claims = new List<Claim>()
        {
            new Claim(ClaimTypes.NameIdentifier, tokenRequest.UserId.ToString()),
            new Claim(ClaimTypes.Name, tokenRequest.UserName),
            new Claim(ClaimTypes.Email, tokenRequest.Email)
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey!));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(expiryMinutes),
            signingCredentials: credentials
        );
        
        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
