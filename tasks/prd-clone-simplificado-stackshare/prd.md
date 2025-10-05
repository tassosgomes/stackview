# PRD - Clone Simplificado do StackShare

## Visão Geral

Plataforma web para desenvolvedores compartilharem e descobrirem tech stacks, permitindo cadastrar sistemas (frontend, backend, mobile) com suas respectivas linguagens, frameworks e bibliotecas. O diferencial é a integração com um servidor MCP (Model Context Protocol) para conectar assistentes de IA como GitHub Copilot e Claude Desktop, possibilitando consultas e interações automatizadas com os dados dos stacks.

## Objetivos

- **Principal**: Criar uma comunidade simplificada onde desenvolvedores possam documentar, compartilhar e descobrir tech stacks de projetos, com suporte a IA através de MCP para consultas inteligentes.
- **Secundários**:
    - Facilitar a descoberta de tecnologias por categoria (Frontend, Backend, Mobile, DevOps, Data, Testing).
    - Permitir documentação rica em Markdown para detalhamento dos stacks.
    - Integrar com assistentes de IA para consultas contextuais sobre stacks, com autenticação individual.
    - Criar uma base de conhecimento pesquisável de decisões tecnológicas, com histórico de versões.

## Histórias de Usuário

- **Como Desenvolvedor(a)**, eu quero cadastrar o stack de um projeto, incluindo tecnologias e uma descrição detalhada, para que outros possam entender as decisões de arquitetura.
- **Como Desenvolvedor(a)**, eu quero buscar por stacks que usam uma tecnologia específica (ex: "React"), para descobrir como outras equipes a estão utilizando.
- **Como Desenvolvedor(a)**, eu quero visualizar o histórico de mudanças de um stack, para entender como ele evoluiu ao longo do tempo.
- **Como Usuário de IA (via Copilot/Claude)**, eu quero gerar um token de acesso pessoal, para poder consultar de forma segura os stacks da plataforma diretamente do meu editor.
- **Como Administrador**, eu quero poder pré-cadastrar tecnologias comuns, para manter a consistência dos dados, enquanto permito que usuários adicionem novas tecnologias que ainda não existem.

## Funcionalidades Principais

### 1. Gestão de Stacks
- **1.1.** Criação de stack com nome, descrição e tipo (Frontend, Backend, Mobile, DevOps, Data, Testing).
- **1.2.** Descrição detalhada em Markdown.
- **1.3.** Marcação de stacks como públicos ou privados.
- **1.4.** Edição e exclusão de stacks.
- **1.5.** Histórico de versões para cada stack, permitindo visualizar alterações ao longo do tempo.

### 2. Gestão de Tecnologias
- **2.1.** Usuários podem selecionar tecnologias de uma lista pré-cadastrada (com autocomplete).
- **2.2.** Usuários podem adicionar novas tecnologias (linguagens, frameworks, bibliotecas) caso não existam na lista. Essas novas tecnologias ficam disponíveis para outros usuários.
- **2.3.** Administradores podem gerenciar (adicionar, editar, remover) a lista de tecnologias pré-cadastradas.

### 3. Navegação e Busca
- **3.1.** Listagem de stacks públicos com filtros por tipo e tecnologia.
- **3.2.** Busca por tecnologia específica, nome do stack ou descrição.
- **3.3.** Visualização pública de stacks de outros usuários.

### 4. Servidor MCP
- **4.1.** Exposição de ferramentas via MCP para consulta de stacks.
- **4.2.** Autenticação individual: usuários podem gerar um token de acesso pessoal em seu perfil para usar com assistentes de IA.
- **4.3.** Ferramentas disponíveis: `search_stacks`, `get_stack_details`, `list_technologies`.

### 5. Perfil de Usuário
- **5.1.** Dashboard com stacks criados pelo usuário.
- **5.2.** Seção para gerar e gerenciar tokens de acesso para o servidor MCP.
- **5.3.** Autenticação via email/senha.

## Experiência do Usuário

- **Fluxo de Cadastro**: O usuário se cadastra, faz login e é direcionado para seu dashboard.
- **Fluxo de Criação de Stack**: No dashboard, o usuário clica em "Criar Novo Stack", preenche o formulário (nome, tipo, tecnologias, descrição em Markdown) e salva.
- **Fluxo de Descoberta**: Na página "Explorar", o usuário pode ver uma lista de stacks, filtrar por categoria ou buscar por uma tecnologia específica para encontrar projetos de interesse.
- **Fluxo de Token MCP**: No seu perfil, o usuário encontra uma seção "Acesso IA", clica em "Gerar Novo Token" e usa esse token para configurar seu assistente de IA (Copilot/Claude).

## Restrições Técnicas de Alto Nível

- **Backend**: .NET 8+ (C#) com ASP.NET Core Web API.
- **Frontend**: React 18+ com TypeScript.
- **Banco de Dados**: PostgreSQL.
- **Autenticação**: ASP.NET Core Identity para usuários; Tokens JWT para a API; Tokens pessoais para o MCP.
- **MCP SDK**: `ModelContextProtocol` NuGet package.
- **Infraestrutura**: O sistema deve ser containerizável com Docker.

## Não-Objetivos (Fora de Escopo para v1)

- **Recursos Sociais**: Funcionalidades como comentários, likes, seguir usuários ou forking de stacks não serão implementadas nesta versão.
- **Integração com Git**: A detecção automática de tecnologias a partir de repositórios GitHub não será incluída.
- **Autenticação OIDC**: O suporte a OIDC (Keycloak, Logto) é um objetivo futuro, mas não para a versão inicial.
- **Organizações/Times**: Não haverá conceito de times ou organizações; todos os stacks pertencem a usuários individuais.

## Questões em Aberto

- **Aprovação de Novas Tecnologias**: Quando um usuário adiciona uma nova tecnologia, ela deve ser aprovada por um administrador antes de se tornar pública? Ou fica disponível imediatamente?
- **Limites de Versionamento**: Haverá um limite para o número de versões que um stack pode ter?
- **Detalhes do Token MCP**: Qual deve ser o tempo de expiração padrão para os tokens de acesso do MCP? Eles podem ser revogados?
