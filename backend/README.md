# StackShare Backend

Este é o backend da aplicação StackShare, construído com .NET 8 usando uma arquitetura de Vertical Slice.

## Estrutura do Projeto

```
backend/
├── StackShare.sln                          # Arquivo de solução
└── src/
    ├── StackShare.API/                     # Camada de apresentação (Web API)
    │   ├── Controllers/                    # Controllers da API
    │   └── Program.cs                      # Entry point e configuração
    ├── StackShare.Application/             # Lógica de negócio e casos de uso
    │   └── Features/                       # Features organizadas por domínio
    ├── StackShare.Domain/                  # Entidades e regras de negócio
    │   ├── Entities/                       # Entidades do domínio
    │   └── Enums/                         # Enumerações
    └── StackShare.Infrastructure/          # Acesso a dados e serviços externos
        └── Data/                          # Contexto do EF Core e configurações
```

## Arquitetura

### Vertical Slice Architecture
O projeto utiliza a arquitetura Vertical Slice combinada com Clean Architecture:

- **StackShare.API**: Endpoints, middlewares, configuração da aplicação
- **StackShare.Application**: Handlers MediatR, DTOs, validações (FluentValidation)
- **StackShare.Domain**: Entidades, value objects, regras de negócio
- **StackShare.Infrastructure**: Implementações de repositórios, EF Core, serviços externos

### Tecnologias Configuradas

#### Logging (Serilog)
- **Console Logging**: Para desenvolvimento
- **File Logging**: JSON estruturado em `logs/stackshare-{data}.json`
- **Request Logging**: Middleware para log de requisições HTTP

#### Observabilidade (OpenTelemetry)
- **Tracing**: Instrumentação para ASP.NET Core e HttpClient
- **Console Exporter**: Para visualização de traces durante o desenvolvimento
- **Service Name**: `StackShare.API v1.0.0`

#### CQRS (MediatR)
- Separação entre Commands e Queries
- Handlers organizados por feature
- Dependency injection configurado

#### Pacotes Principais
- `MediatR` - CQRS pattern
- `FluentValidation` - Validação de requests
- `Npgsql.EntityFrameworkCore.PostgreSQL` - Acesso a dados PostgreSQL
- `Serilog.AspNetCore` - Logging estruturado
- `OpenTelemetry.Extensions.Hosting` - Observabilidade

## Como Executar

### Pré-requisitos
- .NET 8 SDK
- PostgreSQL (será configurado nas próximas tarefas)

### Executar em Desenvolvimento

```bash
cd backend
dotnet build
cd src/StackShare.API
dotnet run
```

A aplicação estará disponível em:
- **HTTP**: http://localhost:5095
- **HTTPS**: https://localhost:7095
- **OpenAPI**: http://localhost:5095/swagger (em desenvolvimento)

### Endpoints Disponíveis

- `GET /api/health` - Health check da aplicação

## Logs

### Localização
Os logs são salvos na pasta `logs/` dentro do projeto StackShare.API:
- Formato: `stackshare-{YYYYMMDD}.json`
- Retenção: 7 dias
- Formato: JSON estruturado (Compact JSON Formatter)

### Estrutura dos Logs
```json
{
  "@t": "2025-10-05T13:11:28.3545561Z",
  "@mt": "Health check requested",
  "@l": "Information",
  "SourceContext": "StackShare.API.Controllers.HealthController"
}
```

## Observabilidade

### Tracing
O OpenTelemetry está configurado para capturar:
- Requisições HTTP (ASP.NET Core)
- Chamadas HTTP de saída (HttpClient)
- Traces são exportados para o console em desenvolvimento

### Métricas
- Futuramente será configurado para exportar métricas de performance

## Próximos Passos

Esta tarefa estabeleceu as fundações do backend. As próximas tarefas incluem:

1. **Tarefa 2.0**: Configuração do EF Core e migrations para PostgreSQL
2. **Tarefa 3.0**: Implementação de autenticação com Identity + JWT
3. **Tarefa 4.0**: Feature de Stacks (CRUD completo)

## Comandos Úteis

```bash
# Build da solução
dotnet build

# Executar testes (quando implementados)
dotnet test

# Restore de pacotes
dotnet restore

# Limpar build
dotnet clean
```

---

**Tarefa 1.0 Completa** ✅
- ✅ Solução .NET criada com 4 projetos
- ✅ Referências entre projetos configuradas
- ✅ Pacotes base adicionados (MediatR, FluentValidation, EF Core, Serilog, OpenTelemetry)
- ✅ Serilog configurado (console + arquivo JSON)
- ✅ OpenTelemetry configurado (AspNetCore + HttpClient)
- ✅ Build funcionando
- ✅ API executando com logs estruturados
- ✅ Tracing básico ativo