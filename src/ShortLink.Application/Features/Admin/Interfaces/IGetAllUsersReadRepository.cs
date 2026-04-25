using ShortLink.Application.DTOs.Admin;

namespace ShortLink.Application.Features.Admin.Interfaces;

public interface IGetAllUsersReadRepository
{
    Task<IEnumerable<UserInfoDto>> GetAllUsersAsync();
}
