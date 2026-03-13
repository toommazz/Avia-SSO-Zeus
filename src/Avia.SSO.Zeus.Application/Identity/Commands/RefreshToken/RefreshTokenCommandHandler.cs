using Avia.SSO.Zeus.Application.Common.DTOs;
using Avia.SSO.Zeus.Domain.Identity.Errors;
using Avia.SSO.Zeus.Domain.Identity.Repositories;
using Avia.SSO.Zeus.Domain.Identity.Services;
using Avia.SSO.Zeus.Domain.Identity.ValueObjects;
using Avia.SSO.Zeus.Domain.Shared;
using MediatR;

namespace Avia.SSO.Zeus.Application.Identity.Commands.RefreshToken;

public sealed class RefreshTokenCommandHandler(
    IRefreshTokenRepository refreshTokenRepository,
    IUserRepository userRepository,
    ITokenService tokenService)
    : IRequestHandler<RefreshTokenCommand, Result<AuthTokenDto>>
{
    public async Task<Result<AuthTokenDto>> Handle(RefreshTokenCommand request, CancellationToken ct)
    {
        var refreshToken = await refreshTokenRepository.GetByTokenAsync(request.Token, ct);
        if (refreshToken is null || !refreshToken.IsActive)
            return Result.Failure<AuthTokenDto>(new Error("RefreshToken.Invalid", "Invalid or expired refresh token.", ErrorType.Unauthorized));

        refreshToken.Revoke();
        await refreshTokenRepository.UpdateAsync(refreshToken, ct);

        var userId = UserId.From(refreshToken.UserId);
        var user = await userRepository.GetByIdAsync(userId, ct);
        if (user is null)
            return Result.Failure<AuthTokenDto>(UserErrors.NotFound);

        var accessToken = tokenService.GenerateAccessToken(user);
        var newRefreshTokenValue = tokenService.GenerateRefreshToken();
        var newRefreshToken = Domain.Identity.Entities.RefreshToken.Create(refreshToken.UserId, newRefreshTokenValue, DateTime.UtcNow.AddDays(7));
        await refreshTokenRepository.AddAsync(newRefreshToken, ct);

        return Result.Success(new AuthTokenDto(accessToken, newRefreshTokenValue, 60));
    }
}
