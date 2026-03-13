using Avia.SSO.Zeus.Application.Common.DTOs;
using Avia.SSO.Zeus.Domain.Shared;
using MediatR;

namespace Avia.SSO.Zeus.Application.Identity.Commands.Register;

public sealed record RegisterUserCommand(
    Guid TenantId,
    string Email,
    string Password) : IRequest<Result<UserDto>>;
