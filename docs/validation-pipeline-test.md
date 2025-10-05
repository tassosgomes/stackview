# Teste do ValidationPipelineBehavior

## Teste de Validação Automática

Este arquivo documenta o teste manual do ValidationPipelineBehavior implementado na Task 7.0.

### Cenário: Validação automática via MediatR Pipeline

**Request**: POST /api/users/me/mcp-tokens  
**Body inválido**:
```json
{
    "name": ""
}
```

**Comportamento esperado**:
1. ValidationPipelineBehavior intercepta o request
2. Executa GenerateMcpTokenValidator
3. Falha na validação (nome vazio)
4. Lança ValidationException
5. GlobalExceptionMiddleware captura a exceção
6. Retorna 400 Bad Request com formato padronizado

**Resposta esperada**:
```json
{
    "message": "Validation failed",
    "errors": {
        "name": "Nome é obrigatório"
    }
}
```

### Implementação Testada

1. ✅ **ValidationPipelineBehavior**: Intercepta todos os requests MediatR
2. ✅ **Registro no DI**: Pipeline behavior registrado no Program.cs
3. ✅ **GlobalExceptionMiddleware**: Trata ValidationException
4. ✅ **Formato padronizado**: Retorna JSON conforme documentação

### Validação Manual

Para testar manualmente:

```bash
# 1. Iniciar a API
cd backend/src/StackShare.API
dotnet run

# 2. Registrar usuário
curl -X POST "http://localhost:5000/api/auth/register" \
  -H "Content-Type: application/json" \
  -d '{"email": "test@example.com", "password": "Password123", "confirmPassword": "Password123"}'

# 3. Fazer login
curl -X POST "http://localhost:5000/api/auth/login" \
  -H "Content-Type: application/json" \
  -d '{"email": "test@example.com", "password": "Password123"}'

# 4. Testar validação com nome vazio
curl -X POST "http://localhost:5000/api/users/me/mcp-tokens" \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer {JWT_TOKEN}" \
  -d '{"name": ""}'
```

**Resultado esperado**: HTTP 400 com formato de erro padronizado.