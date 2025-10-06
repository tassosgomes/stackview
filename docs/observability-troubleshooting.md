# Guia de Troubleshooting - Observabilidade StackShare

## Visão Geral

Este documento fornece orientações para troubleshooting e monitoramento da plataforma StackShare utilizando os recursos de observabilidade implementados com Serilog e OpenTelemetry.

## Componentes de Observabilidade

### 1. Logging Estruturado (Serilog)

#### Localização dos Logs

**API Backend:**
- Console: Logs estruturados com correlation ID em desenvolvimento
- Arquivo: `logs/api/stackshare-api-YYYYMMDD.json` (formato JSON compacto)

**MCP Server:**
- Console: Logs estruturados com correlation ID 
- Arquivo: `logs/mcp/stackshare-mcp-YYYYMMDD.json` (formato JSON compacto)

#### Propriedades dos Logs

Todos os logs incluem as seguintes propriedades enriquecidas:

```json
{
  "@t": "2025-10-05T14:30:00.123Z",
  "@l": "Information", 
  "@mt": "Processing request {Method} {Path}",
  "Method": "GET",
  "Path": "/api/stacks",
  "CorrelationId": "12345678-1234-1234-1234-123456789012",
  "Application": "StackShare.API",
  "Environment": "Development",
  "MachineName": "server-01"
}
```

### 2. Distributed Tracing (OpenTelemetry)

#### Instrumentações Ativas

**API Backend:**
- ASP.NET Core (requests HTTP)
- HttpClient (chamadas de saída)
- Entity Framework Core (queries de banco)

**MCP Server:**
- HttpClient (chamadas para API)
- Ferramentas MCP personalizadas

#### Tags de Trace Importantes

- `correlation_id`: ID de correlação entre serviços
- `mcp.tool`: Nome da ferramenta MCP executada
- `http.method`: Método HTTP
- `http.status_code`: Código de resposta HTTP
- `service.name`: Nome do serviço (StackShare.API ou StackShare.McpServer)

## Cenários de Troubleshooting

### 1. Rastreando Requisições End-to-End

**Problema:** Necessário rastrear uma requisição completa desde o MCP até o banco de dados.

**Solução:**
1. Identifique o `correlation_id` nos logs do MCP Server
2. Procure o mesmo `correlation_id` nos logs da API
3. Use o `trace_id` do OpenTelemetry para ver o span completo

**Exemplo de Busca nos Logs:**
```bash
# Buscar por correlation ID
grep "12345678-1234-1234-1234-123456789012" logs/api/stackshare-api-*.json
grep "12345678-1234-1234-1234-123456789012" logs/mcp/stackshare-mcp-*.json

# Buscar por trace ID no console (desenvolvimento)
docker-compose logs | grep "trace_id:abcdef123456"
```

### 2. Monitorando Performance das Ferramentas MCP

**Problema:** Ferramentas MCP estão lentas ou falhando.

**Indicadores a Verificar:**
- Duração dos spans de ferramentas MCP
- Erros HTTP nas chamadas API
- Timeouts de conexão

**Logs Relevantes:**
```json
{
  "@l": "Information",
  "@mt": "Buscando stacks - Search: {Search}, Type: {Type}",
  "Search": "React",
  "Type": "Frontend", 
  "CorrelationId": "...",
  "mcp.tool": "search_stacks"
}
```

### 3. Identificando Gargalos de Performance

**Problema:** API ou MCP Server com performance degradada.

**Métricas OpenTelemetry para Analisar:**
- `http.server.duration`: Tempo de resposta da API
- `http.client.duration`: Tempo de chamadas HTTP do MCP para API
- `db.statement.duration`: Tempo de queries do EF Core

**Queries SQL Lentas:**
Ative `SetDbStatementForText = true` no OpenTelemetry para ver as queries SQL nos traces.

### 4. Debugging de Erros de Correlação

**Problema:** Correlation ID não está propagando entre serviços.

**Verificações:**
1. Header `X-Correlation-ID` está sendo enviado pelo MCP Server
2. Middleware `CorrelationIdMiddleware` está ativo na API
3. `LogContext.PushProperty("CorrelationId", ...)` está sendo usado

**Exemplo de Debug:**
```csharp
// No MCP Server
_logger.LogDebug("Enviando request com Correlation-ID: {CorrelationId}", correlationId);

// Na API  
_logger.LogDebug("Recebido request com Correlation-ID: {CorrelationId}", context.TraceIdentifier);
```

## Comandos Úteis

### Análise de Logs

```bash
# Filtrar logs por nível
jq 'select(.["@l"] == "Error")' logs/api/stackshare-api-*.json

# Buscar por correlation ID específico
jq 'select(.CorrelationId == "12345678-1234-1234-1234-123456789012")' logs/**/*.json

# Contar erros por hora
jq -r '.["@t"]' logs/api/stackshare-api-*.json | cut -c1-13 | sort | uniq -c

# Ver todas as ferramentas MCP executadas
jq 'select(.["mcp.tool"]) | .["mcp.tool"]' logs/mcp/stackshare-mcp-*.json | sort | uniq -c
```

### Monitoramento em Tempo Real

```bash
# Seguir logs em tempo real
tail -f logs/api/stackshare-api-$(date +%Y%m%d).json | jq '.'
tail -f logs/mcp/stackshare-mcp-$(date +%Y%m%d).json | jq '.'

# Filtrar apenas erros em tempo real
tail -f logs/**/*.json | jq 'select(.["@l"] == "Error")'
```

## Configurações Recomendadas

### Desenvolvimento
```json
{
  "Serilog": {
    "MinimumLevel": {
      "Default": "Debug",
      "Override": {
        "Microsoft": "Information",
        "System": "Warning"
      }
    }
  }
}
```

### Produção
```json
{
  "Serilog": {
    "MinimumLevel": {
      "Default": "Information",
      "Override": {
        "Microsoft": "Warning",
        "System": "Warning"
      }
    }
  }
}
```

## Alertas Sugeridos

### 1. Taxa de Erro Alta
- **Condição:** Mais de 5% de requests com erro (status 5xx) em 5 minutos
- **Query:** `count(http_requests_total{status=~"5.."}) / count(http_requests_total) > 0.05`

### 2. Latência Alta
- **Condição:** P95 de tempo de resposta > 2 segundos
- **Query:** `histogram_quantile(0.95, http_request_duration_seconds) > 2`

### 3. MCP Tools Falhando
- **Condição:** Mais de 3 falhas de ferramentas MCP em 5 minutos
- **Log Pattern:** `@l="Error" AND mcp.tool=*`

## Integração com Ferramentas Externas

### Jaeger (Tracing)
```yaml
# docker-compose.yml
jaeger:
  image: jaegertracing/all-in-one:latest
  ports:
    - "16686:16686" # UI
    - "14268:14268" # HTTP collector
```

### Grafana + Loki (Logs)
```yaml
loki:
  image: grafana/loki:latest
  
grafana:
  image: grafana/grafana:latest
  ports:
    - "3000:3000"
```

### Prometheus (Métricas)
```yaml
prometheus:
  image: prom/prometheus:latest
  ports:
    - "9090:9090"
```

## Resolução de Problemas Comuns

### 1. Logs Não Aparecem
- Verificar permissões da pasta `logs/`
- Verificar configuração do Serilog
- Verificar se `UseSerilog()` está configurado

### 2. Correlation ID Não Funciona
- Verificar ordem dos middlewares
- Verificar se header HTTP está sendo propagado
- Verificar `LogContext.PushProperty()`

### 3. Traces Não Aparecem
- Verificar se `AddConsoleExporter()` está configurado
- Verificar se `ActivitySource` está registrado
- Verificar se instrumentações estão ativas

### 4. Performance Degradada
- Verificar se exporters não estão causando overhead
- Considerar sampling em produção
- Verificar se logs não estão muito verbosos

---

**Última Atualização:** 2025-10-05  
**Versão:** 1.0  
**Responsável:** Equipe de Desenvolvimento StackShare