using FluentValidation;
using ShortLink.Application.Features.Account.Register;

namespace ShortLink.Application.Features.Account;

public class RegisterValidator : AbstractValidator<RegisterCommand>
{
    public RegisterValidator()
    {
        RuleFor(x => x.UserName)
            .NotEmpty().WithMessage("User name is required");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required")
            .EmailAddress().WithMessage("InValid email format");

        RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Password is required");
    }
}
