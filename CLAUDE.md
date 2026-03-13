# SSO-Zeus-AI

## Stack
- .NET 10
- C#
- VSCode
- PostgreSQL 16 + Dapper

## Estrutura
```
sso-zeus-ai/
├── .gitignore
├── CLAUDE.md
├── SSO-Zeus-AI.slnx
├── src/
│   ├── Avia.SSO.Zeus.Api/
│   │   ├── Properties/
│   │   │   └── launchSettings.json
│   │   ├── appsettings.json
│   │   ├── appsettings.Development.json
│   │   ├── Program.cs
│   │   ├── Avia.SSO.Zeus.Api.http
│   │   └── Avia.SSO.Zeus.Api.csproj
│   ├── Avia.SSO.Zeus.Application/
│   │   ├── Class1.cs
│   │   └── Avia.SSO.Zeus.Application.csproj
│   ├── Avia.SSO.Zeus.Domain/
│   │   ├── Common/
│   │   ├── Identity/
│   │   ├── Messaging/
│   │   ├── Multitenancy/
│   │   ├── Session/
│   │   ├── Shared/
│   │   └── Avia.SSO.Zeus.Domain.csproj
│   └── Avia.SSO.Zeus.Infrastructure/
│       ├── DependencyInjection/
│       ├── Messaging/
│       ├── Migrations/
│       ├── Multitenancy/
│       ├── Persistence/
│       ├── Security/
│       └── Avia.SSO.Zeus.Infrastructure.csproj
└── tests/
    └── Avia.SSO.Zeus.Tests/
        ├── MSTestSettings.cs
        ├── Test1.cs
        └── Avia.SSO.Zeus.Tests.csproj
```

## Projetos
- **Avia.SSO.Zeus.Api** — ASP.NET Core Web API (.NET 10)
- **Avia.SSO.Zeus.Application** — Camada de aplicação (use cases / handlers CQRS)
- **Avia.SSO.Zeus.Domain** — Camada de domínio (entidades, interfaces, regras de negócio)
- **Avia.SSO.Zeus.Infrastructure** — Camada de infraestrutura (persistência, serviços externos, mensageria)
- **Avia.SSO.Zeus.Tests** — Testes unitários (MSTest)

## Referências entre Projetos
- **Api** → Application, Infrastructure
- **Application** → Domain
- **Infrastructure** → Application, Domain
- **Domain** → (nenhuma)
- **Tests** → Domain, Application, Infrastructure

## Convenções Gerais
- Idioma do código: English
- Nomenclatura: PascalCase para classes e métodos
- Projetos ficam em `src/`
- Namespace base: `Avia.SSO.Zeus`

---

# Domain Layer — Avia.SSO.Zeus.Domain

## Contexto

Este projeto é um **SSO (Single Sign-On)** **multitenant** com autenticação por
**login/senha + 2FA (Two-Factor Authentication)**, construído com **.NET 10** seguindo
**DDD**, **CQRS** e **Mensageria**.

A camada de domínio é o **núcleo da aplicação** e:
- **NÃO** referencia nenhuma outra camada (Application, Infrastructure, Api)
- **NÃO** usa frameworks de persistência (EF Core, Dapper) diretamente
- **NÃO** usa frameworks HTTP ou de apresentação
- Contém toda a **lógica de negócio**, **regras de domínio** e **invariantes**
- É **completamente testável** de forma isolada

## Estrutura de Pastas — Domain
```
Avia.SSO.Zeus.Domain/
├── Common/
│   ├── BaseEntity.cs
│   ├── AggregateRoot.cs
│   ├── ValueObject.cs
│   ├── DomainEvent.cs
│   ├── IDomainEventHandler.cs
│   ├── Entity.cs
│   └── Enumeration.cs
│
├── Multitenancy/
│   ├── Entities/
│   │   └── Tenant.cs
│   ├── ValueObjects/
│   │   ├── TenantId.cs
│   │   ├── TenantName.cs
│   │   └── TenantSettings.cs
│   ├── Events/
│   │   ├── TenantCreatedEvent.cs
│   │   └── TenantDeactivatedEvent.cs
│   ├── Errors/
│   │   └── TenantErrors.cs
│   ├── Validators/
│   │   └── TenantValidator.cs
│   └── Repositories/
│       └── ITenantRepository.cs
│
├── Identity/
│   ├── Aggregates/
│   │   └── User.cs                    ← AggregateRoot principal
│   ├── Entities/
│   │   ├── RefreshToken.cs
│   │   ├── TwoFactorToken.cs
│   │   └── LoginAttempt.cs
│   ├── ValueObjects/
│   │   ├── UserId.cs
│   │   ├── Email.cs
│   │   ├── Password.cs                ← hash + salt encapsulados
│   │   ├── PhoneNumber.cs
│   │   └── TwoFactorSecret.cs
│   ├── Enums/
│   │   ├── UserStatus.cs
│   │   ├── TwoFactorMethod.cs
│   │   └── LoginFailureReason.cs
│   ├── Events/
│   │   ├── UserRegisteredEvent.cs
│   │   ├── UserLoginSucceededEvent.cs
│   │   ├── UserLoginFailedEvent.cs
│   │   ├── UserLockedOutEvent.cs
│   │   ├── TwoFactorRequestedEvent.cs
│   │   ├── TwoFactorVerifiedEvent.cs
│   │   ├── PasswordChangedEvent.cs
│   │   └── UserDeactivatedEvent.cs
│   ├── Errors/
│   │   └── UserErrors.cs
│   ├── Validators/
│   │   ├── UserValidator.cs
│   │   ├── EmailValidator.cs
│   │   └── PasswordValidator.cs
│   ├── Repositories/
│   │   ├── IUserRepository.cs
│   │   └── IRefreshTokenRepository.cs
│   └── Services/
│       ├── IPasswordHasher.cs         ← Interface; implementação na Infrastructure
│       ├── ITwoFactorService.cs
│       └── ITokenService.cs
│
├── Session/
│   ├── Aggregates/
│   │   └── AuthSession.cs
│   ├── ValueObjects/
│   │   ├── SessionId.cs
│   │   └── DeviceInfo.cs
│   ├── Events/
│   │   ├── SessionCreatedEvent.cs
│   │   └── SessionRevokedEvent.cs
│   ├── Errors/
│   │   └── SessionErrors.cs
│   └── Repositories/
│       └── IAuthSessionRepository.cs
│
├── Messaging/
│   ├── IEventBus.cs                   ← Interface de publicação; implementação na Infrastructure
│   └── IIntegrationEvent.cs           ← Contrato para eventos cross-bounded-context
│
└── Shared/
    ├── Result.cs                      ← Result<T> pattern — sem exceptions no domínio
    ├── Error.cs
    ├── ErrorType.cs
    └── ITenantContext.cs              ← Abstração para TenantId corrente (multitenant)
```

## Regras de Implementação

### BaseEntity e AggregateRoot
```csharp
public abstract class BaseEntity
{
    public Guid Id { get; protected set; }
    private readonly List<DomainEvent> _domainEvents = [];
    public IReadOnlyCollection<DomainEvent> DomainEvents => _domainEvents.AsReadOnly();
    protected void RaiseDomainEvent(DomainEvent domainEvent) => _domainEvents.Add(domainEvent);
    public void ClearDomainEvents() => _domainEvents.Clear();
}

public abstract class AggregateRoot : BaseEntity { }
```

### Value Objects

- Sempre **imutáveis** (`record` ou `sealed class` com construtor privado)
- Validação dentro do próprio ValueObject via factory method `Create(...)`
- Retornam `Result<T>` — **nunca lançam exceptions**

### Result Pattern
```csharp
public class Result
{
    public bool IsSuccess { get; }
    public bool IsFailure => !IsSuccess;
    public Error Error { get; }
    public static Result Success() => new(true, Error.None);
    public static Result Failure(Error error) => new(false, error);
    public static Result<T> Success<T>(T value) => new(value, true, Error.None);
    public static Result<T> Failure<T>(Error error) => new(default!, false, error);
}
```

### Aggregate User

- `TenantId` — multitenant, cada usuário pertence a um tenant
- `Email`, `Password` (hash + salt), `TwoFactorSecret` — Value Objects
- Lockout automático após **5 tentativas falhas consecutivas**
- Métodos: `Register`, `ChangePassword`, `EnableTwoFactor`, `RecordLoginAttempt`, `Unlock`, `Deactivate`
- Cada método retorna `Result` e levanta `DomainEvent`

### Multitenancy

- `ITenantContext` expõe o `TenantId` corrente (implementado na Infrastructure via header HTTP)
- **Nunca filtre por tenant na camada de domínio** — responsabilidade dos repositórios

### Separação de Eventos

| Tipo | Localização | Finalidade |
|---|---|---|
| `DomainEvent` | `Domain/Common` | Consistência interna do bounded context |
| `IIntegrationEvent` | `Domain/Messaging` | Publicado para outros serviços via broker |

## Package Permitido no Domain
```xml
<PackageReference Include="FluentValidation" Version="11.*" />
```

## O Que NÃO Fazer no Domain

- ❌ Referenciar `Microsoft.EntityFrameworkCore`
- ❌ Usar `ILogger` ou qualquer infraestrutura de log
- ❌ Lançar `Exception` para fluxos de negócio — use `Result<T>`
- ❌ Colocar Commands/Queries/Handlers — isso é Application Layer
- ❌ Implementar `IEventBus` — apenas a interface
- ❌ Acessar `HttpContext` ou dados de request diretamente

## Convenções de Nomenclatura — Domain

| Artefato | Convenção | Exemplo |
|---|---|---|
| Aggregate | PascalCase | `User`, `AuthSession`, `Tenant` |
| ValueObject | PascalCase | `Email`, `TenantId`, `Password` |
| DomainEvent | sufixo `Event` | `UserRegisteredEvent` |
| Errors | sufixo `Errors` | `UserErrors`, `TenantErrors` |
| Validator | sufixo `Validator` | `PasswordValidator` |
| Repository Interface | prefixo `I` + sufixo `Repository` | `IUserRepository` |
| Service Interface | prefixo `I` + sufixo `Service/Hasher` | `IPasswordHasher` |

---

# Infrastructure Layer — Avia.SSO.Zeus.Infrastructure

## Contexto

A camada de infraestrutura implementa todas as interfaces definidas no Domain e provê:
- Persistência com **PostgreSQL 16** via **Dapper**
- Segurança: hash de senha (PBKDF2), TOTP 2FA (Otp.NET), JWT (System.IdentityModel.Tokens.Jwt)
- Multitenancy via header HTTP `X-Tenant-Id`
- Event bus (stub in-memory, preparado para MassTransit/RabbitMQ)
- Registro de dependências via `AddInfrastructure()`

## Estrutura de Pastas — Infrastructure
```
Avia.SSO.Zeus.Infrastructure/
├── DependencyInjection/
│   └── InfrastructureServiceExtensions.cs  ← AddInfrastructure()
├── Messaging/
│   └── InMemoryEventBus.cs                 ← stub; trocar por MassTransit futuramente
├── Migrations/
│   └── schema.sql                          ← DDL PostgreSQL
├── Multitenancy/
│   └── HttpTenantContext.cs                ← lê header X-Tenant-Id
├── Persistence/
│   ├── IDbConnectionFactory.cs
│   ├── DbConnectionFactory.cs              ← cria NpgsqlConnection
│   └── Repositories/
│       ├── TenantRepository.cs
│       ├── UserRepository.cs
│       ├── RefreshTokenRepository.cs
│       └── AuthSessionRepository.cs
└── Security/
    ├── JwtSettings.cs                      ← POCO de configuração
    ├── PasswordHasher.cs                   ← PBKDF2/SHA-256
    ├── TwoFactorService.cs                 ← TOTP via Otp.NET
    └── TokenService.cs                     ← JWT access token + refresh token
```

## Packages — Infrastructure
```xml
<FrameworkReference Include="Microsoft.AspNetCore.App" />
<PackageReference Include="Dapper" Version="2.*" />
<PackageReference Include="Npgsql" Version="9.*" />
<PackageReference Include="Otp.NET" Version="1.*" />
<PackageReference Include="System.IdentityModel.Tokens.Jwt" Version="8.*" />
```

## Banco de Dados — PostgreSQL

### Configuração Local
| Item | Valor |
|---|---|
| Host | `localhost` |
| Port | `5432` |
| Database | `sso_zeus` |
| Username | `sso_zeus_user` |
| Password | `sso_zeus_pass` |

### Connection String
```json
"ConnectionStrings": {
  "Postgres": "Host=localhost;Port=5432;Database=sso_zeus;Username=sso_zeus_user;Password=sso_zeus_pass"
}
```

### Tabelas
| Tabela | Descrição |
|---|---|
| `tenants` | Tenants do sistema |
| `users` | Usuários por tenant |
| `login_attempts` | Histórico de tentativas de login |
| `refresh_tokens` | Tokens de refresh JWT |
| `auth_sessions` | Sessões autenticadas |

### Executar Migration
```bash
psql -h localhost -U sso_zeus_user -d sso_zeus -f src/Avia.SSO.Zeus.Infrastructure/Migrations/schema.sql
```

## Configuração JWT — appsettings
```json
"Jwt": {
  "SecretKey": "CHANGE_ME_USE_A_STRONG_SECRET_KEY_IN_PRODUCTION",
  "Issuer": "Avia.SSO.Zeus",
  "Audience": "Avia.SSO.Zeus.Clients",
  "ExpirationMinutes": 60
}
```

## Registro de Dependências
```csharp
// Program.cs
builder.Services.AddInfrastructure(builder.Configuration);
```

## Regras da Infrastructure

- Implementa interfaces do Domain — **nunca o contrário**
- Repositórios usam Dapper com **SQL explícito** — sem LINQ to SQL ou ORM
- `HttpTenantContext` lê o header `X-Tenant-Id` — obrigatório em todas as requisições multitenant
- `PasswordHasher` usa PBKDF2 com SHA-256, 100.000 iterações e comparação em tempo constante
- `InMemoryEventBus` é um stub — substituir por MassTransit quando mensageria for implementada

## O Que NÃO Fazer na Infrastructure

- ❌ Adicionar lógica de negócio — isso pertence ao Domain
- ❌ Referenciar diretamente `HttpContext` fora de `HttpTenantContext`
- ❌ Usar EF Core ou qualquer ORM com migrations automáticas
- ❌ Lançar `Exception` para fluxos esperados — retornar `Result<T>` ou `null`
