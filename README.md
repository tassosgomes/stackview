# StackView - Clone Simplificado do StackShare

[![Backend CI/CD](https://github.com/tassosgomes/stackview/actions/workflows/backend.yml/badge.svg)](https://github.com/tassosgomes/stackview/actions/workflows/backend.yml)
[![Frontend CI/CD](https://github.com/tassosgomes/stackview/actions/workflows/frontend.yml/badge.svg)](https://github.com/tassosgomes/stackview/actions/workflows/frontend.yml)
[![MCP Server CI/CD](https://github.com/tassosgomes/stackview/actions/workflows/mcp.yml/badge.svg)](https://github.com/tassosgomes/stackview/actions/workflows/mcp.yml)
[![Security Scan](https://github.com/tassosgomes/stackview/actions/workflows/security.yml/badge.svg)](https://github.com/tassosgomes/stackview/actions/workflows/security.yml)

Plataforma web para desenvolvedores compartilharem e descobrirem tech stacks, com integração MCP para assistentes de IA como GitHub Copilot e Claude Desktop.

## 🚀 CI/CD Pipeline

Este projeto utiliza uma arquitetura de CI/CD moderna com pipelines especializados:

- **Backend Pipeline**: Testes unitários/integração, build .NET, Docker publish
- **Frontend Pipeline**: ESLint, TypeScript, testes E2E com Playwright, Lighthouse
- **MCP Pipeline**: Build e testes do servidor Model Context Protocol  
- **Security Pipeline**: Scans de segurança, updates automáticos de dependências
- **Release Pipeline**: Criação de releases com pacotes de deployment

### 📊 Quality Gates

- ✅ Testes unitários e integração (xUnit + Testcontainers)
- ✅ Testes E2E multi-browser (Playwright)
- ✅ Lint e type checking (ESLint + TypeScript)
- ✅ Security scanning (Trivy + CodeQL)  
- ✅ Performance audit (Lighthouse)
- ✅ Multi-architecture Docker builds (amd64/arm64)

## 📦 Container Images

Todas as imagens são publicadas no GitHub Container Registry:

- **API**: `ghcr.io/tassosgomes/stackview/stackshare-api:latest`
- **Frontend**: `ghcr.io/tassosgomes/stackview/stackshare-frontend:latest`  
- **MCP Server**: `ghcr.io/tassosgomes/stackview/stackshare-mcp:latest`

## 📖 Documentação

- [CI/CD Pipeline](docs/ci-cd.md) - Documentação completa dos pipelines
- [Docker Setup](DOCKER.md) - Instruções de containerização
- [PRD](docs/prd.md) - Product Requirements Document
- [Tech Spec](tasks/prd-clone-simplificado-stackshare/techspec.md) - Especificação Técnica