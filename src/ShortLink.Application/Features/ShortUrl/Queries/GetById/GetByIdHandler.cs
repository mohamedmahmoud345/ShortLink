
using MediatR;
using ShortLink.Application.Common;
using ShortLink.Domain.Interfaces.UnitOfWork;

namespace ShortLink.Application.Features.ShortUrl.Queries.GetById;

public class GetByIdHandler : IRequestHandler<GetByIdQuery, QueryResponse>
{
    private readonly IUnitOfWork _unitOfWork;
    public GetByIdHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }
    public async Task<QueryResponse> Handle(GetByIdQuery request, CancellationToken cancellationToken)
    {
        var url = await _unitOfWork.ShortUrls.GetByIdAsync(request.Id);
        if (url is null)
            throw new NotFoundException($"The link with ID '{request.Id}' was not found.");

        return new QueryResponse()
        {
            Id = url.Id,
            OriginalLink = url.OriginalLink,
            ShortCode = url.ShortCode,
            CreatedAt = url.CreatedAt,
            ExpiresAt = url.ExpiresAt,
            IsActive = url.IsActive,
            Clicks = url.Clicks
        };
    }
}
