
using FluentValidation;

namespace ShortLink.Application.Features.ClickEvent.Queries.GetDeviceStats;

public class GetDeviceStatsValidator : AbstractValidator<GetDeviceStatsQuery>
{
    public GetDeviceStatsValidator()
    {
        RuleFor(x => x.UrlId)
            .NotEmpty().WithMessage("Url Id is required");

        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("User Id is required");
    }
}
