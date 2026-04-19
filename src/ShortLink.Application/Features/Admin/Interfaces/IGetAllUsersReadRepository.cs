using ShortLink.Application.DTOs.Admin;
using ShortLink.Domain.Entities;

namespace ShortLink.Application.Features.Admin.Interfaces;

public interface IGetAllUsersReadRepository
{
    Task<IEnumerable<UserInfoDto>> GetAllUsersAsync();
}
