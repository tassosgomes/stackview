---
status: pending
parallelizable: true
blocked_by: ["8.0","15.0","16.0"]
---

<task_context>
<domain>infra/ci-cd</domain>
<type>integration</type>
<scope>configuration</scope>
<complexity>medium</complexity>
<dependencies>ci|docker|testing</dependencies>
<unblocks>"-"</unblocks>
</task_context>

# Tarefa 17.0: CI/CD (GitHub Actions: build, test, docker publish)

## Visão Geral
Configurar pipelines para build, testes unitários/integração/E2E, e publicar imagens Docker.

## Requisitos
- Workflow para backend: build, testes, docker push
- Workflow para frontend: build, lint, E2E (opcional headless)
- Workflow para MCP

## Subtarefas
- [ ] 17.1 Backend pipeline (matrix .NET)
- [ ] 17.2 Frontend pipeline (node cache)
- [ ] 17.3 Docker publish para registry

## Sequenciamento
- Bloqueado por: 8.0, 15.0, 16.0
- Desbloqueia: -
- Paralelizável: Sim (após testes & docker)

## Detalhes de Implementação
Adicionar caches e artefatos; gates de qualidade.

## Critérios de Sucesso
- Pipelines executam e publicam imagens com tags
