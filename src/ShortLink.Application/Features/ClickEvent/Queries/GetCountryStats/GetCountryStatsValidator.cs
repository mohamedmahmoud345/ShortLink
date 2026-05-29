
using System.Data;
using FluentValidation;

namespace ShortLink.Application.Features.ClickEvent.Queries.GetCountryStats;

public class GetCountryStatsValidator : AbstractValidator<GetCountryStatsQuery>
{
    public GetCountryStatsValidator()
    {
        RuleFor(x => x.UrlId)
            .NotEmpty().WithMessage("Url Id is required");

        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("User Id is required");
    }
}
