---
status: pending
parallelizable: true
blocked_by: ["9.0","10.0","11.0","12.0","13.0","14.0"]
---

<task_context>
<domain>testing/e2e</domain>
<type>testing</type>
<scope>quality</scope>
<complexity>medium</complexity>
<dependencies>http_server|http_client|testing</dependencies>
<unblocks>"17.0"</unblocks>
</task_context>

# Tarefa 15.0: Testes E2E (Playwright)

## Visão Geral
Criar suíte E2E cobrindo login, criação de stack com markdown, filtros/busca e geração de token MCP no perfil.

## Requisitos
- Playwright configurado no frontend
- Fluxos principais do PRD cobertos

## Subtarefas
- [ ] 15.1 Cenário: login e criação de stack
- [ ] 15.2 Cenário: exploração por filtros/busca
- [ ] 15.3 Cenário: geração e revogação de token MCP

## Sequenciamento
- Bloqueado por: 9.0, 10.0, 11.0, 12.0, 13.0, 14.0
- Desbloqueia: 17.0
- Paralelizável: Sim (após features)

## Detalhes de Implementação
TechSpec seção 6 e PRD UX.

## Critérios de Sucesso
- Testes E2E passam em ambiente local e CI
