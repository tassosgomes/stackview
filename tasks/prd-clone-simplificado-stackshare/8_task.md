---
status: pending
parallelizable: true
blocked_by: ["1.0","2.0","4.0","7.0"]
---

<task_context>
<domain>testing/backend</domain>
<type>testing</type>
<scope>quality</scope>
<complexity>medium</complexity>
<dependencies>database|http_server|testing</dependencies>
<unblocks>"11.0","13.0","15.0"</unblocks>
</task_context>

# Tarefa 8.0: Testes Backend (unitários + integração com Testcontainers)

## Visão Geral
Criar suíte de testes unitários para handlers e integração para principais fluxos: criação de stack, geração de token MCP.

## Requisitos
- xUnit para unit tests
- WebApplicationFactory + Testcontainers para integração
- Cobrir cenários críticos da TechSpec

## Subtarefas
- [ ] 8.1 Testes unitários dos handlers de Stacks
- [ ] 8.2 Testes integração: criar stack
- [ ] 8.3 Testes integração: gerar e revogar token MCP

## Sequenciamento
- Bloqueado por: 1.0, 2.0, 4.0, 7.0
- Desbloqueia: 11.0, 13.0, 15.0
- Paralelizável: Sim

## Detalhes de Implementação
TechSpec seção 6.

## Critérios de Sucesso
- Testes rodam verdes localmente
