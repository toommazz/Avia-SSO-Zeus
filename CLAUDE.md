# SSO-Zeus-AI

## Stack
- .NET 10
- C#
- VSCode
- PostgreSQL 16 + Dapper
- MediatR 12 (CQRS)
- MSTest + Moq (testes)

## Estrutura
```
sso-zeus-ai/
├── .gitignore
├── CLAUDE.md
├── SSO-Zeus-AI.slnx
├── src/
│   ├── Avia.SSO.Zeus.Api/
│   │   ├── Controllers/
│   │   │   ├── ApiController.cs
│   │   │   ├── AuthController.cs
│   │   │   ├── TenantsController.cs
│   │   │   └── UsersController.cs
│   │   ├── Properties/
│   │   │   └── launchSettings.json
│   │   ├── appsettings.json
│   │   ├── appsettings.Development.json
│   │   ├── Program.cs
│   │   ├── Avia.SSO.Zeus.Api.http
│   │   └── Avia.SSO.Zeus.Api.csproj
│   ├── Avia.SSO.Zeus.Application/
│   │   ├── Common/
│   │   │   ├── Behaviors/
│   │   │   │   └── ValidationBehavior.cs
│   │   │   └── DTOs/
│   │   │       ├── AuthTokenDto.cs
│   │   │       ├── TenantDto.cs
│   │   │       └── UserDto.cs
│   │   ├── DependencyInjection/
│   │   │   └── ApplicationServiceExtensions.cs
│   │   ├── Identity/
│   │   │   ├── Commands/
│   │   │   │   ├── ChangePassword/
│   │   │   │   ├── EnableTwoFactor/
│   │   │   │   ├── Login/
│   │   │   │   ├── RefreshToken/
│   │   │   │   ├── Register/
│   │   │   │   └── VerifyTwoFactor/
│   │   │   └── Queries/
│   │   │       └── GetUser/
│   │   ├── Multitenancy/
│   │   │   └── Commands/
│   │   │       └── CreateTenant/
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
        ├── Application/
        │   ├── Identity/
        │   │   ├── LoginCommandHandlerTests.cs
        │   │   └── RegisterUserCommandHandlerTests.cs
        │   └── Multitenancy/
        │       └── CreateTenantCommandHandlerTests.cs
        ├── Domain/
        │   ├── Identity/
        │   │   ├── Aggregates/UserTests.cs
        │   │   ├── Entities/RefreshTokenTests.cs
        │   │   └── ValueObjects/
        │   │       ├── EmailTests.cs
        │   │       ├── PasswordTests.cs
        │   │       └── UserIdTests.cs
        │   ├── Multitenancy/TenantTests.cs
        │   ├── Session/AuthSessionTests.cs
        │   └── Shared/ResultTests.cs
        ├── Infrastructure/
        │   └── Security/PasswordHasherTests.cs
        ├── MSTestSettings.cs
        └── Avia.SSO.Zeus.Tests.csproj
```

## Projetos
- **Avia.SSO.Zeus.Api** — ASP.NET Core Web API (.NET 10)
- **Avia.SSO.Zeus.Application** — Camada de aplicação (use cases / handlers CQRS via MediatR)
- **Avia.SSO.Zeus.Domain** — Camada de domínio (entidades, interfaces, regras de negócio)
- **Avia.SSO.Zeus.Infrastructure** — Camada de infraestrutura (persistência, serviços externos, mensageria)
- **Avia.SSO.Zeus.Tests** — Testes unitários (MSTest + Moq)

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
│   ├── Entities/Tenant.cs
│   ├── ValueObjects/TenantId.cs, TenantName.cs, TenantSettings.cs
│   ├── Events/TenantCreatedEvent.cs, TenantDeactivatedEvent.cs
│   ├── Errors/TenantErrors.cs
│   ├── Validators/TenantValidator.cs
│   └── Repositories/ITenantRepository.cs
│
├── Identity/
│   ├── Aggregates/User.cs             ← AggregateRoot principal
│   ├── Entities/RefreshToken.cs (UserId, Token, ExpiresAt, IsRevoked), TwoFactorToken.cs, LoginAttempt.cs
│   ├── ValueObjects/UserId.cs, Email.cs, Password.cs, PhoneNumber.cs, TwoFactorSecret.cs
│   ├── Enums/UserStatus.cs, TwoFactorMethod.cs, LoginFailureReason.cs
│   ├── Events/UserRegisteredEvent.cs, UserLoginSucceededEvent.cs, UserLoginFailedEvent.cs,
│   │         UserLockedOutEvent.cs, TwoFactorRequestedEvent.cs, TwoFactorVerifiedEvent.cs,
│   │         PasswordChangedEvent.cs, UserDeactivatedEvent.cs
│   ├── Errors/UserErrors.cs
│   ├── Validators/UserValidator.cs, EmailValidator.cs, PasswordValidator.cs
│   ├── Repositories/IUserRepository.cs, IRefreshTokenRepository.cs
│   └── Services/IPasswordHasher.cs, ITwoFactorService.cs, ITokenService.cs
│
├── Session/
│   ├── Aggregates/AuthSession.cs
│   ├── ValueObjects/SessionId.cs, DeviceInfo.cs
│   ├── Events/SessionCreatedEvent.cs, SessionRevokedEvent.cs
│   ├── Errors/SessionErrors.cs
│   └── Repositories/IAuthSessionRepository.cs
│
├── Messaging/
│   ├── IEventBus.cs
│   └── IIntegrationEvent.cs
│
└── Shared/
    ├── Result.cs                      ← Result<T> pattern
    ├── Error.cs
    ├── ErrorType.cs
    └── ITenantContext.cs
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

- Sempre **imutáveis** (`sealed class` com construtor privado)
- Validação via factory method `Create(...)` que retorna `Result<T>`
- **Nunca lançam exceptions**

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
- Métodos: `Register`, `Reconstitute`, `ChangePassword`, `EnableTwoFactor`, `RecordLoginAttempt`, `Unlock`, `Deactivate`
- Cada método retorna `Result` e levanta `DomainEvent`
- **`Register`** — cria novo usuário (gera novo `UserId`)
- **`Reconstitute`** — reconstrói usuário a partir do banco (preserva o `UserId` persistido); usado exclusivamente pelos repositórios

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

---

# Application Layer — Avia.SSO.Zeus.Application

## Contexto

Camada de aplicação responsável pelos casos de uso do sistema via **CQRS com MediatR**.
Orquestra o Domain sem conter regras de negócio.

## Estrutura de Pastas — Application
```
Avia.SSO.Zeus.Application/
├── Common/
│   ├── Behaviors/
│   │   └── ValidationBehavior.cs       ← pipeline MediatR que executa FluentValidation
│   └── DTOs/
│       ├── AuthTokenDto.cs             ← AccessToken, RefreshToken, ExpiresInMinutes
│       ├── TenantDto.cs
│       └── UserDto.cs
├── DependencyInjection/
│   └── ApplicationServiceExtensions.cs ← AddApplication()
├── Identity/
│   ├── Commands/
│   │   ├── Register/                   ← RegisterUserCommand + Handler + Validator
│   │   ├── Login/                      ← LoginCommand + Handler + Validator
│   │   ├── RefreshToken/               ← RefreshTokenCommand + Handler + Validator
│   │   ├── ChangePassword/             ← ChangePasswordCommand + Handler + Validator
│   │   ├── EnableTwoFactor/            ← EnableTwoFactorCommand + Handler + Validator
│   │   └── VerifyTwoFactor/            ← VerifyTwoFactorCommand + Handler + Validator
│   └── Queries/
│       └── GetUser/                    ← GetUserQuery + Handler
└── Multitenancy/
    └── Commands/
        └── CreateTenant/               ← CreateTenantCommand + Handler + Validator
```

## Packages — Application
```xml
<PackageReference Include="MediatR" Version="12.*" />
<PackageReference Include="FluentValidation" Version="11.*" />
<PackageReference Include="FluentValidation.DependencyInjectionExtensions" Version="11.*" />
```

## Registro de Dependências
```csharp
// Program.cs
builder.Services.AddApplication();
```

## Regras da Application

- Handlers recebem Command/Query e retornam `Result<T>`
- **Nunca lança exceptions** para fluxos esperados — usa `Result<T>`
- Validação de entrada via `FluentValidation` (estrutural) — regras de negócio ficam no Domain
- `ValidationBehavior` executa validadores automaticamente antes de cada handler
- Não acessa banco de dados diretamente — usa interfaces do Domain
- Não conhece Infrastructure — depende apenas de Domain

## Convenções — Application

| Artefato | Convenção | Exemplo |
|---|---|---|
| Command | sufixo `Command` | `RegisterUserCommand` |
| Query | sufixo `Query` | `GetUserQuery` |
| Handler | sufixo `CommandHandler` / `QueryHandler` | `RegisterUserCommandHandler` |
| Validator | sufixo `CommandValidator` | `RegisterUserCommandValidator` |
| DTO | sufixo `Dto` | `UserDto`, `AuthTokenDto` |

---

# Infrastructure Layer — Avia.SSO.Zeus.Infrastructure

## Contexto

Implementa todas as interfaces definidas no Domain e provê:
- Persistência com **PostgreSQL 16** via **Dapper**
- Segurança: hash de senha (PBKDF2), TOTP 2FA (Otp.NET), JWT (System.IdentityModel.Tokens.Jwt)
- Multitenancy via header HTTP `X-Tenant-Id`
- Event bus (stub in-memory, preparado para MassTransit/RabbitMQ)

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
│   ├── DbConnectionFactory.cs
│   └── Repositories/
│       ├── TenantRepository.cs
│       ├── UserRepository.cs
│       ├── RefreshTokenRepository.cs
│       └── AuthSessionRepository.cs
└── Security/
    ├── JwtSettings.cs
    ├── PasswordHasher.cs                   ← PBKDF2/SHA-256, 100k iterações
    ├── TwoFactorService.cs                 ← TOTP via Otp.NET
    └── TokenService.cs                     ← JWT + refresh token
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

## O Que NÃO Fazer na Infrastructure

- ❌ Adicionar lógica de negócio — isso pertence ao Domain
- ❌ Referenciar diretamente `HttpContext` fora de `HttpTenantContext`
- ❌ Usar EF Core ou qualquer ORM com migrations automáticas
- ❌ Lançar `Exception` para fluxos esperados

---

# Api Layer — Avia.SSO.Zeus.Api

## Endpoints

| Controller | Método | Rota | Acesso | Descrição |
|---|---|---|---|---|
| `AuthController` | POST | `/api/auth/register` | Público | Registra novo usuário |
| `AuthController` | POST | `/api/auth/login` | Público | Autentica usuário |
| `AuthController` | POST | `/api/auth/refresh-token` | Público | Renova access token |
| `AuthController` | POST | `/api/auth/verify-two-factor` | Público | Verifica código 2FA |
| `TenantsController` | POST | `/api/tenants` | `[Authorize]` | Cria novo tenant |
| `UsersController` | GET | `/api/users/{id}` | `[Authorize]` | Retorna usuário por ID |
| `UsersController` | PUT | `/api/users/{id}/password` | `[Authorize]` | Altera senha |
| `UsersController` | POST | `/api/users/{id}/two-factor` | `[Authorize]` | Habilita 2FA — retorna secret TOTP |

## Headers Obrigatórios

| Header | Endpoints | Descrição |
|---|---|---|
| `X-Tenant-Id` | `/api/auth/login`, `/api/auth/register` | GUID do tenant do usuário |
| `Authorization: Bearer {token}` | `/api/users/*` | JWT de acesso |

## Padrão de Resposta

- **200 OK** — sucesso com body
- **204 No Content** — sucesso sem body
- **400 Bad Request** — erro de validação
- **401 Unauthorized** — credenciais inválidas / token expirado
- **403 Forbidden** — conta bloqueada
- **404 Not Found** — recurso não encontrado
- **409 Conflict** — recurso duplicado

## Packages — Api
```xml
<PackageReference Include="Swashbuckle.AspNetCore" Version="6.*" />
<PackageReference Include="Microsoft.AspNetCore.Authentication.JwtBearer" Version="8.*" />
```

## Registro de Dependências — Program.cs
```csharp
builder.Services.AddControllers()
    .AddJsonOptions(options =>
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

// JWT Authentication
builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = ...,
            ValidAudience = ...,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey))
        };
    });
builder.Services.AddAuthorization();

// Middleware pipeline
app.UseAuthentication();  // antes de UseAuthorization
app.UseAuthorization();
```

## Autorização
- `AuthController` — `[AllowAnonymous]` em toda a controller (rotas públicas)
- `UsersController` — `[Authorize]` em toda a controller (JWT obrigatório)
- `TenantsController` — `[Authorize]` em toda a controller (JWT obrigatório)
- Token inválido, expirado ou ausente → **401 Unauthorized**

## Swagger
- Disponível em `/swagger` no ambiente Development
- Suporte a autenticação Bearer JWT configurado no SwaggerGen
- `JsonStringEnumConverter` registrado globalmente para deserializar enums como strings (ex: `TwoFactorMethod`)

## Exception Handler Global
```csharp
app.UseExceptionHandler(errApp => errApp.Run(async ctx =>
{
    // ValidationException → 400 com lista de erros
    // Outras exceptions → 500
}));
```
Captura `FluentValidation.ValidationException` lançada pelo `ValidationBehavior` e retorna 400 com detalhes dos campos inválidos.

---

# Decisões Técnicas e Correções Aplicadas

## Mapeamento Dapper snake_case → PascalCase
```csharp
// InfrastructureServiceExtensions.cs
DefaultTypeMap.MatchNamesWithUnderscores = true;
```
Necessário porque o PostgreSQL usa `snake_case` e as classes C# usam `PascalCase`.

## User.Reconstitute vs User.Register
- `User.Register(...)` — cria novo usuário, gera novo `UserId` com `UserId.New()`
- `User.Reconstitute(...)` — reconstrói a partir do banco, preserva o `UserId` existente
- `UserRepository.UserRow.ToDomain()` **sempre** chama `Reconstitute`, nunca `Register`

## RefreshToken.UserId
- `RefreshToken` armazena `UserId` (FK para `users.id`)
- `RefreshToken.Create(Guid userId, string token, DateTime expiresAt)`
- `RefreshTokenCommandHandler` usa `refreshToken.UserId` para buscar o usuário associado

## TenantId no Login
- `LoginCommand.TenantId` pode vir do corpo JSON **ou** do header `X-Tenant-Id`
- `AuthController.Login` extrai o header `X-Tenant-Id` e sobrescreve o campo do command

## JsonStringEnumConverter
- Registrado globalmente em `Program.cs` via `AddJsonOptions`
- Necessário para deserializar `TwoFactorMethod` ("Totp", "Sms", "None") a partir do JSON

## Autenticação JWT (JwtBearer)
- Package: `Microsoft.AspNetCore.Authentication.JwtBearer` versão `8.*`
- A mesma `SecretKey` da seção `Jwt` do `appsettings.json` é usada para **gerar** (TokenService) e **validar** (JwtBearer middleware) os tokens
- `UseAuthentication()` deve vir **antes** de `UseAuthorization()` no pipeline
- Rotas públicas usam `[AllowAnonymous]`; rotas protegidas usam `[Authorize]`

## EnableTwoFactor retorna o secret TOTP
- `POST /api/users/{id}/two-factor` retorna HTTP 200 com o secret Base32 em texto
- O cliente deve exibir o secret (ou QR Code) para o usuário configurar no app autenticador (Google Authenticator, Authy, etc.)
- Após configurar, o usuário usa `POST /api/auth/verify-two-factor` com o código TOTP

---

# Tests — Avia.SSO.Zeus.Tests

## Cobertura Atual — 59 testes

| Área | Arquivo | Testes |
|---|---|---|
| Domain/Shared | `ResultTests` | 4 |
| Domain/Identity/ValueObjects | `EmailTests`, `PasswordTests`, `UserIdTests` | 13 |
| Domain/Identity/Aggregates | `UserTests` | 11 |
| Domain/Identity/Entities | `RefreshTokenTests` | 3 |
| Domain/Multitenancy | `TenantTests` | 6 |
| Domain/Session | `AuthSessionTests` | 4 |
| Infrastructure/Security | `PasswordHasherTests` | 5 |
| Application/Identity | `RegisterUserCommandHandlerTests`, `LoginCommandHandlerTests` | 8 |
| Application/Multitenancy | `CreateTenantCommandHandlerTests` | 3 |

## Packages — Tests
```xml
<PackageReference Include="MSTest" Version="4.*" />
<PackageReference Include="Moq" Version="4.*" />
```
    
## Convenções de Teste

- Nomenclatura: `MethodName_Scenario_ExpectedResult`
- Mocks via **Moq** para dependências externas (repositórios, serviços)
- Testes de domínio sem mocks — instanciar agregados diretamente
- Testes de handler isolam apenas as dependências da camada Application
