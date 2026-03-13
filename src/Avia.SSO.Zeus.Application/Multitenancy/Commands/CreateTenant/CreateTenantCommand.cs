using Avia.SSO.Zeus.Application.Common.DTOs;
using Avia.SSO.Zeus.Domain.Shared;
using MediatR;

namespace Avia.SSO.Zeus.Application.Multitenancy.Commands.CreateTenant;

public sealed record CreateTenantCommand(string Name) : IRequest<Result<TenantDto>>;
