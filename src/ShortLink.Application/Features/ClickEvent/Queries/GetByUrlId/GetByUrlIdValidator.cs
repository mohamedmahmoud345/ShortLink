
using FluentValidation;

namespace ShortLink.Application.Features.ClickEvent.Queries.GetByUrlId;

public class GetByUrlIdValidator : AbstractValidator<GetByUrlIdQuery>
{
    public GetByUrlIdValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("User Id is required");

        RuleFor(x => x.UrlId)
            .NotEmpty().WithMessage("Url Id is required");
    }
}
