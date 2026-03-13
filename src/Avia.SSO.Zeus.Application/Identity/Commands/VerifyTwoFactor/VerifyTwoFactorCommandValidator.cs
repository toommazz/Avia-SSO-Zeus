using FluentValidation;

namespace Avia.SSO.Zeus.Application.Identity.Commands.VerifyTwoFactor;

public sealed class VerifyTwoFactorCommandValidator : AbstractValidator<VerifyTwoFactorCommand>
{
    public VerifyTwoFactorCommandValidator()
    {
        RuleFor(x => x.UserId).NotEmpty().WithMessage("UserId is required.");
        RuleFor(x => x.Code).NotEmpty().WithMessage("Two-factor code is required.");
    }
}
