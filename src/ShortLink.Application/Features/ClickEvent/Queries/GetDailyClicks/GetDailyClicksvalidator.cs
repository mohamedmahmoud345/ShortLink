
using FluentValidation;

namespace ShortLink.Application.Features.ClickEvent.Queries.GetDailyClicks;

public class GetDailyClicksvalidator : AbstractValidator<GetDailyClicksQuery>
{
    public GetDailyClicksvalidator()
    {
        RuleFor(x => x.UserId)
                    .NotEmpty().WithMessage("User Id is required");

        RuleFor(x => x.UrlId)
            .NotEmpty().WithMessage("Url Id is required");

    }
}
