
using ShortLink.Domain.Interfaces.Repositories;

namespace ShortLink.Domain.Interfaces.UnitOfWork;

public interface IUnitOfWork
{
    IShortUrlRepository ShortUrls { get; }
    IClickEventRepository ClickEvents { get; }
    Task<int> SaveChangesAsync();
}
