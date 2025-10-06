# Teste de Observabilidade - Task 14.0

## Resumo da Implementação

A tarefa 14.0 implementou observabilidade completa no sistema StackShare com:

### ✅ 1. Serilog Aprimorado

**API Backend (`StackShare.API`):**
- Logs estruturados em JSON com enrichers
- Correlation ID em todos os logs
- Configuração separada para console (desenvolvimento) e arquivo JSON
- Localização: `logs/api/stackshare-api-YYYYMMDD.json`

**MCP Server (`StackShare.McpServer`):**
- Logs estruturados sincronizados com a API
- Correlation ID propagado via headers HTTP
- Localização: `logs/mcp/stackshare-mcp-YYYYMMDD.json`

### ✅ 2. OpenTelemetry Distribuído 

**API Backend:**
- Instrumentação ASP.NET Core (requests HTTP)
- Instrumentação HttpClient (calls de saída)
- Instrumentação Entity Framework Core (queries DB)
- Tags customizadas com correlation ID

**MCP Server:**
- Instrumentação HttpClient para chamadas à API
- Tracing customizado das ferramentas MCP (`search_stacks`, `get_stack_details`, `list_technologies`)
- Propagação de contexto via headers

### ✅ 3. Correlation ID entre Serviços

**Middleware `CorrelationIdMiddleware`:**
- Adiciona header `X-Correlation-ID` em todos os responses
- Usa TraceIdentifier do request ou gera novo GUID
- Enriquece logs com correlation ID
- Adiciona tags ao trace do OpenTelemetry

**Propagação MCP → API:**
- MCP Server adiciona header `X-Correlation-ID` nas chamadas
- API reconhece e propaga o correlation ID
- Logs de ambos os serviços ficam correlacionados

### ✅ 4. Documentação Completa

- **Guia de Troubleshooting:** `docs/observability-troubleshooting.md`
- Comandos para análise de logs
- Cenários de debugging end-to-end
- Configurações recomendadas para dev/prod
- Integração com ferramentas externas (Jaeger, Grafana, Prometheus)

## Como Testar

### 1. Executar os Serviços

```bash
# Terminal 1 - API
cd backend/src/StackShare.API
dotnet run

# Terminal 2 - MCP Server  
cd backend/src/StackShare.McpServer
dotnet run
```

### 2. Executar Ferramenta MCP

No Claude Desktop ou através do cliente MCP:
```json
{
  "tool": "search_stacks", 
  "parameters": {
    "search": "React",
    "type": "Frontend"
  }
}
```

### 3. Verificar Logs Correlacionados

```bash
# Ver logs da API
tail -f logs/api/stackshare-api-*.json | jq '.'

# Ver logs do MCP Server
tail -f logs/mcp/stackshare-mcp-*.json | jq '.'

# Buscar por correlation ID específico
grep "correlation-id-aqui" logs/**/*.json
```

### 4. Analisar Traces OpenTelemetry

Os traces aparecem no console durante execução mostrando:
- Span da ferramenta MCP (`search_stacks`)
- Span da chamada HTTP (MCP → API) 
- Span da query Entity Framework (API → DB)

## Fluxo de Trace Completo

```
MCP Tool: search_stacks
├── HTTP Call: GET /api/stacks
│   ├── Database Query: SELECT * FROM Stacks
│   ├── Database Query: SELECT * FROM Technologies  
│   └── Response: 200 OK
└── MCP Response: JSON with stacks
```

## Critérios de Sucesso Atendidos

✅ **Serilog com sinks adequados:** Console + arquivo JSON  
✅ **OpenTelemetry com propagação:** Entre API e MCP Server  
✅ **Correlation ID em responses:** Header `X-Correlation-ID`  
✅ **Traces mostram fluxo MCP → API → DB:** Via instrumentações  
✅ **Documentação troubleshooting:** Guia completo criado  

## Arquivos Modificados/Criados

```
✅ backend/src/StackShare.API/Program.cs              # OpenTelemetry + Serilog + Middleware
✅ backend/src/StackShare.API/Middleware/
   └── CorrelationIdMiddleware.cs                      # Novo - Correlation ID
✅ backend/src/StackShare.McpServer/Program.cs        # OpenTelemetry
✅ backend/src/StackShare.McpServer/appsettings.json  # Serilog estruturado
✅ backend/src/StackShare.McpServer/Services/
   └── StackShareApiClient.cs                         # Propagação correlation ID
✅ backend/src/StackShare.McpServer/Tools/
   └── StackShareTools.cs                             # Tracing customizado
✅ docs/observability-troubleshooting.md              # Novo - Documentação
```

## Dependências Adicionadas

**API:**
- `OpenTelemetry.Instrumentation.EntityFrameworkCore` (1.0.0-beta.12)
- `Serilog.Enrichers.Environment` (3.0.1)

**MCP Server:**
- `OpenTelemetry.Extensions.Hosting` (1.13.0)
- `OpenTelemetry.Instrumentation.Http` (1.12.0) 
- `OpenTelemetry.Exporter.Console` (1.13.0)
- `Serilog.Formatting.Compact` (3.0.0)

---

**Status:** ✅ IMPLEMENTAÇÃO COMPLETA  
**Data:** 2025-10-05  
**Branch:** `feat/task-14-observability`