
using MediatR;
using ShortLink.Domain.Interfaces.UnitOfWork;

namespace ShortLink.Application.Features.ShortUrl.Commands.DeleteUrl;

public class DeleteHandler : IRequestHandler<DeleteCommand, bool>
{
    private readonly IUnitOfWork _unitOfWork;
    public DeleteHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }
    public async Task<bool> Handle(DeleteCommand request, CancellationToken cancellationToken)
    {
        var url = await _unitOfWork.ShortUrls.GetByIdForUserAsync(request.UrlId, request.UserId);
        if (url is null)
            return false;

        url.IsActive = false;

        await _unitOfWork.ShortUrls.UpdateAsync(url);
        return true;
    }
}
