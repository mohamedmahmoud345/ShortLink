using Microsoft.AspNetCore.Identity;
using ShortLink.Application.DTOs.Account;
using ShortLink.Application.Services.Auth;
using ShortLink.Domain.Interfaces.UnitOfWork;
using ShortLink.Infrastructure.Data.Identity;

namespace ShortLink.Infrastructure.Services.Auth;

public class AuthService : IAuthService
{
    private readonly UserManager<ApplicationUser> _userMang;
    private readonly IUnitOfWork _unitOfWork;
    public AuthService(UserManager<ApplicationUser> userManager, IUnitOfWork unitOfWork)
    {
        _userMang = userManager;
        _unitOfWork = unitOfWork;
    }
    public async Task<AuthDto?> RegisterUserAsync(string userName, string email, string password)
    {
        var existingUser = await _userMang.FindByEmailAsync(email);
        if (existingUser != null)
            return null;

        var user = new ApplicationUser()
        {
            UserName = userName,
            Email = email
        };
        await _unitOfWork.ExecuteInTransactionAsync(async () =>
        {
            var result = await _userMang.CreateAsync(user, password);
            if (!result.Succeeded)
                throw new InvalidOperationException();

            var isSeccess = await _userMang.AddToRoleAsync(user, "User");
            if (!isSeccess.Succeeded)
                throw new InvalidOperationException();
        });

        var roles = await _userMang.GetRolesAsync(user);
        var authDto = new AuthDto(user.Id, email, userName, true, roles.ToList());

        return authDto;
    }
    public async Task<AuthDto?> LoginAsync(string email, string password)
    {
        var existingUser = await _userMang.FindByEmailAsync(email);
        if (existingUser == null)
            return null;

        var result = await _userMang.CheckPasswordAsync(existingUser, password);
        if (!result)
            return null;

        var roles = await _userMang.GetRolesAsync(existingUser);

        var authDto = new AuthDto(existingUser.Id, email, existingUser.UserName!, true, roles.ToList());

        return authDto;
    }
}
