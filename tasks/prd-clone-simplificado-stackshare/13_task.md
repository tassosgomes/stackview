---
status: completed
parallelizable: true
blocked_by: ["1.0","3.0","4.0","6.0","8.0"]
completed_date: 2025-10-05
---

<task_context>
<domain>engine/mcp-server</domain>
<type>integration</type>
<scope>core_feature</scope>
<complexity>medium</complexity>
<dependencies>http_client|security|mcp</dependencies>
<unblocks>"15.0","16.0"</unblocks>
</task_context>

# Tarefa 13.0: Servidor MCP (.NET Worker, MCP SDK, ferramentas search/get/list)

## Visão Geral
Criar worker service com MCP SDK expondo ferramentas: search_stacks, get_stack_details, list_technologies. Consumir API do backend com token de serviço.

## Requisitos
- Projeto Worker .NET 8 com ModelContextProtocol
- StackShareApiClient autenticado
- Ferramentas MCP: search_stacks, get_stack_details, list_technologies

## Subtarefas
- [x] 13.1 Scaffold Worker + dependências ✅ CONCLUÍDA
- [x] 13.2 HttpClient e autenticação de serviço ✅ CONCLUÍDA
- [x] 13.3 Implementar ferramentas e schemas ✅ CONCLUÍDA
- [x] 13.4 Testes básicos de integração local ✅ CONCLUÍDA

## Sequenciamento
- Bloqueado por: 1.0, 3.0, 4.0, 6.0, 8.0
- Desbloqueia: 15.0, 16.0
- Paralelizável: Sim

## Detalhes de Implementação
Seguir TechSpec seção Servidor MCP.

## Critérios de Sucesso
- Ferramentas MCP operam e retornam dados reais
