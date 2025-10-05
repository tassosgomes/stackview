---
status: pending
parallelizable: false
blocked_by: ["1.0","2.0"]
---

<task_context>
<domain>engine/backend/auth</domain>
<type>implementation</type>
<scope>core_feature</scope>
<complexity>medium</complexity>
<dependencies>http_server|database</dependencies>
<unblocks>"4.0","6.0","10.0","12.0","13.0"</unblocks>
</task_context>

# Tarefa 3.0: Autenticação e Autorização (Identity + JWT)

## Visão Geral
Implementar ASP.NET Core Identity, endpoints de registro/login e emissão de tokens JWT.

## Requisitos
- Registro e login de usuário
- Emissão de JWT para API
- Políticas de autorização básicas

## Subtarefas
- [ ] 3.1 Configurar Identity com User<Guid>
- [ ] 3.2 Endpoints: POST /api/auth/register, /api/auth/login
- [ ] 3.3 Emissão e validação de JWT
- [ ] 3.4 Proteger endpoints de perfil e criação de stacks

## Sequenciamento
- Bloqueado por: 1.0, 2.0
- Desbloqueia: 4.0, 6.0, 10.0, 12.0, 13.0
- Paralelizável: Não (base de segurança)

## Detalhes de Implementação
Usar IdentityEntityFrameworkCore e JwtBearer.

## Critérios de Sucesso
- Login retorna JWT válido
- Endpoints protegidos exigem autorização
