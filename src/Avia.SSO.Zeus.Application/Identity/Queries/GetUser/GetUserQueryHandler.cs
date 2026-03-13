using Avia.SSO.Zeus.Application.Common.DTOs;
using Avia.SSO.Zeus.Domain.Identity.Errors;
using Avia.SSO.Zeus.Domain.Identity.Repositories;
using Avia.SSO.Zeus.Domain.Identity.ValueObjects;
using Avia.SSO.Zeus.Domain.Shared;
using MediatR;

namespace Avia.SSO.Zeus.Application.Identity.Queries.GetUser;

public sealed class GetUserQueryHandler(IUserRepository userRepository)
    : IRequestHandler<GetUserQuery, Result<UserDto>>
{
    public async Task<Result<UserDto>> Handle(GetUserQuery request, CancellationToken ct)
    {
        var userId = UserId.From(request.UserId);
        var user = await userRepository.GetByIdAsync(userId, ct);
        if (user is null)
            return Result.Failure<UserDto>(UserErrors.NotFound);

        var dto = new UserDto(
            user.UserId.Value,
            user.TenantId.Value,
            user.Email.Value,
            user.Status.ToString(),
            user.TwoFactorMethod != Domain.Identity.Enums.TwoFactorMethod.None);

        return Result.Success(dto);
    }
}
