# StackView Docker Setup

This document provides comprehensive instructions for running StackView using Docker and Docker Compose.

## 🏗️ Architecture

StackView consists of the following services:

- **Frontend** (React + TypeScript + Vite): Port 3000
- **API** (.NET 8 Web API): Port 5001  
- **MCP Server** (.NET 8 Worker): Port 5002
- **PostgreSQL Database**: Port 5432
- **Jaeger** (Optional - Observability): Port 16686

## 📋 Prerequisites

- [Docker](https://docs.docker.com/get-docker/) (version 20.10+)
- [Docker Compose](https://docs.docker.com/compose/install/) (version 2.0+)
- [Make](https://www.gnu.org/software/make/) (optional, for convenience commands)

### Verify Installation

```bash
docker --version
docker compose version
```

## 🚀 Quick Start

### Option 1: Using Make (Recommended)

```bash
# Setup and start all services
make up

# View service status
make status

# View logs
make logs
```

### Option 2: Using Scripts

```bash
# Linux/macOS
./scripts/dev.sh up

# Windows PowerShell
.\scripts\dev.ps1 up
```

### Option 3: Direct Docker Compose

```bash
# Setup environment
cp .env.example .env

# Start all services
docker compose up -d
```

## 🔧 Available Commands

### Make Commands

| Command | Description |
|---------|-------------|
| `make up` | Start all services |
| `make down` | Stop all services |
| `make build` | Build all Docker images |
| `make logs` | Show logs from all services |
| `make status` | Show service status |
| `make clean` | Clean up Docker resources |
| `make restart` | Restart all services |
| `make rebuild` | Full rebuild and restart |

### Service-Specific Commands

| Command | Description |
|---------|-------------|
| `make logs-api` | Show API logs |
| `make logs-frontend` | Show frontend logs |
| `make logs-mcp` | Show MCP server logs |
| `make logs-db` | Show database logs |
| `make shell-api` | Open shell in API container |
| `make shell-db` | Open PostgreSQL shell |

### Development Commands

| Command | Description |
|---------|-------------|
| `make dev` | Start only database for local development |
| `make migrate` | Run database migrations |
| `make backup-db` | Backup database |

### Observability

```bash
# Start with Jaeger tracing
make up-observability

# Access Jaeger UI
open http://localhost:16686
```

## ⚙️ Configuration

### Environment Variables

Copy `.env.example` to `.env` and customize:

```bash
cp .env.example .env
```

Key configuration options:

| Variable | Default | Description |
|----------|---------|-------------|
| `POSTGRES_DB` | stackshare | Database name |
| `POSTGRES_USER` | stackshare | Database user |
| `POSTGRES_PASSWORD` | stackshare123 | Database password |
| `JWT_SECRET` | dev-jwt-secret... | JWT signing key |
| `MCP_API_KEY` | dev-mcp-key | MCP service API key |
| `LOG_LEVEL` | Information | Logging level |

### Ports

| Service | Port | URL |
|---------|------|-----|
| Frontend | 3000 | http://localhost:3000 |
| API | 5001 | http://localhost:5001 |
| MCP Server | 5002 | http://localhost:5002 |
| Database | 5432 | postgresql://localhost:5432 |
| Jaeger UI | 16686 | http://localhost:16686 |

## 🏃‍♂️ Development Workflow

### Full Stack Development

1. **Start all services:**
   ```bash
   make up
   ```

2. **Verify services are running:**
   ```bash
   make health
   ```

3. **Access applications:**
   - Frontend: http://localhost:3000
   - API: http://localhost:5001/swagger
   - Database: Connect using any PostgreSQL client

### Local Development (Recommended)

For faster development cycles, run only the database in Docker:

1. **Start database only:**
   ```bash
   make dev
   ```

2. **Run services locally:**
   ```bash
   # Terminal 1: API
   cd backend
   dotnet run --project src/StackShare.API

   # Terminal 2: Frontend  
   cd frontend
   npm run dev

   # Terminal 3: MCP Server
   cd backend
   dotnet run --project src/StackShare.McpServer
   ```

### Database Management

```bash
# Run migrations
make migrate

# Access database shell
make shell-db

# Backup database
make backup-db

# Restore database
make restore-db BACKUP_FILE=backups/file.sql.gz
```

## 🐛 Troubleshooting

### Common Issues

#### Port Already in Use

```bash
# Check what's using the port
lsof -i :3000
netstat -tulpn | grep :3000

# Kill process using port
kill -9 <PID>
```

#### Permission Denied

```bash
# Fix Docker permissions (Linux)
sudo usermod -aG docker $USER
newgrp docker

# Make scripts executable
chmod +x scripts/dev.sh
```

#### Container Won't Start

```bash
# Check container logs
docker logs stackview-api
make logs-api

# Check container status
docker ps -a
make status
```

#### Database Connection Issues

```bash
# Check database logs
make logs-db

# Verify database is ready
docker compose exec postgres pg_isready -U stackshare
```

### Clean Start

If you encounter persistent issues:

```bash
# Complete cleanup and restart
make clean
make rebuild
```

### View Service Health

```bash
# Check health status
make health

# Test specific endpoints
curl http://localhost:5001/health
curl http://localhost:3000/health
```

## 📊 Monitoring & Observability

### Health Checks

All services include health checks:

- **Frontend**: `GET /health`
- **API**: `GET /health` 
- **Database**: PostgreSQL `pg_isready`

### Logging

Structured logging with Serilog:

```bash
# View all logs
make logs

# View API logs only
make logs-api

# Follow logs in real-time
docker compose logs -f
```

### Distributed Tracing

Start with observability stack:

```bash
make up-observability
```

Access Jaeger UI at http://localhost:16686

## 🔒 Security Notes

### Development vs Production

The current setup is optimized for development. For production:

1. **Change default passwords:**
   ```bash
   # Generate secure passwords
   openssl rand -base64 32
   ```

2. **Use secrets management:**
   - Docker Secrets
   - Kubernetes Secrets
   - External secret providers

3. **Enable HTTPS:**
   - Add TLS certificates
   - Configure reverse proxy

4. **Harden containers:**
   - Use distroless images
   - Run as non-root user
   - Scan for vulnerabilities

### Network Security

Services communicate via Docker networks:
- `stackview-network`: Internal communication
- Only necessary ports exposed to host

## 📦 Production Deployment

### Docker Compose Production

1. **Create production environment:**
   ```bash
   cp .env.example .env.production
   # Edit with production values
   ```

2. **Override for production:**
   ```yaml
   # docker-compose.prod.yml
   version: '3.8'
   services:
     api:
       environment:
         ASPNETCORE_ENVIRONMENT: Production
   ```

3. **Deploy:**
   ```bash
   docker compose -f docker-compose.yml -f docker-compose.prod.yml up -d
   ```

### Kubernetes

Helm charts and Kubernetes manifests available in `k8s/` directory.

### CI/CD

GitHub Actions workflows in `.github/workflows/` for:
- Building and pushing images
- Running tests
- Deploying to environments

## 🤝 Contributing

When working with Docker:

1. **Test your changes:**
   ```bash
   make rebuild
   ```

2. **Update documentation:**
   - Update this README
   - Update docker-compose.yml comments

3. **Follow best practices:**
   - Use multi-stage builds
   - Minimize image layers
   - Use .dockerignore effectively

## 📞 Support

For Docker-related issues:

1. Check troubleshooting section above
2. Review container logs: `make logs`
3. Verify service health: `make health`
4. Try clean rebuild: `make rebuild`

For application issues, see the main project README.