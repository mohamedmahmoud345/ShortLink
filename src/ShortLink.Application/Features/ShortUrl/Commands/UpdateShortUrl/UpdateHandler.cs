
using MediatR;
using ShortLink.Domain.Interfaces.UnitOfWork;

namespace ShortLink.Application.Features.ShortUrl.Commands.UpdateShortUrl;

public class UpdateHandler : IRequestHandler<UpdateCommand, bool>
{
    private readonly IUnitOfWork _unitOfWork;
    public UpdateHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }
    public async Task<bool> Handle(UpdateCommand request, CancellationToken cancellationToken)
    {
        var url = await _unitOfWork.ShortUrls.GetByIdForUserAsync(request.UrlId, request.UserId);

        if (url is null)
            return false;

        url.OriginalLink = request.Url;

        await _unitOfWork.ShortUrls.UpdateAsync(url);

        return true;
    }
}
