using Avia.SSO.Zeus.Domain.Multitenancy.Entities;
using FluentValidation;

namespace Avia.SSO.Zeus.Domain.Multitenancy.Validators;

public class TenantValidator : AbstractValidator<Tenant>
{
    public TenantValidator()
    {
        RuleFor(t => t.Name).NotNull();
        RuleFor(t => t.TenantId).NotNull();
    }
}
