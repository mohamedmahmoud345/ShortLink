
using FluentValidation;

namespace ShortLink.Application.Features.ShortUrl.Commands.DeleteUrl;

public class DeleteValidator : AbstractValidator<DeleteCommand>
{
    public DeleteValidator()
    {
        RuleFor(x => x.UrlId)
            .NotEmpty().WithMessage("Url id is required");

        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("User id is required");

    }
}
