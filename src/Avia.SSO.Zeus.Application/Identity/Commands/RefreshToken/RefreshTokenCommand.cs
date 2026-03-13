using Avia.SSO.Zeus.Application.Common.DTOs;
using Avia.SSO.Zeus.Domain.Shared;
using MediatR;

namespace Avia.SSO.Zeus.Application.Identity.Commands.RefreshToken;

public sealed record RefreshTokenCommand(string Token) : IRequest<Result<AuthTokenDto>>;
