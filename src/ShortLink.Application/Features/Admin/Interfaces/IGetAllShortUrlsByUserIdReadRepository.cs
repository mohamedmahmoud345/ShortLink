using ShortLink.Domain.Entities;

namespace ShortLink.Application.Features.Admin.Interfaces;

public interface IGetAllShortUrlsByUserIdReadRepository
{
    Task<IEnumerable<ShortUrl>> GetByUserIdAsync(Guid userId);
}
