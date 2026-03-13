using Avia.SSO.Zeus.Application.Common.DTOs;
using Avia.SSO.Zeus.Domain.Shared;
using MediatR;

namespace Avia.SSO.Zeus.Application.Identity.Queries.GetUser;

public sealed record GetUserQuery(Guid UserId) : IRequest<Result<UserDto>>;
