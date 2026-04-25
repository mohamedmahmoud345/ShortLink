
using MediatR;

namespace ShortLink.Application.Features.Account.Login;

public class LoginCommand : IRequest<AuthResponse?>
{
    public LoginCommand(string email, string password)
    {
        Email = email;
        Password = password;
    }

    public string Email { get; set; }
    public string Password { get; set; }
}
