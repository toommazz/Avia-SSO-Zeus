using Avia.SSO.Zeus.Domain.Identity.Errors;
using Avia.SSO.Zeus.Domain.Identity.Repositories;
using Avia.SSO.Zeus.Domain.Identity.Services;
using Avia.SSO.Zeus.Domain.Identity.ValueObjects;
using Avia.SSO.Zeus.Domain.Shared;
using MediatR;

namespace Avia.SSO.Zeus.Application.Identity.Commands.EnableTwoFactor;

public sealed class EnableTwoFactorCommandHandler(
    IUserRepository userRepository,
    ITwoFactorService twoFactorService)
    : IRequestHandler<EnableTwoFactorCommand, Result<string>>
{
    public async Task<Result<string>> Handle(EnableTwoFactorCommand request, CancellationToken ct)
    {
        var userId = UserId.From(request.UserId);
        var user = await userRepository.GetByIdAsync(userId, ct);
        if (user is null)
            return Result.Failure<string>(UserErrors.NotFound);

        var secret = twoFactorService.GenerateSecret();
        var result = user.EnableTwoFactor(secret, request.Method);
        if (result.IsFailure)
            return Result.Failure<string>(result.Error);

        await userRepository.UpdateAsync(user, ct);
        return Result.Success(secret);
    }
}
