using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Avia.SSO.Zeus.Domain.Identity.Repositories;
using Avia.SSO.Zeus.Domain.Identity.Services;
using Avia.SSO.Zeus.Domain.Messaging;
using Avia.SSO.Zeus.Domain.Multitenancy.Repositories;
using Avia.SSO.Zeus.Domain.Session.Repositories;
using Avia.SSO.Zeus.Domain.Shared;
using Avia.SSO.Zeus.Infrastructure.Messaging;
using Avia.SSO.Zeus.Infrastructure.Multitenancy;
using Avia.SSO.Zeus.Infrastructure.Persistence;
using Avia.SSO.Zeus.Infrastructure.Persistence.Repositories;
using Avia.SSO.Zeus.Infrastructure.Security;

namespace Avia.SSO.Zeus.Infrastructure.DependencyInjection;

public static class InfrastructureServiceExtensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("Postgres")
            ?? throw new InvalidOperationException("Connection string 'Postgres' is not configured.");

        services.AddSingleton<IDbConnectionFactory>(_ => new DbConnectionFactory(connectionString));

        services.Configure<JwtSettings>(configuration.GetSection("Jwt"));

        services.AddHttpContextAccessor();

        // Repositories
        services.AddScoped<ITenantRepository, TenantRepository>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
        services.AddScoped<IAuthSessionRepository, AuthSessionRepository>();

        // Services
        services.AddScoped<IPasswordHasher, PasswordHasher>();
        services.AddScoped<ITwoFactorService, TwoFactorService>();
        services.AddScoped<ITokenService, TokenService>();

        // Multitenancy
        services.AddScoped<ITenantContext, HttpTenantContext>();

        // Messaging
        services.AddScoped<IEventBus, InMemoryEventBus>();

        return services;
    }
}
