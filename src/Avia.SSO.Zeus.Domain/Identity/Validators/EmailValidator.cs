using FluentValidation;

namespace Avia.SSO.Zeus.Domain.Identity.Validators;

public class EmailValidator : AbstractValidator<string>
{
    public EmailValidator()
    {
        RuleFor(e => e)
            .NotEmpty().WithMessage("Email cannot be empty.")
            .EmailAddress().WithMessage("Invalid email format.");
    }
}
