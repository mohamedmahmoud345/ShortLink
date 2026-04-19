using ShortLink.Domain.Interfaces.Repositories;
using ShortLink.Domain.Interfaces.UnitOfWork;
using ShortLink.Infrastructure.Dapper;
using ShortLink.Infrastructure.Data;
using ShortLink.Infrastructure.Repositories;


namespace ShortLink.Infrastructure.UnitofWork;

public class UnitOfWork : IUnitOfWork
{
    private readonly AppDbContext _context;
    private IShortUrlRepository _shortUrlRepository;
    private IClickEventRepository _clickEventRepository;

    public UnitOfWork(AppDbContext context, DapperContext dapperContext)
    {
        _context = context;
        _shortUrlRepository = new ShortUrlRepository(context);
        _clickEventRepository = new ClickEventRepository(context, dapperContext);
    }

    public IShortUrlRepository ShortUrls => _shortUrlRepository;

    public IClickEventRepository ClickEvents => _clickEventRepository;

    public async Task<int> SaveChangesAsync()
    {
        return await _context.SaveChangesAsync();
    }
}
