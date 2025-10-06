---
status: completed
parallelizable: true
blocked_by: ["1.0","4.0"]
completed_date: 2025-10-05
---

<task_context>
<domain>infra/observability</domain>
<type>integration</type>
<scope>configuration</scope>
<complexity>low</complexity>
<dependencies>logging|tracing</dependencies>
<unblocks>"15.0","17.0"</unblocks>
</task_context>

# Tarefa 14.0: Observabilidade (Serilog + OpenTelemetry, correlação API <-> MCP)

## Visão Geral
Concluir configuração de logs estruturados e tracing distribuído, adicionando correlation ids entre API e MCP.

## Requisitos
- Serilog com sinks adequados (console/arquivo JSON)
- OTel com propagação de contexto entre serviços
- Correlation/RequestId em responses

## Subtarefas
- [x] 14.1 Configurar propagadores e instrumentações ✅ CONCLUÍDA
- [x] 14.2 Correlacionar logs API e MCP ✅ CONCLUÍDA  
- [x] 14.3 Documentar troubleshooting observabilidade ✅ CONCLUÍDA
- [x] 14.4 Definição da tarefa, PRD e tech spec validados ✅ CONCLUÍDA
- [x] 14.5 Análise de regras e conformidade verificadas ✅ CONCLUÍDA
- [x] 14.6 Revisão de código completada ✅ CONCLUÍDA
- [x] 14.7 Pronto para deploy ✅ CONCLUÍDA

## Sequenciamento
- Bloqueado por: 1.0, 4.0
- Desbloqueia: 15.0, 17.0
- Paralelizável: Sim

## Detalhes de Implementação
TechSpec seção 8.

## Critérios de Sucesso
- ✅ Traces mostram fluxo MCP -> API -> DB
- ✅ Correlation IDs funcionando entre serviços
- ✅ Logs estruturados em JSON
- ✅ Documentação de troubleshooting criada
