using FluentValidation;

namespace Avia.SSO.Zeus.Application.Identity.Commands.EnableTwoFactor;

public sealed class EnableTwoFactorCommandValidator : AbstractValidator<EnableTwoFactorCommand>
{
    public EnableTwoFactorCommandValidator()
    {
        RuleFor(x => x.UserId).NotEmpty().WithMessage("UserId is required.");
        RuleFor(x => x.Method).IsInEnum().WithMessage("Invalid two-factor method.");
    }
}
