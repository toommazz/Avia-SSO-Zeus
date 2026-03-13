using Avia.SSO.Zeus.Application.Common.DTOs;
using Avia.SSO.Zeus.Domain.Shared;
using MediatR;

namespace Avia.SSO.Zeus.Application.Identity.Commands.VerifyTwoFactor;

public sealed record VerifyTwoFactorCommand(
    Guid UserId,
    string Code) : IRequest<Result<AuthTokenDto>>;
