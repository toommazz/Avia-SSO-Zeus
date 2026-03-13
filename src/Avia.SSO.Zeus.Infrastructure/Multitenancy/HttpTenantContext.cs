using Microsoft.AspNetCore.Http;
using Avia.SSO.Zeus.Domain.Shared;

namespace Avia.SSO.Zeus.Infrastructure.Multitenancy;

public sealed class HttpTenantContext(IHttpContextAccessor httpContextAccessor) : ITenantContext
{
    private const string TenantIdHeader = "X-Tenant-Id";

    public Guid TenantId
    {
        get
        {
            var context = httpContextAccessor.HttpContext;
            if (context is null)
                throw new InvalidOperationException("No HTTP context available.");

            if (context.Request.Headers.TryGetValue(TenantIdHeader, out var value) &&
                Guid.TryParse(value, out var tenantId))
                return tenantId;

            throw new InvalidOperationException($"Header '{TenantIdHeader}' is missing or invalid.");
        }
    }
}
