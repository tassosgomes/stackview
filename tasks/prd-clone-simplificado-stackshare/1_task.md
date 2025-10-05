---
status: pending
parallelizable: true
blocked_by: []
---

<task_context>
<domain>infra/backend</domain>
<type>implementation</type>
<scope>core_feature</scope>
<complexity>medium</complexity>
<dependencies>http_server|database|logging|tracing</dependencies>
<unblocks>"2.0","3.0","4.0","5.0","6.0","7.0","8.0"</unblocks>
</task_context>

# Tarefa 1.0: Fundações do Backend (.NET 8, estrutura de soluções, packages, Serilog, OTel)

## Visão Geral
Inicializar a solução .NET com projetos API, Application, Domain e Infrastructure. Configurar Serilog para logs e OpenTelemetry para tracing básico.

## Requisitos
- Projeto deve ser iniciado na pasta backend
- Solução com 4 projetos em layout vertical slice
- Configuração de Serilog e logging estruturado
- OpenTelemetry adicionada (traces para requests HTTP)
- Pacotes base (MediatR, FluentValidation, EF Core Npgsql)

## Subtarefas
- [ ] 1.1 Criar solução e projetos dentro da pasta backend (API, Application, Domain, Infrastructure)
- [ ] 1.2 Adicionar pacotes NuGet e configurar DI base
- [ ] 1.3 Configurar Serilog (console e arquivo JSON)
- [ ] 1.4 Habilitar OpenTelemetry (AspNetCore + HttpClient)
- [ ] 1.5 Documentar estrutura em README interno

## Sequenciamento
- Bloqueado por: -
- Desbloqueia: 2.0, 3.0, 4.0, 5.0, 6.0, 7.0, 8.0
- Paralelizável: Sim (é inicial, mas isolada)

## Detalhes de Implementação
Seguir especificação: uso de MediatR, Serilog, OTel e estrutura proposta nas pastas.

## Critérios de Sucesso
- Build da solução ok
- Logs estruturados visíveis
- Tracing básico ativo para requests
