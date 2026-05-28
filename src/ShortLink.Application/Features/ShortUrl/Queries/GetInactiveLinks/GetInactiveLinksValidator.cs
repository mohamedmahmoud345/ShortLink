
using FluentValidation;

namespace ShortLink.Application.Features.ShortUrl.Queries.GetInactiveLinks;

public class GetInactiveLinksValidator : AbstractValidator<GetInactiveLinksQuery>
{   
    public GetInactiveLinksValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("User id is required");
    }
}
