
using FluentValidation;

namespace ShortLink.Application.Features.ShortUrl.Commands.RefreshLink;

public class RefreshLinkValidator : AbstractValidator<RefreshLinkCommand>
{
    public RefreshLinkValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("User id is required");

        RuleFor(x => x.UrlId)
            .NotEmpty().WithMessage("Url id is required");
    }
}
