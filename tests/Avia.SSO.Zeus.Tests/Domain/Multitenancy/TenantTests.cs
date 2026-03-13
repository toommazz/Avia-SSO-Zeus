using Avia.SSO.Zeus.Domain.Multitenancy.Entities;
using Avia.SSO.Zeus.Domain.Multitenancy.Events;

namespace Avia.SSO.Zeus.Tests.Domain.Multitenancy;

[TestClass]
public sealed class TenantTests
{
    [TestMethod]
    public void Create_WithValidData_ShouldSucceed()
    {
        var id = Guid.NewGuid();
        var result = Tenant.Create(id, "Acme Corp");
        Assert.IsTrue(result.IsSuccess);
        Assert.AreEqual("Acme Corp", result.Value.Name.Value);
        Assert.IsTrue(result.Value.IsActive);
    }

    [TestMethod]
    public void Create_ShouldRaiseTenantCreatedEvent()
    {
        var result = Tenant.Create(Guid.NewGuid(), "Acme Corp");
        Assert.IsInstanceOfType<TenantCreatedEvent>(result.Value.DomainEvents.First());
    }

    [TestMethod]
    public void Create_WithEmptyGuid_ShouldFail()
    {
        var result = Tenant.Create(Guid.Empty, "Acme Corp");
        Assert.IsTrue(result.IsFailure);
    }

    [TestMethod]
    public void Create_WithEmptyName_ShouldFail()
    {
        var result = Tenant.Create(Guid.NewGuid(), "");
        Assert.IsTrue(result.IsFailure);
    }

    [TestMethod]
    public void Deactivate_ShouldSetInactiveAndRaiseEvent()
    {
        var tenant = Tenant.Create(Guid.NewGuid(), "Acme Corp").Value;
        tenant.ClearDomainEvents();

        var result = tenant.Deactivate();

        Assert.IsTrue(result.IsSuccess);
        Assert.IsFalse(tenant.IsActive);
        Assert.IsInstanceOfType<TenantDeactivatedEvent>(tenant.DomainEvents.First());
    }

    [TestMethod]
    public void Deactivate_AlreadyInactive_ShouldFail()
    {
        var tenant = Tenant.Create(Guid.NewGuid(), "Acme Corp").Value;
        tenant.Deactivate();

        var result = tenant.Deactivate();

        Assert.IsTrue(result.IsFailure);
    }
}
