
using FluentValidation;

namespace ShortLink.Application.Features.ClickEvent.Commands.RecordClickEvent;

public class RecordClickEventValidator : AbstractValidator<RecordClickEventCommand>
{
    public RecordClickEventValidator()
    {
        RuleFor(_ => _.ShortCode)
            .NotEmpty().WithMessage("Short code is required");

        RuleFor(_ => _.Referrer)
            .NotEmpty().WithMessage("Referrer is required");

        RuleFor(_ => _.IpAddress)
                    .NotEmpty().WithMessage("Ip address is required");
        RuleFor(_ => _.UserAgent)
            .NotEmpty().WithMessage("user agent is required");
    }
}
