---
status: completed
parallelizable: true
blocked_by: ["9.0","10.0","11.0","12.0","13.0","14.0"]
completed_date: 2025-10-06
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
- [x] 15.1 Cenário: login e criação de stack ✅ CONCLUÍDA
- [x] 15.2 Cenário: exploração por filtros/busca ✅ CONCLUÍDA
- [x] 15.3 Cenário: geração e revogação de token MCP ✅ CONCLUÍDA
- [x] 15.4 Configuração Playwright e Page Objects ✅ CONCLUÍDA
- [x] 15.5 Setup CI/CD e documentação ✅ CONCLUÍDA
- [x] 15.6 Validação e testes locais ✅ CONCLUÍDA

## Sequenciamento
- Bloqueado por: 9.0, 10.0, 11.0, 12.0, 13.0, 14.0
- Desbloqueia: 17.0
- Paralelizável: Sim (após features)

## Detalhes de Implementação
TechSpec seção 6 e PRD UX.

## Critérios de Sucesso
- ✅ Playwright configurado com TypeScript
- ✅ Page Object Model implementado
- ✅ 36 testes E2E cobrindo fluxos principais
- ✅ Testes executam em 3 browsers (Chrome, Firefox, Safari)
- ✅ CI/CD pipeline configurado
- ✅ Documentação completa criada
- ✅ Todos os cenários do PRD implementados
