using Avia.SSO.Zeus.Application.Identity.Commands.Register;
using Avia.SSO.Zeus.Domain.Identity.Aggregates;
using Avia.SSO.Zeus.Domain.Identity.Repositories;
using Avia.SSO.Zeus.Domain.Identity.Services;
using Avia.SSO.Zeus.Domain.Identity.ValueObjects;
using Avia.SSO.Zeus.Domain.Multitenancy.ValueObjects;
using Moq;

namespace Avia.SSO.Zeus.Tests.Application.Identity;

[TestClass]
public sealed class RegisterUserCommandHandlerTests
{
    private readonly Mock<IUserRepository> _userRepositoryMock = new();
    private readonly Mock<IPasswordHasher> _passwordHasherMock = new();
    private readonly RegisterUserCommandHandler _handler;

    public RegisterUserCommandHandlerTests()
    {
        _passwordHasherMock
            .Setup(x => x.Hash(It.IsAny<string>()))
            .Returns(("hashed", "salted"));

        _handler = new RegisterUserCommandHandler(
            _userRepositoryMock.Object,
            _passwordHasherMock.Object);
    }

    [TestMethod]
    public async Task Handle_WithValidData_ShouldSucceed()
    {
        var tenantId = Guid.NewGuid();
        _userRepositoryMock
            .Setup(x => x.EmailExistsInTenantAsync(It.IsAny<Email>(), It.IsAny<TenantId>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var command = new RegisterUserCommand(tenantId, "user@example.com", "Password@1");
        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.IsTrue(result.IsSuccess);
        Assert.AreEqual("user@example.com", result.Value.Email);
        _userRepositoryMock.Verify(x => x.AddAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [TestMethod]
    public async Task Handle_WithDuplicateEmail_ShouldFail()
    {
        var tenantId = Guid.NewGuid();
        _userRepositoryMock
            .Setup(x => x.EmailExistsInTenantAsync(It.IsAny<Email>(), It.IsAny<TenantId>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var command = new RegisterUserCommand(tenantId, "user@example.com", "Password@1");
        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.IsTrue(result.IsFailure);
        _userRepositoryMock.Verify(x => x.AddAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [TestMethod]
    public async Task Handle_WithInvalidEmail_ShouldFail()
    {
        var command = new RegisterUserCommand(Guid.NewGuid(), "invalidemail", "Password@1");
        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.IsTrue(result.IsFailure);
        _userRepositoryMock.Verify(x => x.AddAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [TestMethod]
    public async Task Handle_WithEmptyTenantId_ShouldFail()
    {
        var command = new RegisterUserCommand(Guid.Empty, "user@example.com", "Password@1");
        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.IsTrue(result.IsFailure);
    }
}
