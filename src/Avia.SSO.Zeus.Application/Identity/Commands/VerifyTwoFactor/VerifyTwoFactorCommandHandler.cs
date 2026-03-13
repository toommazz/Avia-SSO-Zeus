using Avia.SSO.Zeus.Application.Common.DTOs;
using RefreshTokenEntity = Avia.SSO.Zeus.Domain.Identity.Entities.RefreshToken;
using Avia.SSO.Zeus.Domain.Identity.Errors;
using Avia.SSO.Zeus.Domain.Identity.Repositories;
using Avia.SSO.Zeus.Domain.Identity.Services;
using Avia.SSO.Zeus.Domain.Identity.ValueObjects;
using Avia.SSO.Zeus.Domain.Shared;
using MediatR;

namespace Avia.SSO.Zeus.Application.Identity.Commands.VerifyTwoFactor;

public sealed class VerifyTwoFactorCommandHandler(
    IUserRepository userRepository,
    IRefreshTokenRepository refreshTokenRepository,
    ITwoFactorService twoFactorService,
    ITokenService tokenService)
    : IRequestHandler<VerifyTwoFactorCommand, Result<AuthTokenDto>>
{
    public async Task<Result<AuthTokenDto>> Handle(VerifyTwoFactorCommand request, CancellationToken ct)
    {
        var userId = UserId.From(request.UserId);
        var user = await userRepository.GetByIdAsync(userId, ct);
        if (user is null)
            return Result.Failure<AuthTokenDto>(UserErrors.NotFound);

        if (user.TwoFactorSecret is null)
            return Result.Failure<AuthTokenDto>(UserErrors.TwoFactorInvalid);

        var isValid = twoFactorService.VerifyCode(user.TwoFactorSecret.Value, request.Code, user.TwoFactorMethod);
        if (!isValid)
            return Result.Failure<AuthTokenDto>(UserErrors.TwoFactorInvalid);

        var accessToken = tokenService.GenerateAccessToken(user);
        var refreshTokenValue = tokenService.GenerateRefreshToken();
        var refreshToken = RefreshTokenEntity.Create(user.UserId.Value, refreshTokenValue, DateTime.UtcNow.AddDays(7));
        await refreshTokenRepository.AddAsync(refreshToken, ct);

        return Result.Success(new AuthTokenDto(accessToken, refreshTokenValue, 60));
    }
}
