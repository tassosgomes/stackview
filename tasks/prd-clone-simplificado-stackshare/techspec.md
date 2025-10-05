# Especificação Técnica: Clone Simplificado do StackShare

## 1. Visão Geral da Arquitetura

Esta especificação técnica detalha a implementação do "StackShare Simplificado", conforme definido no PRD. A arquitetura consiste em três componentes principais: um **Frontend** em React, um **Backend API** em .NET 8 (ASP.NET Core), e um **Servidor MCP** também em .NET 8. A persistência de dados será feita em um banco de dados **PostgreSQL**.

A comunicação entre o Frontend e o Backend será via API REST. O Servidor MCP atuará como um cliente da API REST do Backend para obter os dados necessários para as ferramentas de IA, garantindo um único ponto de verdade.

```
+----------------+      +------------------+      +-----------------+
| Frontend       |      | Backend API      |      | Servidor MCP    |
| (React)        |----->| (.NET 8)         |<-----| (.NET 8)        |
+----------------+      +-------+----------+      +-----------------+
                                |
                                |
                         +------v------+
                         | PostgreSQL  |
                         +-------------+
```

## 2. Design de Componentes

### Backend API

- **Padrão de Projeto**: Vertical Slice Architecture com MediatR para implementar o padrão CQRS. Cada funcionalidade (feature) será autocontida em seu próprio "slice", contendo o Request, Handler, e qualquer lógica específica.
- **Estrutura de Pastas**:
  ```
  /src
  ├── StackShare.API/         # Endpoints, Middlewares, Program.cs
  ├── StackShare.Application/ # Lógica de negócio, MediatR Handlers, DTOs
  ├── StackShare.Domain/      # Entidades, Enums, Exceções de domínio
  └── StackShare.Infrastructure/ # EF Core, Repositórios, Serviços externos
  ```
- **Bibliotecas Principais**:
  - `Microsoft.EntityFrameworkCore.PostgreSQL`: Acesso a dados.
  - `MediatR`: Implementação de CQRS.
  - `Serilog`: Logging, conforme `rules/logging.md`.
  - `FluentValidation`: Validação de requests.
  - `FuzzySharp`: Para sugestão de tecnologias existentes e prevenção de duplicatas.
  - `Microsoft.AspNetCore.Identity.EntityFrameworkCore`: Autenticação.

### Frontend

- **Estrutura de Pastas**:
  ```
  /src
  ├── components/   # Componentes de UI reutilizáveis (ShadCN)
  ├── features/     # Componentes e hooks por funcionalidade (ex: stacks, auth)
  ├── hooks/        # Hooks globais
  ├── lib/          # Configuração de clients (axios, react-query)
  ├── pages/        # Páginas da aplicação
  └── services/     # Lógica de chamada de API
  ```
- **Bibliotecas Principais**:
  - `react-router-dom`: Roteamento.
  - `tailwindcss` & `shadcn/ui`: Estilização.
  - `@tanstack/react-query` (React Query): Gerenciamento de estado do servidor (caching, refetching).
  - `axios`: Cliente HTTP para chamadas à API.
  - `react-hook-form` & `zod`: Gerenciamento e validação de formulários.
  - `react-markdown`: Renderização de Markdown.

### Servidor MCP

- Será um projeto `Worker Service` em .NET.
- Usará o pacote `ModelContextProtocol` para expor as ferramentas.
- Implementará um `HttpClient` (`StackShareApiClient`) para se comunicar com a Backend API de forma autenticada (usando um token de serviço interno).

## 3. Modelos de Dados e Esquema

As entidades do PRD serão mapeadas para o PostgreSQL usando EF Core.

#### `Stack`
- Conforme o PRD.

#### `Technology`
- Conforme o PRD.

#### `StackTechnology` (Tabela de Junção)
- Conforme o PRD.

#### `User`
- Herdará de `IdentityUser<Guid>`.

#### `StackHistory` (Nova tabela para versionamento)
```csharp
public class StackHistory
{
    public Guid Id { get; set; }
    public Guid StackId { get; set; } // FK para Stack
    public int Version { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public StackType Type { get; set; }
    public string TechnologiesJson { get; set; } // Snapshot das tecnologias em JSON
    public DateTime CreatedAt { get; set; }
    public Guid ModifiedByUserId { get; set; }
}
```
- A cada `PUT /api/stacks/{id}`, uma nova entrada será criada nesta tabela.

#### `McpApiToken` (Nova tabela para tokens MCP)
```csharp
public class McpApiToken
{
    public Guid Id { get; set; }
    public string Token { get; set; } // Hash do token
    public Guid UserId { get; set; }
    public DateTime CreatedAt { get; set; }
    public bool IsRevoked { get; set; }
}
```

## 4. Endpoints da API (Contratos)

A API seguirá os padrões REST e usará DTOs para requests e responses. A paginação será `?page=1&pageSize=20`.

- `POST /api/stacks`
  - **Request**: `CreateStackRequest { Name, Description, Type, TechnologyIds[] }`
  - **Response**: `201 Created` com `StackResponse`.
- `PUT /api/stacks/{id}`
  - **Ação**: Atualiza o stack e cria uma entrada em `StackHistory`.
  - **Response**: `200 OK` com `StackResponse`.
- `GET /api/stacks`
  - **Query Params**: `page`, `pageSize`, `type`, `technology`.
  - **Response**: `200 OK` com `PagedList<StackSummaryResponse>`.
- `GET /api/stacks/{id}/history`
  - **Response**: `200 OK` com `List<StackHistoryResponse>`.
- `POST /api/technologies/suggest`
  - **Request**: `{ Name }`
  - **Response**: `200 OK` com `List<TechnologyDto>` (sugestões baseadas em `FuzzySharp`).
- `POST /api/users/me/mcp-tokens`
  - **Response**: `201 Created` com `{ RawToken }`. O token real só é retornado uma vez.
- `DELETE /api/users/me/mcp-tokens/{id}`
  - **Ação**: Define `IsRevoked = true`.
  - **Response**: `204 No Content`.

## 5. Tratamento de Erros e Logging

- Um middleware global de tratamento de exceções será implementado para capturar erros não tratados e retornar uma resposta JSON padronizada (`500 Internal Server Error`).
- Erros de validação (`FluentValidation`) retornarão `400 Bad Request` com detalhes dos campos inválidos.
- Erros de domínio (ex: "Stack não encontrado") retornarão `404 Not Found`.
- O Serilog será configurado para logar em console (desenvolvimento) e em arquivos JSON, seguindo o padrão de `rules/logging.md`.

## 6. Estratégia de Testes

- **Unitários (xUnit)**: Foco na lógica de negócio dentro dos Handlers do MediatR e nos serviços de domínio. As dependências externas (como repositórios) serão mockadas.
- **Integração (WebApplicationFactory + Testcontainers)**:
  - **Cenário 1 (Crítico)**: Fluxo de criação de stack.
    1. Registrar um usuário.
    2. Autenticar e obter token JWT.
    3. Criar um stack (`POST /api/stacks`).
    4. Verificar se o stack foi salvo no banco de dados de teste.
  - **Cenário 2 (Crítico)**: Geração e revogação de token MCP.
    1. Autenticar usuário.
    2. Gerar um token (`POST /api/users/me/mcp-tokens`).
    3. Verificar se o hash do token foi salvo.
    4. Revogar o token (`DELETE /api/users/me/mcp-tokens/{id}`).
    5. Verificar se `IsRevoked` é `true`.
- **E2E (Playwright)**: Simulará os fluxos de usuário completos descritos no PRD, incluindo login, criação de stack com preenchimento do editor Markdown e busca por tecnologias.

## 7. Análise de Impacto

Este é um projeto greenfield (novo), portanto não há impacto em código existente. A estrutura de diretórios e os arquivos serão criados do zero.

## 8. Observabilidade

- **Logging**: Serilog configurado para capturar logs de requests, erros e informações de diagnóstico.
- **Tracing**: OpenTelemetry será configurado no Backend API e no Servidor MCP para rastrear requisições. Isso será especialmente útil para monitorar a latência da comunicação entre o MCP e a API.

## 9. Plano de Rollout

O plano seguirá as Sprints definidas no PRD, com foco técnico em:
1.  **Sprint 1 (Fundação)**: Setup dos projetos, migrations do EF Core para criar o schema inicial, implementação da autenticação com Identity e CRUD básico de Stacks.
2.  **Sprint 2 (Frontend Core)**: Desenvolvimento das páginas de CRUD de stacks, dashboard e integração com a API (React Query).
3.  **Sprint 3 (MCP Server)**: Implementação do Worker Service, do `StackShareApiClient` e das ferramentas MCP.
4.  **Sprint 4 (Refinamento)**: Implementação do versionamento, testes E2E e configuração de CI/CD.

## 10. Questões em Aberto

Nenhuma questão técnica em aberto. O documento está pronto para guiar a implementação.
