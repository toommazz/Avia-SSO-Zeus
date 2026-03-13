using Avia.SSO.Zeus.Application.Common.DTOs;
using Avia.SSO.Zeus.Domain.Multitenancy.Entities;
using Avia.SSO.Zeus.Domain.Multitenancy.Repositories;
using Avia.SSO.Zeus.Domain.Shared;
using MediatR;

namespace Avia.SSO.Zeus.Application.Multitenancy.Commands.CreateTenant;

public sealed class CreateTenantCommandHandler(ITenantRepository tenantRepository)
    : IRequestHandler<CreateTenantCommand, Result<TenantDto>>
{
    public async Task<Result<TenantDto>> Handle(CreateTenantCommand request, CancellationToken ct)
    {
        var result = Tenant.Create(Guid.NewGuid(), request.Name);
        if (result.IsFailure)
            return Result.Failure<TenantDto>(result.Error);

        await tenantRepository.AddAsync(result.Value, ct);

        var dto = new TenantDto(result.Value.TenantId.Value, result.Value.Name.Value, result.Value.IsActive);
        return Result.Success(dto);
    }
}
