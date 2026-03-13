using Avia.SSO.Zeus.Domain.Common;
using Avia.SSO.Zeus.Domain.Multitenancy.Errors;
using Avia.SSO.Zeus.Domain.Multitenancy.Events;
using Avia.SSO.Zeus.Domain.Multitenancy.ValueObjects;
using Avia.SSO.Zeus.Domain.Shared;

namespace Avia.SSO.Zeus.Domain.Multitenancy.Entities;

public sealed class Tenant : AggregateRoot
{
    public TenantId TenantId { get; private set; } = null!;
    public TenantName Name { get; private set; } = null!;
    public TenantSettings Settings { get; private set; } = null!;
    public bool IsActive { get; private set; }
    public DateTime CreatedAt { get; private set; }

    private Tenant() { }

    public static Result<Tenant> Create(Guid id, string name)
    {
        var tenantIdResult = TenantId.Create(id);
        if (tenantIdResult.IsFailure)
            return Result.Failure<Tenant>(tenantIdResult.Error);

        var nameResult = TenantName.Create(name);
        if (nameResult.IsFailure)
            return Result.Failure<Tenant>(nameResult.Error);

        var tenant = new Tenant
        {
            Id = id,
            TenantId = tenantIdResult.Value,
            Name = nameResult.Value,
            Settings = TenantSettings.Default(),
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        tenant.RaiseDomainEvent(new TenantCreatedEvent(id, nameResult.Value.Value));
        return Result.Success(tenant);
    }

    public Result Deactivate()
    {
        if (!IsActive)
            return Result.Failure(TenantErrors.Inactive);

        IsActive = false;
        RaiseDomainEvent(new TenantDeactivatedEvent(TenantId.Value));
        return Result.Success();
    }
}
