using MediatR;
using ShortLink.Application.Features.Account.Register;
using ShortLink.Application.Services.Auth;
using ShortLink.Domain.Interfaces.UnitOfWork;

namespace ShortLink.Application.Features.Account;

public class RegisterHandler : IRequestHandler<RegisterCommand, AuthResponse?>
{
    private readonly IAuthService _authService;
    private readonly IJwtTokenGenerator _jwtToken;
    public RegisterHandler(IAuthService authService, IJwtTokenGenerator jwtTokenGenerator)
    {
        _authService = authService;
        _jwtToken = jwtTokenGenerator;
    }
    async Task<AuthResponse?> IRequestHandler<RegisterCommand, AuthResponse?>.Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        var newUser = await _authService.RegisterUserAsync(request.UserName, request.Email, request.Password);
        if (newUser == null || !newUser.Succeeded)
            return null;

        var tokenRequest = new TokenRequestDto(newUser.Id, newUser.Name, newUser.Email, newUser.Roles);

        var token = _jwtToken.GenerateToken(tokenRequest);

        return new AuthResponse()
        {
            UserName = newUser.Name,
            Token = token
        };
    }
}
