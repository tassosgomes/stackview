---
status: pending
parallelizable: false
blocked_by: ["1.0","2.0","3.0"]
---

<task_context>
<domain>engine/backend/stacks</domain>
<type>implementation</type>
<scope>core_feature</scope>
<complexity>high</complexity>
<dependencies>database|http_server</dependencies>
<unblocks>"8.0","11.0","13.0","15.0"</unblocks>
</task_context>

# Tarefa 4.0: Feature Stacks (Entidades, CQRS, CRUD, filtros, público/privado)

## Visão Geral
Implementar endpoints de CRUD de stacks com MediatR, incluindo filtros, paginação, visibilidade público/privado e histórico via StackHistory no PUT.

## Requisitos
- POST /api/stacks
- PUT /api/stacks/{id} (gera StackHistory)
- GET /api/stacks (filtros: type, technology, paginação)
- GET /api/stacks/{id}/history
- Respeitar visibilidade pública/privada

## Subtarefas
- [ ] 4.1 DTOs e validators (FluentValidation)
- [ ] 4.2 Handlers CQRS (Create/Update/List/GetHistory)
- [ ] 4.3 Regras de autorização por dono/privado
- [ ] 4.4 Paginação e filtros eficientes

## Sequenciamento
- Bloqueado por: 1.0, 2.0, 3.0
- Desbloqueia: 8.0, 11.0, 13.0, 15.0
- Paralelizável: Não (feature central)

## Detalhes de Implementação
Conforme seções 2, 3 e 4 da TechSpec.

## Critérios de Sucesso
- Suite de testes de integração verde para CRUD
- Histórico gravado no update
