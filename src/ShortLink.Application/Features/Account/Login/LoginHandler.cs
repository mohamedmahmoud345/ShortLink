
using MediatR;
using ShortLink.Application.Services.Auth;

namespace ShortLink.Application.Features.Account.Login;

public class LoginHandler : IRequestHandler<LoginCommand, AuthResponse?>
{
    private readonly IAuthService _authService;
    private readonly IJwtTokenGenerator _jwtToken;

    public LoginHandler(IAuthService authService, IJwtTokenGenerator jwtToken)
    {
        _authService = authService;
        _jwtToken = jwtToken;
    }

    public async Task<AuthResponse?> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var authResult = await _authService.LoginAsync(request.Email, request.Password);
        if (authResult == null || !authResult.Succeeded)
            return null;

        var tokenRequest = new TokenRequestDto(authResult.Id, authResult.Name, authResult.Email, authResult.Roles);

        var token = _jwtToken.GenerateToken(tokenRequest);

        return new AuthResponse()
        {
            UserName = authResult.Name,
            Token = token
        };
    }
}
