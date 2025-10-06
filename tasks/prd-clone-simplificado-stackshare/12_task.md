---
status: completed
parallelizable: true
blocked_by: ["9.0","6.0","10.0"]
completed_at: 2025-10-05
---

<task_context>
<domain>engine/frontend/mcp-tokens</domain>
<type>implementation</type>
<scope>core_feature</scope>
<complexity>low</complexity>
<dependencies>http_client|ui_framework|security</dependencies>
<unblocks>"13.0","15.0"</unblocks>
</task_context>

# Tarefa 12.0: Frontend Tokens MCP (perfil: gerar/revogar)

## Visão Geral
Implementar seção "Acesso IA" no perfil do usuário para gerar e revogar tokens, exibindo o raw token uma única vez.

## Requisitos
- Tela de perfil com listagem de tokens
- Ação de gerar novo token (mostrar raw token)
- Ação de revogar token

## Subtarefas
- [x] 12.1 Componente de perfil e listagem ✅ CONCLUÍDA
- [x] 12.2 Fluxo de geração (modal com cópia do token) ✅ CONCLUÍDA
- [x] 12.3 Fluxo de revogação ✅ CONCLUÍDA

## Sequenciamento
- Bloqueado por: 9.0, 6.0, 10.0
- Desbloqueia: 13.0, 15.0
- Paralelizável: Sim

## Detalhes de Implementação
Conforme PRD seção 5 e TechSpec 4.

## Critérios de Sucesso
- Usuário consegue gerar/revogar tokens via UI
