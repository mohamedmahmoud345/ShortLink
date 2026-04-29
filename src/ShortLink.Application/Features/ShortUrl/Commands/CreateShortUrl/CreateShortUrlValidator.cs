
using FluentValidation;

namespace ShortLink.Application.Features.ShortUrl.Commands.CreateShortUrl;

public class CreateShortUrlValidator : AbstractValidator<CreateShortUrlCommand>
{
    public CreateShortUrlValidator()
    {
        RuleFor(x => x.OriginalLink)
            .NotEmpty().WithMessage("Url is requered");

    }
}
