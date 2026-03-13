using Avia.SSO.Zeus.Application.Common.DTOs;
using RefreshTokenEntity = Avia.SSO.Zeus.Domain.Identity.Entities.RefreshToken;
using Avia.SSO.Zeus.Domain.Identity.Enums;
using Avia.SSO.Zeus.Domain.Identity.Errors;
using Avia.SSO.Zeus.Domain.Identity.Repositories;
using Avia.SSO.Zeus.Domain.Identity.Services;
using Avia.SSO.Zeus.Domain.Identity.ValueObjects;
using Avia.SSO.Zeus.Domain.Multitenancy.ValueObjects;
using Avia.SSO.Zeus.Domain.Session.Aggregates;
using Avia.SSO.Zeus.Domain.Session.Repositories;
using Avia.SSO.Zeus.Domain.Session.ValueObjects;
using Avia.SSO.Zeus.Domain.Shared;
using MediatR;

namespace Avia.SSO.Zeus.Application.Identity.Commands.Login;

public sealed class LoginCommandHandler(
    IUserRepository userRepository,
    IRefreshTokenRepository refreshTokenRepository,
    IAuthSessionRepository authSessionRepository,
    IPasswordHasher passwordHasher,
    ITokenService tokenService)
    : IRequestHandler<LoginCommand, Result<AuthTokenDto>>
{
    public async Task<Result<AuthTokenDto>> Handle(LoginCommand request, CancellationToken ct)
    {
        var emailResult = Email.Create(request.Email);
        if (emailResult.IsFailure)
            return Result.Failure<AuthTokenDto>(UserErrors.InvalidCredentials);

        var tenantIdResult = TenantId.Create(request.TenantId);
        if (tenantIdResult.IsFailure)
            return Result.Failure<AuthTokenDto>(UserErrors.InvalidCredentials);

        var user = await userRepository.GetByEmailAndTenantAsync(emailResult.Value, tenantIdResult.Value, ct);
        if (user is null)
            return Result.Failure<AuthTokenDto>(UserErrors.InvalidCredentials);

        if (user.Status == Domain.Identity.Enums.UserStatus.Locked)
            return Result.Failure<AuthTokenDto>(UserErrors.LockedOut);

        if (user.Status == Domain.Identity.Enums.UserStatus.Deactivated)
            return Result.Failure<AuthTokenDto>(UserErrors.InvalidCredentials);

        var passwordValid = passwordHasher.Verify(request.Password, user.Password.Hash, user.Password.Salt);
        if (!passwordValid)
        {
            user.RecordLoginAttempt(false, LoginFailureReason.InvalidPassword, request.IpAddress);
            await userRepository.UpdateAsync(user, ct);
            return Result.Failure<AuthTokenDto>(UserErrors.InvalidCredentials);
        }

        if (user.TwoFactorMethod != TwoFactorMethod.None)
            return Result.Failure<AuthTokenDto>(UserErrors.TwoFactorRequired);

        user.RecordLoginAttempt(true, ipAddress: request.IpAddress);
        await userRepository.UpdateAsync(user, ct);

        var accessToken = tokenService.GenerateAccessToken(user);
        var refreshTokenValue = tokenService.GenerateRefreshToken();
        var refreshToken = RefreshTokenEntity.Create(user.UserId.Value, refreshTokenValue, DateTime.UtcNow.AddDays(7));
        await refreshTokenRepository.AddAsync(refreshToken, ct);

        var deviceInfo = DeviceInfo.Create("API", request.IpAddress);
        var sessionResult = AuthSession.Create(user.UserId.Value, user.TenantId.Value, deviceInfo);
        if (sessionResult.IsSuccess)
            await authSessionRepository.AddAsync(sessionResult.Value, ct);

        return Result.Success(new AuthTokenDto(accessToken, refreshTokenValue, 60));
    }
}
