# Formato Padrão de Respostas de Erro - StackShare API

## Visão Geral

A API do StackShare utiliza um formato padronizado para todas as respostas de erro, garantindo consistência e facilitando o tratamento pelos clientes da API.

## Estrutura Base de Erro

Todas as respostas de erro seguem a seguinte estrutura JSON:

```json
{
  "message": "Descrição legível do erro",
  "errors": {} // Opcional: detalhes específicos do erro
}
```

## Códigos de Status HTTP e Formatos

### 400 Bad Request - Erros de Validação

**Cenário**: Dados de entrada inválidos ou falha na validação (FluentValidation via ValidationPipelineBehavior)

```json
{
  "message": "Validation failed",
  "errors": {
    "campo1": "Mensagem de erro específica do campo",
    "campo2": "Outra mensagem de erro"
  }
}
```

**Exemplo real**:
```json
{
  "message": "Validation failed",
  "errors": {
    "name": "Nome é obrigatório",
    "email": "Email deve ter um formato válido"
  }
}
```

**⚠️ Importante**: A validação agora é **automática** via `ValidationPipelineBehavior` - não é necessário validação manual nos controllers.

### 401 Unauthorized - Não Autenticado

**Cenário**: Token JWT inválido, expirado ou ausente

```json
{
  "message": "Token de acesso inválido ou expirado"
}
```

### 404 Not Found - Recurso Não Encontrado

**Cenário**: Recurso solicitado não existe (NotFoundException)

```json
{
  "message": "Stack não encontrado"
}
```

**Outros exemplos**:
```json
{
  "message": "Token MCP não encontrado"
}
```

```json
{
  "message": "Tecnologia não encontrada"
}
```

### 409 Conflict - Conflito de Dados

**Cenário**: Tentativa de criar recurso que já existe

```json
{
  "message": "Tecnologia 'React' já existe"
}
```

### 500 Internal Server Error - Erro Interno

**Cenário**: Erro não tratado ou falha inesperada do sistema

```json
{
  "message": "An internal server error occurred"
}
```

**⚠️ Importante**: Detalhes internos do erro nunca são expostos ao cliente em produção.

## Middlewares Responsáveis

### GlobalExceptionMiddleware

Localizado em `StackShare.API/Middleware/GlobalExceptionMiddleware.cs`, este middleware:

1. **Captura todas as exceções não tratadas**
2. **Mapeia exceções específicas para códigos HTTP apropriados**
3. **Retorna resposta JSON padronizada**
4. **Registra logs de erro estruturados com contexto da requisição**
5. **Usa níveis de log apropriados (Error para não tratadas, Warning para esperadas)**

### ValidationPipelineBehavior

Localizado em `StackShare.Application/Behaviors/ValidationPipelineBehavior.cs`, este behavior:

1. **Intercepta todos os requests MediatR**
2. **Executa validações FluentValidation automaticamente**
3. **Lança ValidationException em caso de falha**
4. **É processado pelo GlobalExceptionMiddleware**

## Logging de Erros

Todos os erros são registrados com Serilog seguindo o padrão:

```csharp
_logger.LogError(ex, "An unhandled exception occurred");
```

### Níveis de Log por Tipo de Erro

- **Error**: Exceções não tratadas (500)
- **Warning**: Erros de negócio esperados (404, 409)
- **Information**: Erros de validação (400, 401)

## Headers HTTP Padronizados

Todas as respostas de erro incluem:

```
Content-Type: application/json
```

## Tratamento no Frontend

### Exemplo de Tratamento com Axios

```typescript
import axios from 'axios';

// Interceptor para tratamento global de erros
axios.interceptors.response.use(
  (response) => response,
  (error) => {
    if (error.response?.data?.message) {
      // Usar mensagem padronizada da API
      showErrorMessage(error.response.data.message);
    }
    
    if (error.response?.status === 400 && error.response.data.errors) {
      // Tratar erros de validação campo por campo
      handleValidationErrors(error.response.data.errors);
    }
    
    return Promise.reject(error);
  }
);
```

## Casos de Teste

### Testando Formato de Erro

```csharp
[Test]
public async Task Should_Return_StandardError_When_ValidationFails()
{
    // Arrange
    var invalidRequest = new { name = "" }; // Nome vazio
    
    // Act
    var response = await _client.PostAsJsonAsync("/api/stacks", invalidRequest);
    
    // Assert
    response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    var content = await response.Content.ReadAsStringAsync();
    var error = JsonSerializer.Deserialize<ErrorResponse>(content);
    
    error.Message.Should().Be("Validation failed");
    error.Errors.Should().ContainKey("name");
}
```

## Versionamento

- **Versão**: 1.0
- **Última atualização**: Task 7.0 - Middleware Global, Validação e Padronização de Respostas
- **Compatibilidade**: Mantida entre versões da API

---

**Nota**: Este documento deve ser mantido atualizado sempre que novos tipos de erro ou formatos de resposta forem adicionados à API.