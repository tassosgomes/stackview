# 🚀 CI/CD Pipeline Documentation

This document describes the comprehensive CI/CD pipeline implementation for StackShare, covering automated testing, building, security scanning, and deployment processes.

## 🏗️ Pipeline Architecture

The CI/CD system consists of multiple specialized workflows that work together to ensure code quality, security, and reliable deployments:

```
┌─────────────────┐    ┌──────────────────┐    ┌─────────────────┐
│   Push/PR       │───▶│  Change Detection │───▶│  Targeted Builds │
│   to main/dev   │    │   (paths-filter)  │    │   (backend/      │
└─────────────────┘    └──────────────────┘    │   frontend/mcp)  │
                                               └─────────────────┘
                                                        │
┌─────────────────┐    ┌──────────────────┐    ┌─────────────────┐
│  Security Scan  │◀───│  Docker Registry │◀───│  Quality Gates   │
│  (Trivy/CodeQL) │    │   (ghcr.io)      │    │  (tests/lint)   │
└─────────────────┘    └──────────────────┘    └─────────────────┘
                                               
┌─────────────────┐    ┌──────────────────┐    ┌─────────────────┐
│   Release       │───▶│  Full Stack      │───▶│   Deployment    │
│   (tags only)   │    │  Integration     │    │   Package       │
└─────────────────┘    └──────────────────┘    └─────────────────┘
```

## 📋 Workflows Overview

### 1. Backend CI/CD (`backend.yml`)

**Trigger**: Changes to `backend/**` or workflow file
**Purpose**: Build, test, and publish backend API and MCP server

**Jobs**:
- **Test**: Unit + Integration tests with PostgreSQL
- **Build & Push**: Docker images for API and MCP
- **Security Scan**: Trivy vulnerability scanning

**Key Features**:
- .NET matrix testing (8.0.x)
- Testcontainers for integration tests
- Multi-architecture builds (amd64, arm64)
- Code coverage reporting
- Cached NuGet packages

### 2. Frontend CI/CD (`frontend.yml`)

**Trigger**: Changes to `frontend/**` or workflow file
**Purpose**: Build, test, lint, and publish frontend application

**Jobs**:
- **Lint & Test**: ESLint, TypeScript, unit tests
- **E2E Tests**: Full stack Playwright testing
- **Build & Push**: Docker image for frontend
- **Security Scan**: Trivy vulnerability scanning
- **Lighthouse**: Performance auditing (main branch only)

**Key Features**:
- Node.js caching
- Multi-browser E2E testing
- Performance monitoring
- Bundle size tracking

### 3. MCP Server CI/CD (`mcp.yml`)

**Trigger**: Changes to MCP server code or workflow file
**Purpose**: Specialized pipeline for Model Context Protocol server

**Jobs**:
- **Test**: MCP-specific testing
- **Build & Push**: Docker image for MCP server
- **Integration Test**: End-to-end MCP connectivity
- **Security Scan**: Vulnerability assessment

### 4. Full Stack Orchestration (`main.yml`)

**Trigger**: Push/PR to main branch
**Purpose**: Coordinate all pipelines and run integration tests

**Jobs**:
- **Change Detection**: Determine which components changed
- **Workflow Orchestration**: Call appropriate sub-workflows
- **Full Stack Integration**: Test complete system
- **Deployment Gate**: Quality gate for production readiness

### 5. Security & Dependencies (`security.yml`)

**Trigger**: Daily schedule (2 AM UTC) + manual
**Purpose**: Automated security scanning and dependency updates

**Jobs**:
- **Dependency Review**: Check for vulnerable dependencies
- **Update .NET Dependencies**: Automated minor/patch updates
- **Update NPM Dependencies**: Frontend dependency management
- **Security Audit**: Regular vulnerability scanning
- **CodeQL Analysis**: Static analysis for C# and JavaScript
- **Docker Security Scan**: Container and filesystem scanning

### 6. Release Management (`release.yml`)

**Trigger**: Git tags (`v*`) or manual workflow dispatch
**Purpose**: Create releases with deployment packages

**Jobs**:
- **Create Release**: Generate GitHub release with changelog
- **Build Release Images**: Multi-architecture production images
- **Deployment Package**: Ready-to-deploy bundles
- **Notifications**: Release summaries and deployment instructions

## 🔧 Configuration

### Environment Variables

All workflows use these standard environment variables:

```yaml
env:
  DOTNET_VERSION: '8.0.x'
  NODE_VERSION: '20'
  REGISTRY: ghcr.io
  IMAGE_NAME_API: ${{ github.repository }}/stackshare-api
  IMAGE_NAME_FRONTEND: ${{ github.repository }}/stackshare-frontend
  IMAGE_NAME_MCP: ${{ github.repository }}/stackshare-mcp
```

### Required Secrets

The following secrets must be configured in GitHub repository settings:

| Secret | Purpose | Scope |
|--------|---------|-------|
| `GITHUB_TOKEN` | Container registry access | Auto-provided |
| `LHCI_GITHUB_APP_TOKEN` | Lighthouse CI integration | Optional |

### Docker Registry

All images are published to GitHub Container Registry (ghcr.io):

- **API**: `ghcr.io/tassosgomes/stackview/stackshare-api`
- **Frontend**: `ghcr.io/tassosgomes/stackview/stackshare-frontend`
- **MCP**: `ghcr.io/tassosgomes/stackview/stackshare-mcp`

**Tagging Strategy**:
- `latest`: Latest main branch build
- `develop`: Latest develop branch build
- `sha-<commit>`: Specific commit builds
- `v1.0.0`: Release tags

## 🧪 Quality Gates

### Backend Quality Criteria

- ✅ All unit tests pass (xUnit)
- ✅ All integration tests pass (Testcontainers + PostgreSQL)
- ✅ Code builds without warnings
- ✅ No high/critical security vulnerabilities
- ✅ Docker images build successfully

### Frontend Quality Criteria

- ✅ ESLint passes (no errors)
- ✅ TypeScript compilation succeeds
- ✅ Unit tests pass (Vitest)
- ✅ E2E tests pass (Playwright)
- ✅ Performance audit passes (Lighthouse)
- ✅ No high/critical security vulnerabilities

### Deployment Readiness

- ✅ Backend pipeline success
- ✅ Frontend pipeline success (if changed)
- ✅ MCP pipeline success (if changed)
- ✅ Full stack integration tests pass
- ✅ Security scans complete
- ✅ All images published to registry

## 📊 Monitoring & Observability

### Artifacts & Reports

Each pipeline produces comprehensive artifacts:

- **Test Results**: xUnit XML, Playwright HTML reports
- **Code Coverage**: Codecov integration
- **Security Reports**: SARIF format for GitHub Security tab
- **Performance Reports**: Lighthouse CI integration
- **Build Artifacts**: Compiled applications and Docker images

### Notifications

- **Pull Requests**: Status checks and required reviews
- **Main Branch**: Deployment summaries with quick-deploy commands
- **Releases**: Automated changelogs and deployment packages
- **Security**: GitHub Security tab integration

## 🚀 Deployment Workflows

### Development Deployment

1. **Push to `develop`** → Triggers targeted pipelines
2. **Quality gates pass** → Images tagged as `develop`
3. **Deploy to staging environment** (manual or automated)

### Production Deployment

1. **Create release tag** → `git tag v1.0.0 && git push origin v1.0.0`
2. **Release workflow runs** → Creates GitHub release
3. **Download deployment package** → Ready-to-deploy bundle
4. **Run deployment script** → `./deploy.sh`

### Manual Deployment

Use the deployment package for any environment:

```bash
# Download and extract release
curl -L -o deploy.tar.gz "https://github.com/tassosgomes/stackview/releases/download/v1.0.0/stackshare-v1.0.0-deployment.tar.gz"
tar -xzf deploy.tar.gz

# Configure environment
cp .env.template .env
# Edit .env with your settings

# Deploy
./deploy.sh
```

## 🛠️ Maintenance & Best Practices

### Automated Dependency Updates

The security workflow runs daily to:
- Check for outdated .NET packages
- Check for outdated NPM packages
- Create automated PRs for minor/patch updates
- Ensure security vulnerabilities are addressed promptly

### Performance Optimization

- **Caching**: NPM and NuGet packages cached between runs
- **Parallel Jobs**: Independent components build simultaneously
- **Docker Layer Caching**: GitHub Actions cache for faster image builds
- **Change Detection**: Only affected components are built/tested

### Security Best Practices

- **Least Privilege**: Minimal permissions for each job
- **Secrets Management**: No hardcoded secrets, environment-based configuration
- **Regular Scanning**: Daily Trivy scans for vulnerabilities
- **Dependency Review**: Automatic dependency vulnerability checks
- **SARIF Integration**: Security findings integrated with GitHub Security tab

## 🔍 Troubleshooting

### Common Issues

#### Pipeline Failing on Dependencies
```bash
# Check for dependency conflicts
npm ls --depth=0  # Frontend
dotnet list package --vulnerable  # Backend
```

#### Docker Build Failures
```bash
# Test builds locally
docker build -f backend/src/StackShare.API/Dockerfile .
docker build -f frontend/Dockerfile ./frontend
```

#### E2E Test Failures
```bash
# Run E2E tests locally
cd frontend
npm run test:e2e
```

#### Security Scan Failures
```bash
# Run security scans locally
npm audit  # Frontend
dotnet list package --vulnerable  # Backend
```

### Pipeline Debugging

1. **Check Workflow Logs**: GitHub Actions → Workflow run → Job details
2. **Review Artifacts**: Download test reports and build outputs
3. **Local Reproduction**: Run the same commands locally
4. **Environment Issues**: Verify secrets and environment variables

### Manual Interventions

Force pipeline runs:
```bash
# Trigger specific workflows
gh workflow run backend.yml
gh workflow run frontend.yml
gh workflow run security.yml
```

Skip CI for commits:
```bash
git commit -m "docs: update README [skip ci]"
```

## 📈 Metrics & Analytics

The CI/CD system tracks:
- **Build Success Rate**: Percentage of successful pipeline runs
- **Build Duration**: Time from trigger to completion
- **Test Coverage**: Backend and frontend code coverage trends
- **Security Posture**: Number and severity of vulnerabilities
- **Deployment Frequency**: Release cadence and deployment success rate
- **Performance Metrics**: Lighthouse scores and bundle size trends

All metrics are available through GitHub Actions insights and can be integrated with external monitoring tools.