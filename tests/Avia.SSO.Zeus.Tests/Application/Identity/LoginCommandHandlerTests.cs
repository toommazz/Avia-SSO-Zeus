using Avia.SSO.Zeus.Application.Identity.Commands.Login;
using Avia.SSO.Zeus.Domain.Identity.Aggregates;
using Avia.SSO.Zeus.Domain.Identity.Enums;
using Avia.SSO.Zeus.Domain.Identity.Repositories;
using Avia.SSO.Zeus.Domain.Identity.Services;
using Avia.SSO.Zeus.Domain.Identity.ValueObjects;
using Avia.SSO.Zeus.Domain.Multitenancy.ValueObjects;
using Avia.SSO.Zeus.Domain.Session.Repositories;
using Moq;

namespace Avia.SSO.Zeus.Tests.Application.Identity;

[TestClass]
public sealed class LoginCommandHandlerTests
{
    private readonly Mock<IUserRepository> _userRepositoryMock = new();
    private readonly Mock<IRefreshTokenRepository> _refreshTokenRepositoryMock = new();
    private readonly Mock<IAuthSessionRepository> _authSessionRepositoryMock = new();
    private readonly Mock<IPasswordHasher> _passwordHasherMock = new();
    private readonly Mock<ITokenService> _tokenServiceMock = new();
    private readonly LoginCommandHandler _handler;

    private static readonly Guid TenantId = Guid.NewGuid();

    public LoginCommandHandlerTests()
    {
        _tokenServiceMock.Setup(x => x.GenerateAccessToken(It.IsAny<User>())).Returns("access_token");
        _tokenServiceMock.Setup(x => x.GenerateRefreshToken()).Returns("refresh_token");

        _handler = new LoginCommandHandler(
            _userRepositoryMock.Object,
            _refreshTokenRepositoryMock.Object,
            _authSessionRepositoryMock.Object,
            _passwordHasherMock.Object,
            _tokenServiceMock.Object);
    }

    private User CreateActiveUser()
    {
        return User.Register(TenantId, "user@example.com", "hashed", "salt").Value;
    }

    [TestMethod]
    public async Task Handle_WithValidCredentials_ShouldReturnTokens()
    {
        var user = CreateActiveUser();
        _userRepositoryMock
            .Setup(x => x.GetByEmailAndTenantAsync(It.IsAny<Email>(), It.IsAny<TenantId>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);
        _passwordHasherMock
            .Setup(x => x.Verify(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
            .Returns(true);

        var command = new LoginCommand(TenantId, "user@example.com", "Password@1");
        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.IsTrue(result.IsSuccess);
        Assert.AreEqual("access_token", result.Value.AccessToken);
        Assert.AreEqual("refresh_token", result.Value.RefreshToken);
    }

    [TestMethod]
    public async Task Handle_WithWrongPassword_ShouldFail()
    {
        var user = CreateActiveUser();
        _userRepositoryMock
            .Setup(x => x.GetByEmailAndTenantAsync(It.IsAny<Email>(), It.IsAny<TenantId>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);
        _passwordHasherMock
            .Setup(x => x.Verify(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
            .Returns(false);

        var command = new LoginCommand(TenantId, "user@example.com", "WrongPassword");
        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.IsTrue(result.IsFailure);
    }

    [TestMethod]
    public async Task Handle_WithUserNotFound_ShouldFail()
    {
        _userRepositoryMock
            .Setup(x => x.GetByEmailAndTenantAsync(It.IsAny<Email>(), It.IsAny<TenantId>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((User?)null);

        var command = new LoginCommand(TenantId, "notfound@example.com", "Password@1");
        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.IsTrue(result.IsFailure);
    }

    [TestMethod]
    public async Task Handle_WithLockedUser_ShouldFail()
    {
        var user = CreateActiveUser();
        for (int i = 0; i < 5; i++)
            user.RecordLoginAttempt(false, LoginFailureReason.InvalidPassword);

        _userRepositoryMock
            .Setup(x => x.GetByEmailAndTenantAsync(It.IsAny<Email>(), It.IsAny<TenantId>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        var command = new LoginCommand(TenantId, "user@example.com", "Password@1");
        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.IsTrue(result.IsFailure);
    }
}
