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
- **Avia.SSO.Zeus.Application** — Camada de aplicação (use cases / handlers)
- **Avia.SSO.Zeus.Domain** — Camada de domínio (entidades, interfaces, regras de negócio)
- **Avia.SSO.Zeus.Infrastructure** — Camada de infraestrutura (persistência, serviços externos)
- **Avia.SSO.Zeus.Tests** — Testes unitários (MSTest)

## Referências entre Projetos
- **Api** → Application, Infrastructure
- **Application** → Domain
- **Infrastructure** → Application, Domain
- **Domain** → (nenhuma)
- **Tests** → Domain, Application, Infrastructure

## Convenções
- Idioma: English
- Nomenclatura: PascalCase para classes e métodos
- Projetos ficam em `src/`
