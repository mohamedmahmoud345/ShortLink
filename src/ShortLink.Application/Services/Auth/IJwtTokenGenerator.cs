
using System.Security.Claims;

namespace ShortLink.Application.Services.Auth;

public record TokenRequestDto(Guid UserId, string UserName, string Email,List<string> Roles);

public interface IJwtTokenGenerator
{
    string GenerateToken(TokenRequestDto tokenRequest);
}
