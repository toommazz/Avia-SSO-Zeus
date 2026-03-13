using Avia.SSO.Zeus.Application.Common.DTOs;
using Avia.SSO.Zeus.Domain.Shared;
using MediatR;

namespace Avia.SSO.Zeus.Application.Identity.Commands.Login;

public sealed record LoginCommand(
    Guid TenantId,
    string Email,
    string Password,
    string? IpAddress = null) : IRequest<Result<AuthTokenDto>>;
