using Avia.SSO.Zeus.Domain.Identity.Errors;
using FluentValidation;

namespace Avia.SSO.Zeus.Domain.Identity.Validators;

public class PasswordValidator : AbstractValidator<string>
{
    public PasswordValidator()
    {
        RuleFor(p => p)
            .NotEmpty().WithErrorCode(UserErrors.Password.Empty.Code)
            .MinimumLength(8).WithErrorCode(UserErrors.Password.TooShort.Code)
            .Matches("[A-Z]").WithErrorCode(UserErrors.Password.NoUpperCase.Code)
            .Matches("[^a-zA-Z0-9]").WithErrorCode(UserErrors.Password.NoSpecialChar.Code);
    }
}
