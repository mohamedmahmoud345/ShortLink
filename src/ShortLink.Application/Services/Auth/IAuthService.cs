using ShortLink.Application.DTOs.Account;

namespace ShortLink.Application.Services.Auth;

public interface IAuthService
{
    Task<AuthDto?> RegisterUserAsync(string userName, string email, string password);
    Task<AuthDto?> LoginAsync(string email, string password);
}
