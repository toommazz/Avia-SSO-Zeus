using Avia.SSO.Zeus.Domain.Identity.Aggregates;
using FluentValidation;

namespace Avia.SSO.Zeus.Domain.Identity.Validators;

public class UserValidator : AbstractValidator<User>
{
    public UserValidator()
    {
        RuleFor(u => u.Email).NotNull();
        RuleFor(u => u.Password).NotNull();
        RuleFor(u => u.TenantId).NotNull();
    }
}
