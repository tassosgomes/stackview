# E2E Testing Guide

Este documento descreve como configurar e executar os testes End-to-End (E2E) usando Playwright.

## Pré-requisitos

- Node.js 18+
- .NET 8 SDK
- PostgreSQL em execução
- Backend e Frontend configurados

## Estrutura dos Testes

```
frontend/e2e/
├── pages/                    # Page Object Models
│   ├── base.page.ts         # Classe base para todas as páginas
│   ├── login.page.ts        # Página de login
│   ├── dashboard.page.ts    # Dashboard do usuário
│   ├── stack-create.page.ts # Criação de stacks
│   ├── explore.page.ts      # Exploração de stacks
│   └── profile.page.ts      # Perfil e tokens MCP
├── fixtures/
│   └── test-data.ts         # Dados de teste e helpers
├── login-and-stack-creation.spec.ts     # Cenário 15.1
├── exploration-and-search.spec.ts       # Cenário 15.2
└── mcp-token-management.spec.ts         # Cenário 15.3
```

## Cenários de Teste

### 15.1 Login e Criação de Stack
- ✅ Login com credenciais válidas
- ✅ Navegação para dashboard
- ✅ Criação de stack com nome, tipo, descrição e tecnologias
- ✅ Criação de stack com conteúdo Markdown
- ✅ Validação de formulários
- ✅ Criação de stacks públicos e privados
- ✅ Adição de múltiplas tecnologias

### 15.2 Exploração por Filtros/Busca
- ✅ Exploração de stacks públicos sem autenticação
- ✅ Busca por tecnologia específica
- ✅ Filtros por tipo de stack
- ✅ Navegação para detalhes do stack
- ✅ Busca case-insensitive
- ✅ Tratamento de caracteres especiais
- ✅ Combinação de filtros e busca

### 15.3 Geração e Revogação de Token MCP
- ✅ Navegação para seção de tokens MCP
- ✅ Geração de novos tokens
- ✅ Exibição única do token (segurança)
- ✅ Cópia do token para clipboard
- ✅ Revogação de tokens existentes
- ✅ Múltiplos tokens por usuário
- ✅ Validação de formato de token

## Configuração

### 1. Instalar Dependências

```bash
cd frontend
npm install
npx playwright install
```

### 2. Configurar Ambiente

Certifique-se de que o backend está rodando:

```bash
cd backend
dotnet run --project src/StackShare.API
```

### 3. Executar Setup (Opcional)

```bash
cd frontend
./scripts/setup-e2e.sh
```

## Execução dos Testes

### Comandos Disponíveis

```bash
# Executar todos os testes (headless)
npm run test:e2e

# Executar com interface do browser visível
npm run test:e2e:headed

# Executar com Playwright UI (interativo)
npm run test:e2e:ui

# Executar em modo debug
npm run test:e2e:debug

# Ver relatório dos testes
npm run test:e2e:report
```

### Executar Testes Específicos

```bash
# Executar apenas testes de login
npx playwright test login-and-stack-creation

# Executar apenas testes de exploração
npx playwright test exploration-and-search

# Executar apenas testes de tokens MCP
npx playwright test mcp-token-management
```

### Executar Teste Específico

```bash
# Executar um teste específico
npx playwright test -g "should successfully login and create a new stack"
```

## Configuração para CI/CD

O arquivo `.github/workflows/e2e-tests.yml` está configurado para executar os testes automaticamente em:
- Push para branches `main` e `develop`
- Pull requests para `main` e `develop`

### Pré-requisitos no CI
- PostgreSQL service
- Backend build e execução
- Frontend build e serve
- Playwright browsers

## Debugging

### Executar com Debug

```bash
npm run test:e2e:debug
```

### Screenshots e Videos

Por padrão, o Playwright captura:
- Screenshots em falhas
- Videos dos testes (se configurado)
- Traces para análise

### Logs Detalhados

```bash
DEBUG=pw:api npx playwright test
```

## Dados de Teste

### Usuário Padrão
- Email: `test@example.com`
- Senha: `Password123!`

### Stacks de Teste
- Frontend: React E-commerce (público)
- Backend: Node.js API (público)
- Mobile: Flutter App (privado)

### Tecnologias Disponíveis
React, Vue.js, Angular, Node.js, Express.js, TypeScript, JavaScript, Python, Django, FastAPI, PostgreSQL, MongoDB, Redis, Docker, Kubernetes

## Troubleshooting

### Backend não está rodando
```bash
cd backend
dotnet run --project src/StackShare.API
```

### Frontend não está rodando
O Playwright irá iniciar automaticamente o servidor de desenvolvimento.

### Browsers não instalados
```bash
npx playwright install
```

### Timeouts
Ajustar timeouts no `playwright.config.ts`:
```typescript
use: {
  actionTimeout: 30000,
  navigationTimeout: 60000,
}
```

### Falhas de Autenticação
Verificar se o usuário de teste existe no banco de dados ou se é criado automaticamente.

## Page Object Model

### Estrutura
- `BasePage`: Métodos comuns (navegação, espera, screenshots)
- Páginas específicas: Encapsulam elementos e ações da página
- Seletores: Priorizam `data-testid`, fallback para seletores CSS

### Exemplo
```typescript
const loginPage = new LoginPage(page);
await loginPage.navigate();
await loginPage.login('user@example.com', 'password');
```

## Relatórios

Os relatórios são gerados em:
- `playwright-report/index.html`
- Acessível via `npm run test:e2e:report`

## Próximos Passos

1. ✅ Configurar Playwright
2. ✅ Implementar Page Objects
3. ✅ Criar cenários de teste
4. ✅ Configurar CI/CD
5. 🔄 Validar execução local
6. 🔄 Otimizar performance
7. 🔄 Adicionar mais cenários de edge cases