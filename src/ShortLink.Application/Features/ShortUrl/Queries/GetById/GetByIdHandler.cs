
using MediatR;
using ShortLink.Domain.Interfaces.UnitOfWork;

namespace ShortLink.Application.Features.ShortUrl.Queries.GetById;

public class GetByIdHandler : IRequestHandler<GetByIdQuery, QueryResponse?>
{
    private readonly IUnitOfWork _unitOfWork;
    public GetByIdHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }
    public async Task<QueryResponse?> Handle(GetByIdQuery request, CancellationToken cancellationToken)
    {
        var shortCode = await _unitOfWork.ShortUrls.GetByIdAsync(request.Id);
        if (shortCode is null)
            return null;

        return new QueryResponse()
        {
            Id = shortCode.Id,
            OriginalLink = shortCode.OriginalLink,
            ShortCode = shortCode.ShortCode,
            CreatedAt = shortCode.CreatedAt,
            ExpiresAt = shortCode.ExpiresAt,
            IsActive = shortCode.IsActive,
            Clicks = shortCode.Clicks
        };
    }
}
