# SSO-Zeus-AI

## Stack
- .NET 10
- C#
- VSCode

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
│   │   ├── Class1.cs
│   │   └── Avia.SSO.Zeus.Domain.csproj
│   └── Avia.SSO.Zeus.Infrastructure/
│       ├── Class1.cs
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
// Common/BaseEntity.cs
public abstract class BaseEntity
{
    public Guid Id { get; protected set; }
    private readonly List<DomainEvent> _domainEvents = [];
    public IReadOnlyCollection<DomainEvent> DomainEvents => _domainEvents.AsReadOnly();
    protected void RaiseDomainEvent(DomainEvent domainEvent) => _domainEvents.Add(domainEvent);
    public void ClearDomainEvents() => _domainEvents.Clear();
}

// Common/AggregateRoot.cs
public abstract class AggregateRoot : BaseEntity { }
```

### Value Objects

- Sempre **imutáveis** (`record` ou `sealed class` com construtor privado)
- Validação dentro do próprio ValueObject via factory method `Create(...)`
- Retornam `Result<T>` — **nunca lançam exceptions**
```csharp
public sealed class Email : ValueObject
{
    public string Value { get; }
    private Email(string value) => Value = value;

    public static Result<Email> Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return Result.Failure<Email>(UserErrors.Email.Empty);
        if (!value.Contains('@'))
            return Result.Failure<Email>(UserErrors.Email.InvalidFormat);
        return Result.Success(new Email(value.ToLowerInvariant().Trim()));
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }
}
```

### Result Pattern
```csharp
// Shared/Result.cs
public class Result
{
    public bool IsSuccess { get; }
    public bool IsFailure => !IsSuccess;
    public Error Error { get; }

    protected Result(bool isSuccess, Error error) { ... }

    public static Result Success() => new(true, Error.None);
    public static Result Failure(Error error) => new(false, error);
    public static Result<T> Success<T>(T value) => new(value, true, Error.None);
    public static Result<T> Failure<T>(Error error) => new(default!, false, error);
}

public class Result<T> : Result
{
    public T Value { get; }
    protected internal Result(T value, bool isSuccess, Error error)
        : base(isSuccess, error) => Value = value;
}
```

### Errors (tipados por contexto)
```csharp
// Identity/Errors/UserErrors.cs
public static class UserErrors
{
    public static class Email
    {
        public static readonly Error Empty =
            new("User.Email.Empty", "Email cannot be empty.", ErrorType.Validation);
        public static readonly Error InvalidFormat =
            new("User.Email.InvalidFormat", "Invalid email format.", ErrorType.Validation);
        public static readonly Error AlreadyInUse =
            new("User.Email.AlreadyInUse", "Email already in use.", ErrorType.Conflict);
    }

    public static class Password
    {
        public static readonly Error TooShort =
            new("User.Password.TooShort", "Password must be at least 8 characters.", ErrorType.Validation);
        public static readonly Error NoUpperCase =
            new("User.Password.NoUpperCase", "Password must contain at least one uppercase letter.", ErrorType.Validation);
        public static readonly Error NoSpecialChar =
            new("User.Password.NoSpecialChar", "Password must contain at least one special character.", ErrorType.Validation);
    }

    public static readonly Error NotFound =
        new("User.NotFound", "User not found.", ErrorType.NotFound);
    public static readonly Error LockedOut =
        new("User.LockedOut", "Account locked due to too many failed attempts.", ErrorType.Forbidden);
    public static readonly Error InvalidCredentials =
        new("User.InvalidCredentials", "Invalid credentials.", ErrorType.Unauthorized);
    public static readonly Error TwoFactorRequired =
        new("User.TwoFactorRequired", "Two-factor authentication required.", ErrorType.Unauthorized);
    public static readonly Error TwoFactorInvalid =
        new("User.TwoFactorInvalid", "Invalid or expired 2FA code.", ErrorType.Unauthorized);
}
```

### Validators (FluentValidation)

Usado apenas para validações **estruturais e reutilizáveis**.
Validações de **regras de negócio** (unicidade, lockout) ficam no Aggregate via `Result`.
```csharp
// Identity/Validators/PasswordValidator.cs
public class PasswordValidator : AbstractValidator<string>
{
    public PasswordValidator()
    {
        RuleFor(p => p)
            .NotEmpty().WithErrorCode(UserErrors.Password.TooShort.Code)
            .MinimumLength(8).WithErrorCode(UserErrors.Password.TooShort.Code)
            .Matches("[A-Z]").WithErrorCode(UserErrors.Password.NoUpperCase.Code)
            .Matches("[^a-zA-Z0-9]").WithErrorCode(UserErrors.Password.NoSpecialChar.Code);
    }
}
```

### Aggregate User

O agregado `User` encapsula:
- `TenantId` — multitenant, cada usuário pertence a um tenant
- `Email` (ValueObject)
- `Password` — hash + salt como ValueObject, **nunca string pura**
- `TwoFactorSecret` (ValueObject opcional)
- `LoginAttempts` — lista de `LoginAttempt`
- `Status` — enum `UserStatus`: Active, Locked, Deactivated
- Lockout automático após **5 tentativas falhas consecutivas**
- Métodos: `Register`, `ChangePassword`, `EnableTwoFactor`, `VerifyTwoFactor`,
  `RecordLoginAttempt`, `Unlock`, `Deactivate`
- Cada método retorna `Result` e levanta `DomainEvent`

### Multitenancy

- `ITenantContext` expõe o `TenantId` corrente (implementado na Infrastructure via HttpContext/header)
- Todo Aggregate com escopo de tenant recebe `TenantId` no construtor
- **Nunca filtre por tenant na camada de domínio** — responsabilidade dos repositórios (Infrastructure)

### Domain Events
```csharp
// Common/DomainEvent.cs
public abstract record DomainEvent(Guid Id, DateTime OccurredAt)
{
    protected DomainEvent() : this(Guid.NewGuid(), DateTime.UtcNow) { }
}

// Exemplo
public sealed record UserRegisteredEvent(
    Guid UserId,
    Guid TenantId,
    string Email) : DomainEvent;
```

### Separação de Eventos — Domain vs Integration

| Tipo | Localização | Finalidade |
|---|---|---|
| `DomainEvent` | `Domain/Common` | Consistência interna do bounded context |
| `IIntegrationEvent` | `Domain/Messaging` | Publicado para outros serviços via broker (RabbitMQ/MassTransit) |

`IEventBus` fica no domínio como interface. Implementação fica na Infrastructure.

### Repositórios — somente interfaces no Domain
```csharp
public interface IUserRepository
{
    Task<User?> GetByIdAsync(UserId id, CancellationToken ct = default);
    Task<User?> GetByEmailAndTenantAsync(Email email, TenantId tenantId, CancellationToken ct = default);
    Task<bool> EmailExistsInTenantAsync(Email email, TenantId tenantId, CancellationToken ct = default);
    Task AddAsync(User user, CancellationToken ct = default);
    Task UpdateAsync(User user, CancellationToken ct = default);
}
```

## Package Permitido no Domain
```xml
<PackageReference Include="FluentValidation" Version="11.*" />
```

Nenhum outro package externo. O domínio deve ser autocontido.

## O Que NÃO Fazer no Domain

- ❌ Referenciar `Microsoft.EntityFrameworkCore`
- ❌ Usar `ILogger` ou qualquer infraestrutura de log
- ❌ Lançar `Exception` para fluxos de negócio — use `Result<T>`
- ❌ Colocar Commands/Queries/Handlers — isso é Application Layer
- ❌ Implementar `IEventBus` — apenas a interface
- ❌ Acessar `HttpContext` ou dados de request diretamente

## Ordem de Criação dos Arquivos

Siga esta ordem para evitar dependências circulares:

1. `Shared/` → Result, Error, ErrorType, ITenantContext
2. `Common/` → BaseEntity, AggregateRoot, ValueObject, DomainEvent
3. `Multitenancy/` → ValueObjects → Entities → Events → Errors → Validators → Repositories
4. `Identity/` → ValueObjects → Aggregates → Entities → Enums → Events → Errors → Validators → Repositories → Services
5. `Session/` → ValueObjects → Aggregates → Events → Errors → Repositories
6. `Messaging/` → IEventBus, IIntegrationEvent

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