---
status: pending
parallelizable: true
blocked_by: ["1.0","2.0","3.0"]
---

<task_context>
<domain>engine/backend/mcp-tokens</domain>
<type>implementation</type>
<scope>core_feature</scope>
<complexity>medium</complexity>
<dependencies>database|http_server|security</dependencies>
<unblocks>"12.0","13.0"</unblocks>
</task_context>

# Tarefa 6.0: Tokens MCP do Usuário (gerar, listar, revogar, hash)

## Visão Geral
Implementar endpoints para gerar tokens MCP (retornar raw token uma única vez), listar e revogar. Persistir apenas hash.

## Requisitos
- POST /api/users/me/mcp-tokens (retorna RawToken uma vez)
- DELETE /api/users/me/mcp-tokens/{id}
- Persistência: hash com sal; flag IsRevoked

## Subtarefas
- [ ] 6.1 Entidade e repositório McpApiToken
- [ ] 6.2 Geração de token seguro e hashing
- [ ] 6.3 Endpoints protegidos do usuário logado

## Sequenciamento
- Bloqueado por: 1.0, 2.0, 3.0
- Desbloqueia: 12.0, 13.0
- Paralelizável: Sim

## Detalhes de Implementação
Ver TechSpec seção 3 e 4.

## Critérios de Sucesso
- Token é retornado uma vez e não reaparece
- Revogação marca IsRevoked = true
