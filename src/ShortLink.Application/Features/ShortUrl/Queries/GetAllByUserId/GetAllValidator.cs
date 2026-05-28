
using FluentValidation;

namespace ShortLink.Application.Features.ShortUrl.Queries.GetAllByUserId;

public class GetAllValidator : AbstractValidator<GetAllQuery>
{
    public GetAllValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("User Id is required");
    }
}
