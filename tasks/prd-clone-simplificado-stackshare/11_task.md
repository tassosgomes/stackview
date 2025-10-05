---
status: pending
parallelizable: true
blocked_by: ["9.0","4.0","5.0","8.0","10.0"]
---

<task_context>
<domain>engine/frontend/stacks</domain>
<type>implementation</type>
<scope>core_feature</scope>
<complexity>high</complexity>
<dependencies>http_client|ui_framework|markdown</dependencies>
<unblocks>"15.0"</unblocks>
</task_context>

# Tarefa 11.0: Frontend Stacks (CRUD, Markdown, filtros e busca)

## Visão Geral
Implementar páginas de criação/edição de stacks com editor Markdown, listagem pública com filtros e busca por tecnologia.

## Requisitos
- Form de criar/editar stack (nome, tipo, tecnologias, markdown, público/privado)
- Listagem com filtros por tipo e tecnologia
- Busca por tecnologia/nome/descrição
- Visualização pública de stacks

## Subtarefas
- [ ] 11.1 Form e UI com validação
- [ ] 11.2 Editor Markdown (react-markdown + textarea)
- [ ] 11.3 Listagem e filtros + paginação
- [ ] 11.4 View pública detalhada

## Sequenciamento
- Bloqueado por: 9.0, 4.0, 5.0, 8.0, 10.0
- Desbloqueia: 15.0
- Paralelizável: Sim (após back pronto)

## Detalhes de Implementação
Conforme PRD e TechSpec (seções 3 e 6).

## Critérios de Sucesso
- Fluxo criar/editar/listar funciona; filtros operam
