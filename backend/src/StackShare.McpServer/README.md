# StackShare MCP Server

Servidor MCP (Model Context Protocol) para integrar com assistentes de IA (GitHub Copilot, Claude Desktop) e consultar stacks e tecnologias do StackShare.

## 🚀 Funcionalidades

O servidor expõe 3 ferramentas MCP:

### 1. `search_stacks`
Busca stacks no StackShare com filtros opcionais.

**Parâmetros:**
- `search` (opcional): Termo de busca para filtrar por nome ou descrição
- `type` (opcional): Tipo do stack (Frontend, Backend, Mobile, DevOps, Data, Testing)
- `technologyName` (opcional): Nome da tecnologia para filtrar
- `page` (opcional): Página da paginação (default: 1)
- `pageSize` (opcional): Tamanho da página (default: 10)

**Exemplo de uso no Claude Desktop:**
```
Busque stacks que usam React
```

### 2. `get_stack_details`
Obtém detalhes completos de um stack específico pelo ID.

**Parâmetros:**
- `stackId` (obrigatório): ID único do stack

**Exemplo de uso:**
```
Mostre os detalhes do stack com ID abc123...
```

### 3. `list_technologies`
Lista todas as tecnologias disponíveis na plataforma.

**Parâmetros:**
- `search` (opcional): Termo de busca para filtrar por nome
- `page` (opcional): Página da paginação (default: 1)
- `pageSize` (opcional): Tamanho da página (default: 20)

**Exemplo de uso:**
```
Liste todas as tecnologias que contenham "javascript"
```

## 🔧 Configuração

### Pré-requisitos
- .NET 8.0 SDK
- StackShare Backend API rodando (default: `http://localhost:5000/`)

### Instalação e Execução

1. **Clone o repositório** (se ainda não fez):
   ```bash
   git clone https://github.com/tassosgomes/stackview.git
   cd stackview/backend
   ```

2. **Build do projeto**:
   ```bash
   dotnet build src/StackShare.McpServer/StackShare.McpServer.csproj
   ```

3. **Configure a URL da API** (se necessário):
   Edite `src/StackShare.McpServer/appsettings.json`:
   ```json
   {
     "StackShareApi": {
       "BaseUrl": "http://localhost:5000/"
     }
   }
   ```

4. **Execute o servidor MCP**:
   ```bash
   dotnet run --project src/StackShare.McpServer/StackShare.McpServer.csproj
   ```

### Testes de Integração

Execute os testes básicos para verificar se tudo está funcionando:

```bash
dotnet run --project src/StackShare.McpServer/StackShare.McpServer.csproj -- --test
```

## 🔌 Integração com Claude Desktop

Para usar com Claude Desktop, adicione a seguinte configuração no arquivo de configuração do Claude (`claude_desktop_config.json`):

```json
{
  "mcpServers": {
    "stackshare": {
      "command": "dotnet",
      "args": [
        "run", 
        "--project", 
        "/caminho/para/stackview/backend/src/StackShare.McpServer/StackShare.McpServer.csproj"
      ],
      "env": {
        "ASPNETCORE_ENVIRONMENT": "Development"
      }
    }
  }
}
```

## 🔌 Integração com GitHub Copilot

Para usar com GitHub Copilot, você pode configurar o MCP server como uma extensão ou usar através de ferramentas que suportam MCP.

## 📊 Logs

Os logs são salvos em:
- **Console**: Saída padrão durante execução
- **Arquivo**: `logs/mcp-server-YYYY-MM-DD.log`

### Níveis de Log:
- **Information**: Operações normais
- **Debug**: Detalhes de depuração (apenas em Development)
- **Warning**: Situações que merecem atenção
- **Error**: Erros de execução

## 🛠️ Desenvolvimento

### Estrutura do Projeto
```
src/StackShare.McpServer/
├── Models/                 # DTOs para comunicação com API
├── Services/              # StackShareApiClient
├── Tools/                 # Ferramentas MCP (StackShareTools)
├── Tests/                 # Testes de integração
├── Program.cs             # Configuração e DI
├── Worker.cs              # Serviço principal MCP
└── appsettings.json       # Configurações
```

### Adicionando Novas Ferramentas

Para adicionar uma nova ferramenta MCP:

1. Adicione um método público e estático na classe `StackShareTools`
2. Decore com `[McpTool("nome", "descrição")]`
3. Use `[McpParameter(required, "description")]` nos parâmetros
4. Retorne uma string JSON com o resultado

Exemplo:
```csharp
[McpTool("nova_ferramenta", "Descrição da nova ferramenta")]
public static async Task<string> NovaFerramenta(
    [McpParameter(true, "Parâmetro obrigatório")] string parametro)
{
    // Implementação
    return JsonSerializer.Serialize(resultado);
}
```

## 🔍 Troubleshooting

### Erro: "API não encontrada"
- Verifique se o StackShare Backend está rodando
- Confirme a URL no `appsettings.json`
- Verifique se não há firewall bloqueando

### Erro: "StackShareTools não foi inicializado"
- Indica problema na inicialização do Worker
- Verifique os logs para mais detalhes
- Confirme se todas as dependências estão disponíveis

### Performance lenta
- Aumente o `pageSize` se estiver fazendo muitas consultas
- Use filtros específicos para reduzir o volume de dados
- Verifique a latência da rede com a API

## 📝 Licença

Este projeto segue a mesma licença do StackShare principal.