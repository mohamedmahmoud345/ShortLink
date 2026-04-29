
using MediatR;
using ShortLink.Application.Features.ShortUrl.Queries.GetById;
using ShortLink.Domain.Interfaces.UnitOfWork;

namespace ShortLink.Application.Features.ShortUrl.Queries.GetAllAdmin;

public class GetAllAdminHandler : IRequestHandler<GetAllAdminQuery, List<QueryResponse>>
{
    private readonly IUnitOfWork _unitOfWork;
    public GetAllAdminHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }
    public async Task<List<QueryResponse>> Handle(GetAllAdminQuery request, CancellationToken cancellationToken)
    {
        var urls = await _unitOfWork.ShortUrls.GetAllAsync();
        if (!urls.Any())
            return [];

        var res = new List<QueryResponse>();
        foreach (var url in urls)
        {
            var obj = new QueryResponse()
            {
                Id = url.Id,
                OriginalLink = url.OriginalLink,
                ShortCode = url.ShortCode,
                CreatedAt = url.CreatedAt,
                ExpiresAt = url.ExpiresAt,
                IsActive = url.IsActive,
                Clicks = url.Clicks
            };
            res.Add(obj);
        }

        return res;
    }
}
