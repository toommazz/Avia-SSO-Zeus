using Avia.SSO.Zeus.Domain.Identity.Errors;
using Avia.SSO.Zeus.Domain.Identity.Repositories;
using Avia.SSO.Zeus.Domain.Identity.Services;
using Avia.SSO.Zeus.Domain.Identity.ValueObjects;
using Avia.SSO.Zeus.Domain.Shared;
using MediatR;

namespace Avia.SSO.Zeus.Application.Identity.Commands.ChangePassword;

public sealed class ChangePasswordCommandHandler(
    IUserRepository userRepository,
    IPasswordHasher passwordHasher)
    : IRequestHandler<ChangePasswordCommand, Result>
{
    public async Task<Result> Handle(ChangePasswordCommand request, CancellationToken ct)
    {
        var userId = UserId.From(request.UserId);
        var user = await userRepository.GetByIdAsync(userId, ct);
        if (user is null)
            return Result.Failure(UserErrors.NotFound);

        var currentPasswordValid = passwordHasher.Verify(request.CurrentPassword, user.Password.Hash, user.Password.Salt);
        if (!currentPasswordValid)
            return Result.Failure(UserErrors.InvalidCredentials);

        var (hash, salt) = passwordHasher.Hash(request.NewPassword);
        var result = user.ChangePassword(hash, salt);
        if (result.IsFailure)
            return result;

        await userRepository.UpdateAsync(user, ct);
        return Result.Success();
    }
}
