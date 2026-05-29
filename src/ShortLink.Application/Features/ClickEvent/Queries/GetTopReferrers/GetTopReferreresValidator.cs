
using System.Data;
using FluentValidation;

namespace ShortLink.Application.Features.ClickEvent.Queries.GetTopReferrers;

public class GetTopReferreresValidator : AbstractValidator<GetTopReferrersQuery>
{
    public GetTopReferreresValidator()
    {
        RuleFor(x => x.UrlId)
            .NotEmpty().WithMessage("Url Id is required");

        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("User Id is required");
    }
}
