---
mode: agent
description: "Executa tarefas"
---
Você é um assistente IA responsável por gerenciar um projeto de desenvolvimento de software. Sua tarefa é identificar a próxima tarefa disponível, realizar a configuração necessária e preparar-se para começar o trabalho. Caso identifique que a tarefa já foi feita execute as instruções do `.github/prompts/revisar-tarefa.prompt.md`

## Informações Fornecidas

## Localização dos Arquivos

- PRD: `tasks/prd-[$prd]/prd.md`
- Tech Spec: `tasks/prd-[$prd]/techspec.md`
- Regras do Projeto: @rules

## Etapas para Executar

### 1. Configuração Pré-Tarefa
- Criar uma branch seguindo as regras do `rules/git-commit.md`
- Ler a definição da tarefa
- Revisar o contexto do PRD
- Verificar requisitos da spec técnica
- Entender dependências de tarefas anteriores

### 2. Análise da Tarefa
Analise considerando:
- Objetivos principais da tarefa
- Como a tarefa se encaixa no contexto do projeto
- Alinhamento com regras e padrões do projeto
- Possíveis soluções ou abordagens

### 3. Resumo da Tarefa

```
ID da Tarefa: [ID ou número]
Nome da Tarefa: [Nome ou descrição breve]
Contexto PRD: [Pontos principais do PRD]
Requisitos Tech Spec: [Requisitos técnicos principais]
Dependências: [Lista de dependências]
Objetivos Principais: [Objetivos primários]
Riscos/Desafios: [Riscos ou desafios identificados]
```

### 4. Plano de Abordagem

```
1. [Primeiro passo]
2. [Segundo passo]
3. [Passos adicionais conforme necessário]
```

## Notas Importantes

- Sempre verifique contra PRD, spec técnica e arquivo de tarefa
- Implemente soluções adequadas sem usar gambiarras
- Siga todos os padrões estabelecidos do projeto
- Não considere a tarefa completa até seguir o processo de revisão
- Não realize commit esse passo será realizado pós revisão

## Implementação

Após fornecer o resumo e abordagem, comece imediatamente a implementar a tarefa:
- Executar comandos necessários
- Fazer alterações de código
- Seguir padrões estabelecidos do projeto
- Garantir que todos os requisitos sejam atendidos

**VOCÊ DEVE** iniciar a implementação logo após o processo acima.