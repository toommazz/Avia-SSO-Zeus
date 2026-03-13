using Avia.SSO.Zeus.Application.Common.DTOs;
using Avia.SSO.Zeus.Domain.Identity.Aggregates;
using Avia.SSO.Zeus.Domain.Identity.Errors;
using Avia.SSO.Zeus.Domain.Identity.Repositories;
using Avia.SSO.Zeus.Domain.Identity.Services;
using Avia.SSO.Zeus.Domain.Identity.ValueObjects;
using Avia.SSO.Zeus.Domain.Multitenancy.ValueObjects;
using Avia.SSO.Zeus.Domain.Shared;
using MediatR;

namespace Avia.SSO.Zeus.Application.Identity.Commands.Register;

public sealed class RegisterUserCommandHandler(
    IUserRepository userRepository,
    IPasswordHasher passwordHasher)
    : IRequestHandler<RegisterUserCommand, Result<UserDto>>
{
    public async Task<Result<UserDto>> Handle(RegisterUserCommand request, CancellationToken ct)
    {
        var emailResult = Email.Create(request.Email);
        if (emailResult.IsFailure)
            return Result.Failure<UserDto>(emailResult.Error);

        var tenantIdResult = TenantId.Create(request.TenantId);
        if (tenantIdResult.IsFailure)
            return Result.Failure<UserDto>(tenantIdResult.Error);

        var emailExists = await userRepository.EmailExistsInTenantAsync(emailResult.Value, tenantIdResult.Value, ct);
        if (emailExists)
            return Result.Failure<UserDto>(UserErrors.Email.AlreadyInUse);

        var (hash, salt) = passwordHasher.Hash(request.Password);

        var userResult = User.Register(request.TenantId, request.Email, hash, salt);
        if (userResult.IsFailure)
            return Result.Failure<UserDto>(userResult.Error);

        await userRepository.AddAsync(userResult.Value, ct);

        var dto = new UserDto(
            userResult.Value.UserId.Value,
            userResult.Value.TenantId.Value,
            userResult.Value.Email.Value,
            userResult.Value.Status.ToString(),
            userResult.Value.TwoFactorMethod != Domain.Identity.Enums.TwoFactorMethod.None);

        return Result.Success(dto);
    }
}
