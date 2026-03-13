using Avia.SSO.Zeus.Domain.Shared;

namespace Avia.SSO.Zeus.Domain.Multitenancy.Errors;

public static class TenantErrors
{
    public static class Id
    {
        public static readonly Error Empty =
            new("Tenant.Id.Empty", "Tenant ID cannot be empty.", ErrorType.Validation);
    }

    public static class Name
    {
        public static readonly Error Empty =
            new("Tenant.Name.Empty", "Tenant name cannot be empty.", ErrorType.Validation);
        public static readonly Error TooLong =
            new("Tenant.Name.TooLong", "Tenant name cannot exceed 100 characters.", ErrorType.Validation);
    }

    public static readonly Error NotFound =
        new("Tenant.NotFound", "Tenant not found.", ErrorType.NotFound);
    public static readonly Error AlreadyExists =
        new("Tenant.AlreadyExists", "Tenant already exists.", ErrorType.Conflict);
    public static readonly Error Inactive =
        new("Tenant.Inactive", "Tenant is inactive.", ErrorType.Forbidden);
}
