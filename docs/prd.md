# PRD - Clone Simplificado do StackShare

## Visão Geral

Plataforma web para desenvolvedores compartilharem e descobrirem tech stacks, permitindo cadastrar sistemas (frontend, backend, mobile) com suas respectivas linguagens, frameworks e bibliotecas. O diferencial é a integração com um servidor MCP (Model Context Protocol) para conectar assistentes de IA como GitHub Copilot e Claude Desktop, possibilitando consultas e interações automatizadas com os dados dos stacks.[^1][^2][^3][^4]

## Objetivos

### Objetivo Principal

Criar uma comunidade simplificada onde desenvolvedores possam documentar, compartilhar e descobrir tech stacks de projetos, com suporte a IA através de MCP para consultas inteligentes.[^5][^1]

### Objetivos Secundários

- Facilitar a descoberta de tecnologias por categoria (frontend, backend, mobile)
- Permitir documentação rica em Markdown para detalhamento dos stacks[^2]
- Integrar com assistentes de IA para consultas contextuais sobre stacks[^6][^7]
- Criar uma base de conhecimento pesquisável de decisões tecnológicas


## Funcionalidades Principais

### Cadastro de Stacks

- Criação de stack com nome, descrição e tipo (Frontend/Backend/Mobile)
- Seleção múltipla de linguagens de programação
- Seleção múltipla de frameworks
- Seleção múltipla de bibliotecas/pacotes
- Descrição detalhada em Markdown com suporte a formatação rica[^2]
- Marcação de stacks como públicos ou privados


### Navegação e Busca

- Listagem de stacks públicos com filtros por tipo
- Busca por tecnologia específica (linguagem, framework, biblioteca)
- Visualização de stacks de outros usuários
- Comparação de tecnologias lado a lado[^1]


### Servidor MCP

- Exposição de ferramentas via MCP para consulta de stacks[^4][^6]
- Integração com GitHub Copilot (VS Code)[^8][^9]
- Integração com Claude Desktop[^10][^11]
- Ferramentas disponíveis: buscar stacks, listar tecnologias, consultar uso de tecnologias


### Perfil de Usuário

- Dashboard com stacks criados
- Estatísticas de tecnologias mais usadas
- Autenticação via email/senha ou OIDC (Keycloak, Logto)[^12]


## Stack Técnico Recomendado

### Backend

- **Framework**: .NET 8+ (C\#) com ASP.NET Core Web API[^12][^6]
- **Banco de Dados**: PostgreSQL com EF Core Migrations[^12]
- **Autenticação**: ASP.NET Core Identity + OIDC (Keycloak/Logto opcional)[^12]
- **Logging**: Serilog[^12]
- **Observabilidade**: OpenTelemetry[^12]
- **MCP SDK**: ModelContextProtocol NuGet package (preview)[^6]


### Frontend

- **Framework**: React 18+ com TypeScript
- **Estilização**: Tailwind CSS + ShadCN UI components[^12]
- **Editor Markdown**: React-Markdown ou MDXEditor
- **State Management**: React Query + Context API
- **Roteamento**: React Router


### Testes

- **Unitários**: xUnit (backend)[^12]
- **Integração**: WebApplicationFactory + Testcontainers (PostgreSQL)
- **E2E**: Playwright[^12]


### Infraestrutura

- **Containerização**: Docker + Docker Compose
- **CI/CD**: GitHub Actions ou Azure DevOps
- **Deploy**: Azure App Service ou Kubernetes


## Arquitetura do Sistema

### Backend API (REST)

```
/api/stacks
  GET    /               - Lista stacks públicos (com paginação/filtros)
  GET    /{id}          - Detalhes de um stack
  POST   /               - Cria novo stack
  PUT    /{id}          - Atualiza stack
  DELETE /{id}          - Remove stack

/api/stacks/search
  GET    ?q=react       - Busca por tecnologia

/api/technologies
  GET    /languages     - Lista linguagens disponíveis
  GET    /frameworks    - Lista frameworks disponíveis
  GET    /libraries     - Lista bibliotecas disponíveis

/api/users
  GET    /me            - Perfil do usuário autenticado
  GET    /me/stacks     - Stacks do usuário
```


### Servidor MCP

O servidor MCP será implementado como um processo separado que expõe ferramentas para assistentes de IA:[^3][^4]

**Ferramentas Disponíveis:**

1. `search_stacks` - Busca stacks por nome, tecnologia ou descrição
2. `get_stack_details` - Obtém detalhes completos de um stack
3. `list_technologies` - Lista todas as tecnologias cadastradas
4. `get_popular_technologies` - Retorna tecnologias mais usadas
5. `compare_stacks` - Compara dois stacks lado a lado

**Configuração para Clientes:**

- **Claude Desktop**: Configuração em `claude_desktop_config.json`[^11]
- **VS Code Copilot**: Detecção automática via `.vscode/mcp.json`[^9][^8]
- **Protocolo**: HTTP/SSE ou WebSocket[^3]


## Modelo de Dados

### Entidades Principais

#### User

```csharp
public class User
{
    public Guid Id { get; set; }
    public string Email { get; set; }
    public string Name { get; set; }
    public DateTime CreatedAt { get; set; }
    public ICollection<Stack> Stacks { get; set; }
}
```


#### Stack

```csharp
public class Stack
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; } // Markdown
    public StackType Type { get; set; } // Frontend/Backend/Mobile
    public bool IsPublic { get; set; }
    public Guid UserId { get; set; }
    public User User { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public ICollection<StackTechnology> Technologies { get; set; }
}

public enum StackType
{
    Frontend,
    Backend,
    Mobile
}
```


#### Technology

```csharp
public class Technology
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public TechnologyCategory Category { get; set; }
    public string? Description { get; set; }
    public string? IconUrl { get; set; }
    public ICollection<StackTechnology> Stacks { get; set; }
}

public enum TechnologyCategory
{
    Language,
    Framework,
    Library
}
```


#### StackTechnology (Join Table)

```csharp
public class StackTechnology
{
    public Guid StackId { get; set; }
    public Stack Stack { get; set; }
    public Guid TechnologyId { get; set; }
    public Technology Technology { get; set; }
}
```


## Implementação do Servidor MCP

### Estrutura do Projeto

```
MyStackShare.MCP/
├── Program.cs
├── Tools/
│   ├── SearchStacksTool.cs
│   ├── GetStackDetailsTool.cs
│   ├── ListTechnologiesTool.cs
│   └── CompareStacksTool.cs
├── Services/
│   └── StackShareApiClient.cs
└── MyStackShare.MCP.csproj
```


### Exemplo de Implementação (C\#)

```csharp
// Program.cs
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ModelContextProtocol;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddMcpServer()
    .AddTool<SearchStacksTool>()
    .AddTool<GetStackDetailsTool>()
    .AddTool<ListTechnologiesTool>()
    .AddTool<CompareStacksTool>();

builder.Services.AddHttpClient<StackShareApiClient>();

var app = builder.Build();
await app.RunAsync();
```


### Configuração para Claude Desktop

```json
{
  "mcpServers": {
    "mystackshare": {
      "command": "dotnet",
      "args": ["run", "--project", "/path/to/MyStackShare.MCP"],
      "env": {
        "API_BASE_URL": "https://localhost:5001/api",
        "API_KEY": "your-api-key"
      }
    }
  }
}
```


## Autenticação e Segurança

### Autenticação

- **Padrão**: Email/senha com ASP.NET Core Identity[^12]
- **Opcional**: OIDC com Keycloak ou Logto (configurável por variável de ambiente)[^12]
- **Token**: JWT para API REST
- **MCP**: API Key para autenticação do servidor MCP[^11]


### Autorização

- Stacks privados só acessíveis pelo criador
- API Key do MCP com rate limiting
- CORS configurado para frontend


## Interface do Usuário

### Páginas Principais

#### Home/Landing

- Hero section explicando o propósito
- Estatísticas da plataforma (stacks cadastrados, tecnologias)
- Stacks em destaque/populares


#### Dashboard

- Lista de stacks do usuário
- Botão "Criar Novo Stack"
- Estatísticas pessoais


#### Criar/Editar Stack

- Formulário com nome, tipo, descrição
- Seleção múltipla de tecnologias (com autocomplete)
- Editor Markdown com preview
- Toggle público/privado


#### Visualizar Stack

- Informações do stack
- Tecnologias organizadas por categoria
- Descrição renderizada (Markdown)
- Informações do autor


#### Explorar/Busca

- Grid de cards de stacks
- Filtros por tipo e tecnologias
- Busca por texto


## Testes

### Unitários (xUnit)

- Testes de domínio e lógica de negócio
- Testes de services e repositories
- Testes das ferramentas MCP[^12]


### Integração

- Testes de API endpoints com WebApplicationFactory
- Testcontainers para PostgreSQL
- Testes de autenticação/autorização[^12]


### E2E (Playwright)

- Fluxo completo de cadastro de stack
- Busca e navegação
- Autenticação[^12]


## Cronograma Estimado

### Sprint 1 (2 semanas) - Fundação

- Setup do projeto (.NET + React + PostgreSQL)
- Autenticação básica (email/senha)
- Modelo de dados e migrations
- CRUD de stacks (backend)


### Sprint 2 (2 semanas) - Frontend Core

- Interface de criação/edição de stack
- Suporte a Markdown
- Dashboard do usuário
- Páginas de listagem e busca


### Sprint 3 (1-2 semanas) - MCP Server

- Implementação do servidor MCP em C\#[^6]
- Ferramentas básicas (search, get details, list)
- Configuração para Claude Desktop[^11]
- Testes de integração com Copilot[^8]


### Sprint 4 (1 semana) - Refinamento

- OIDC opcional
- Testes E2E
- Documentação
- Deploy


## Perguntas para Definição

Antes de iniciar o desenvolvimento, seria importante esclarecer:

1. **Cadastro de Tecnologias**: As tecnologias (linguagens, frameworks, bibliotecas) serão pré-cadastradas por admin ou os usuários podem adicionar novas tecnologias livremente?
2. **Autenticação MCP**: O servidor MCP deve autenticar usuários individuais ou usar uma API key global para acesso aos dados públicos?
3. **Recursos Sociais**: Deseja funcionalidades como comentários, likes, ou seguir outros usuários? Ou manter o foco apenas em cadastro e busca?
4. **Versionamento**: Stacks devem ter histórico de versões para acompanhar mudanças ao longo do tempo?
5. **Categorização Adicional**: Além de Frontend/Backend/Mobile, deseja outras categorizações como "DevOps", "Data", "Testing"?
6. **Integração com GitHub**: Seria útil detectar automaticamente tecnologias de repositórios GitHub dos usuários?
<span style="display:none">[^13][^14][^15][^16][^17][^18][^19][^20]</span>

<div align="center">⁂</div>

[^1]: https://stackshare.io/stacks

[^2]: https://stackshare.io/posts/introducing-stackshare-the-worlds-first-tech-stack-intelligence-platform

[^3]: https://northflank.com/blog/how-to-build-and-deploy-a-model-context-protocol-mcp-server

[^4]: https://towardsdatascience.com/model-context-protocol-mcp-tutorial-build-your-first-mcp-server-in-6-steps/

[^5]: https://www.anthropic.com/news/model-context-protocol

[^6]: https://devblogs.microsoft.com/dotnet/build-a-model-context-protocol-mcp-server-in-csharp/

[^7]: https://modelcontextprotocol.io/docs/develop/build-server

[^8]: https://docs.github.com/copilot/customizing-copilot/using-model-context-protocol/extending-copilot-chat-with-mcp

[^9]: https://code.visualstudio.com/docs/copilot/customization/mcp-servers

[^10]: https://learn.microsoft.com/en-us/power-apps/maker/data-platform/data-platform-mcp

[^11]: https://www.linkedin.com/pulse/talk-your-data-from-github-copilot-claude-without-rupert-barrow-u6c0e

[^12]: https://www.vcstack.io/product/stackshare

[^13]: https://stackshare.io

[^14]: https://pipedream.com/apps/stackshare-api

[^15]: https://himalayas.app/companies/stackshare/tech-stack

[^16]: https://stackshare.io/stackshare/stackshare

[^17]: https://github.com/stackshareio

[^18]: https://github.com/orgs/community/discussions/166967

[^19]: https://www.reddit.com/r/Doesthisexist/comments/1b90cad/an_app_for_sharing_your_tech_stack_with_friends/

[^20]: https://modelcontextprotocol.io/examples

