---
status: completed
parallelizable: false
blocked_by: ["1.0"]
---

<task_context>
<domain>infra/database</domain>
<type>implementation</type>
<scope>core_feature</scope>
<complexity>medium</complexity>
<dependencies>database</dependencies>
<unblocks>"3.0","4.0","5.0","6.0","8.0","16.0"</unblocks>
</task_context>

# Tarefa 2.0: Banco de Dados e Migrations (EF Core + PostgreSQL, schema inicial)

## Visão Geral
Configurar DbContext, connection string, migrations e schema inicial para entidades base.

## Requisitos
- Configurar EF Core para PostgreSQL
- Criar DbContext e aplicar migrations iniciais
- Tabelas: Users (Identity), Stacks, Technologies, StackTechnology, McpApiToken, StackHistory

## Subtarefas
- [x] 2.1 Adicionar provider Npgsql e configurar DbContext
- [x] 2.2 Modelar entidades e relacionamentos
- [x] 2.3 Criar e aplicar migration inicial
- [x] 2.4 Dados seed de tecnologias comuns (opcional)

## Sequenciamento
- Bloqueado por: 1.0
- Desbloqueia: 3.0, 4.0, 5.0, 6.0, 8.0, 16.0
- Paralelizável: Não (depende do setup base)

## Detalhes de Implementação
Conforme seção 3 da TechSpec.

## Critérios de Sucesso
- Migration inicial aplicada em um Postgres local
- Entidades e FKs criadas corretamente
