using Avia.SSO.Zeus.Application.Multitenancy.Commands.CreateTenant;
using Avia.SSO.Zeus.Domain.Multitenancy.Entities;
using Avia.SSO.Zeus.Domain.Multitenancy.Repositories;
using Moq;

namespace Avia.SSO.Zeus.Tests.Application.Multitenancy;

[TestClass]
public sealed class CreateTenantCommandHandlerTests
{
    private readonly Mock<ITenantRepository> _tenantRepositoryMock = new();
    private readonly CreateTenantCommandHandler _handler;

    public CreateTenantCommandHandlerTests()
    {
        _handler = new CreateTenantCommandHandler(_tenantRepositoryMock.Object);
    }

    [TestMethod]
    public async Task Handle_WithValidName_ShouldSucceed()
    {
        var command = new CreateTenantCommand("Acme Corp");
        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.IsTrue(result.IsSuccess);
        Assert.AreEqual("Acme Corp", result.Value.Name);
        Assert.IsTrue(result.Value.IsActive);
        _tenantRepositoryMock.Verify(x => x.AddAsync(It.IsAny<Tenant>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [TestMethod]
    public async Task Handle_WithEmptyName_ShouldFail()
    {
        var command = new CreateTenantCommand("");
        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.IsTrue(result.IsFailure);
        _tenantRepositoryMock.Verify(x => x.AddAsync(It.IsAny<Tenant>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [TestMethod]
    public async Task Handle_WithNameTooLong_ShouldFail()
    {
        var command = new CreateTenantCommand(new string('A', 101));
        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.IsTrue(result.IsFailure);
        _tenantRepositoryMock.Verify(x => x.AddAsync(It.IsAny<Tenant>(), It.IsAny<CancellationToken>()), Times.Never);
    }
}
