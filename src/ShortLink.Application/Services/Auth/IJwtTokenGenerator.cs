
namespace ShortLink.Application.Services.Auth;

public record TokenRequestDto(Guid UserId, string UserName, string Email);

public interface IJwtTokenGenerator
{
    string GenerateToken(TokenRequestDto tokenRequest);
}
