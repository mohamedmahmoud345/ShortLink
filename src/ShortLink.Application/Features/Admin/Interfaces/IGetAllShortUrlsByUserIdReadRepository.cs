
namespace ShortLink.Application.Features.Admin.Interfaces;

public interface IGetAllShortUrlsByUserIdReadRepository
{
    Task<IEnumerable<Domain.Entities.ShortUrl>> GetByUserIdAsync(Guid userId);
}
