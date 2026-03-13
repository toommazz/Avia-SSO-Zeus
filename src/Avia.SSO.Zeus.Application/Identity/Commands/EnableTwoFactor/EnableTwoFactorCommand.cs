using Avia.SSO.Zeus.Domain.Identity.Enums;
using Avia.SSO.Zeus.Domain.Shared;
using MediatR;

namespace Avia.SSO.Zeus.Application.Identity.Commands.EnableTwoFactor;

public sealed record EnableTwoFactorCommand(
    Guid UserId,
    TwoFactorMethod Method) : IRequest<Result<string>>;
