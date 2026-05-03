
using FluentValidation;

namespace ShortLink.Application.Features.ShortUrl.Commands.UpdateShortUrl;

public class UpdateValidator : AbstractValidator<UpdateCommand>
{
    public UpdateValidator()
    {
        RuleFor(x => x.UrlId)
            .NotEmpty().WithMessage("Url id is required");

        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("User id is required");

        RuleFor(x => x.Url)
            .NotEmpty().WithMessage("Url is required");
    }
}
