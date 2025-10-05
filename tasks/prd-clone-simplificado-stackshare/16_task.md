---
status: pending
parallelizable: true
blocked_by: ["1.0","2.0","4.0","9.0","13.0"]
---

<task_context>
<domain>infra/devops</domain>
<type>integration</type>
<scope>configuration</scope>
<complexity>medium</complexity>
<dependencies>docker|compose</dependencies>
<unblocks>"15.0","17.0"</unblocks>
</task_context>

# Tarefa 16.0: Dockerização e Docker Compose (API, Frontend, MCP, Postgres)

## Visão Geral
Criar Dockerfiles para API, Frontend e MCP. Configurar docker-compose com Postgres e redes/variáveis.

## Requisitos
- Dockerfile para cada serviço
- docker-compose para dev com volumes e envs
- Scripts make/task para subir stack local

## Subtarefas
- [ ] 16.1 Dockerfile API (.NET)
- [ ] 16.2 Dockerfile MCP (.NET Worker)
- [ ] 16.3 Dockerfile Frontend (Vite build + nginx)
- [ ] 16.4 docker-compose.yml com Postgres

## Sequenciamento
- Bloqueado por: 1.0, 2.0, 4.0, 9.0, 13.0
- Desbloqueia: 15.0, 17.0
- Paralelizável: Sim

## Detalhes de Implementação
Seguir restrições do PRD (containerizável).

## Critérios de Sucesso
- Ambiente local sobe com um comando e funciona
