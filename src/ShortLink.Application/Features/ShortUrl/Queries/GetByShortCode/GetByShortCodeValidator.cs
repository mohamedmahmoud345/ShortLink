using FluentValidation;

namespace ShortLink.Application.Features.ShortUrl.Queries.GetByShortCode;

public class GetByShortCodeValidator : AbstractValidator<GetByShortCodeQuery>
{   
    public GetByShortCodeValidator()
    {
        RuleFor(x => x.ShortCode)
            .NotEmpty().WithMessage("short code is required");
    }
}
