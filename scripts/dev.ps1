# StackView Docker Management Script for Windows PowerShell
# Alternative to Makefile for Windows users

param(
    [Parameter(Position=0)]
    [string]$Command = "help"
)

# Variables
$ComposeFile = "docker-compose.yml"
$EnvFile = ".env"

function Show-Help {
    Write-Host "StackView Development Environment" -ForegroundColor Cyan
    Write-Host ""
    Write-Host "Usage: .\scripts\dev.ps1 [command]" -ForegroundColor Yellow
    Write-Host ""
    Write-Host "Available commands:" -ForegroundColor Cyan
    Write-Host "  setup              " -NoNewline -ForegroundColor Blue
    Write-Host "Setup development environment"
    Write-Host "  build              " -NoNewline -ForegroundColor Blue
    Write-Host "Build all Docker images"
    Write-Host "  up                 " -NoNewline -ForegroundColor Blue
    Write-Host "Start all services"
    Write-Host "  up-logs            " -NoNewline -ForegroundColor Blue
    Write-Host "Start all services with logs"
    Write-Host "  up-observability   " -NoNewline -ForegroundColor Blue
    Write-Host "Start with observability stack"
    Write-Host "  down               " -NoNewline -ForegroundColor Blue
    Write-Host "Stop all services"
    Write-Host "  down-volumes       " -NoNewline -ForegroundColor Blue
    Write-Host "Stop services and remove volumes"
    Write-Host "  restart            " -NoNewline -ForegroundColor Blue
    Write-Host "Restart all services"
    Write-Host "  logs               " -NoNewline -ForegroundColor Blue
    Write-Host "Show logs from all services"
    Write-Host "  logs-api           " -NoNewline -ForegroundColor Blue
    Write-Host "Show API logs"
    Write-Host "  logs-frontend      " -NoNewline -ForegroundColor Blue
    Write-Host "Show frontend logs"
    Write-Host "  logs-mcp           " -NoNewline -ForegroundColor Blue
    Write-Host "Show MCP server logs"
    Write-Host "  logs-db            " -NoNewline -ForegroundColor Blue
    Write-Host "Show database logs"
    Write-Host "  status             " -NoNewline -ForegroundColor Blue
    Write-Host "Show status of all services"
    Write-Host "  health             " -NoNewline -ForegroundColor Blue
    Write-Host "Check health of all services"
    Write-Host "  clean              " -NoNewline -ForegroundColor Blue
    Write-Host "Clean up Docker resources"
    Write-Host "  rebuild            " -NoNewline -ForegroundColor Blue
    Write-Host "Full rebuild of all services"
    Write-Host "  dev                " -NoNewline -ForegroundColor Blue
    Write-Host "Start development environment only"
    Write-Host "  backup-db          " -NoNewline -ForegroundColor Blue
    Write-Host "Backup database"
    Write-Host "  help               " -NoNewline -ForegroundColor Blue
    Write-Host "Show this help message"
}

function Setup-Environment {
    Write-Host "Setting up development environment..." -ForegroundColor Yellow
    if (-not (Test-Path $EnvFile)) {
        Copy-Item ".env.example" $EnvFile
        Write-Host "Created $EnvFile from template" -ForegroundColor Green
    } else {
        Write-Host "$EnvFile already exists" -ForegroundColor Yellow
    }
    Write-Host "Setup completed!" -ForegroundColor Green
}

function Build-Images {
    Write-Host "Building Docker images..." -ForegroundColor Yellow
    docker compose -f $ComposeFile build
    if ($LASTEXITCODE -eq 0) {
        Write-Host "Build completed!" -ForegroundColor Green
    }
}

function Start-Services {
    Setup-Environment
    Write-Host "Starting all services..." -ForegroundColor Yellow
    docker compose -f $ComposeFile up -d
    if ($LASTEXITCODE -eq 0) {
        Write-Host "All services are running!" -ForegroundColor Green
        Write-Host ""
        Write-Host "Services:" -ForegroundColor Cyan
        Write-Host "  Frontend: " -NoNewline -ForegroundColor Blue
        Write-Host "http://localhost:3000"
        Write-Host "  API:      " -NoNewline -ForegroundColor Blue
        Write-Host "http://localhost:5001"
        Write-Host "  MCP:      " -NoNewline -ForegroundColor Blue
        Write-Host "http://localhost:5002"
        Write-Host "  Database: " -NoNewline -ForegroundColor Blue
        Write-Host "postgresql://localhost:5432"
    }
}

function Start-WithLogs {
    Setup-Environment
    Write-Host "Starting all services with logs..." -ForegroundColor Yellow
    docker compose -f $ComposeFile up
}

function Start-WithObservability {
    Setup-Environment
    Write-Host "Starting services with observability..." -ForegroundColor Yellow
    docker compose -f $ComposeFile --profile observability up -d
    if ($LASTEXITCODE -eq 0) {
        Write-Host "All services including observability are running!" -ForegroundColor Green
        Write-Host ""
        Write-Host "Services:" -ForegroundColor Cyan
        Write-Host "  Frontend:   " -NoNewline -ForegroundColor Blue
        Write-Host "http://localhost:3000"
        Write-Host "  API:        " -NoNewline -ForegroundColor Blue
        Write-Host "http://localhost:5001"
        Write-Host "  MCP:        " -NoNewline -ForegroundColor Blue
        Write-Host "http://localhost:5002"
        Write-Host "  Database:   " -NoNewline -ForegroundColor Blue
        Write-Host "postgresql://localhost:5432"
        Write-Host "  Jaeger UI:  " -NoNewline -ForegroundColor Blue
        Write-Host "http://localhost:16686"
    }
}

function Stop-Services {
    Write-Host "Stopping all services..." -ForegroundColor Yellow
    docker compose -f $ComposeFile down
    if ($LASTEXITCODE -eq 0) {
        Write-Host "All services stopped!" -ForegroundColor Green
    }
}

function Stop-WithVolumes {
    Write-Host "Stopping all services and removing volumes..." -ForegroundColor Yellow
    docker compose -f $ComposeFile down -v
    if ($LASTEXITCODE -eq 0) {
        Write-Host "All services stopped and volumes removed!" -ForegroundColor Green
    }
}

function Restart-Services {
    Stop-Services
    Start-Services
}

function Show-Logs {
    docker compose -f $ComposeFile logs -f
}

function Show-LogsService($Service) {
    docker compose -f $ComposeFile logs -f $Service
}

function Show-Status {
    Write-Host "Service Status:" -ForegroundColor Cyan
    docker compose -f $ComposeFile ps
}

function Show-Health {
    Write-Host "Health Checks:" -ForegroundColor Cyan
    docker compose -f $ComposeFile ps --format "table {{.Service}}\t{{.Status}}\t{{.Ports}}"
}

function Clean-Docker {
    Write-Host "Cleaning up Docker resources..." -ForegroundColor Yellow
    docker compose -f $ComposeFile down -v --remove-orphans
    docker system prune -f
    if ($LASTEXITCODE -eq 0) {
        Write-Host "Cleanup completed!" -ForegroundColor Green
    }
}

function Rebuild-All {
    Stop-Services
    Clean-Docker
    Build-Images
    Start-Services
}

function Start-Dev {
    Write-Host "Starting development environment..." -ForegroundColor Yellow
    docker compose -f $ComposeFile up -d postgres
    if ($LASTEXITCODE -eq 0) {
        Write-Host "Development environment ready!" -ForegroundColor Green
        Write-Host ""
        Write-Host "Next steps:" -ForegroundColor Cyan
        Write-Host "  1. Start API: cd backend && dotnet run --project src/StackShare.API"
        Write-Host "  2. Start Frontend: cd frontend && npm run dev"
        Write-Host "  3. Start MCP: cd backend && dotnet run --project src/StackShare.McpServer"
    }
}

function Backup-Database {
    Write-Host "Creating database backup..." -ForegroundColor Yellow
    if (-not (Test-Path "backups")) {
        New-Item -ItemType Directory -Path "backups" -Force | Out-Null
    }
    $timestamp = Get-Date -Format "yyyyMMdd_HHmmss"
    docker compose -f $ComposeFile exec postgres pg_dump -U stackshare stackshare | gzip > "backups/stackshare_$timestamp.sql.gz"
    Write-Host "Database backup created in backups/ directory" -ForegroundColor Green
}

# Main command dispatcher
switch ($Command.ToLower()) {
    "setup" { Setup-Environment }
    "build" { Build-Images }
    "up" { Start-Services }
    "up-logs" { Start-WithLogs }
    "up-observability" { Start-WithObservability }
    "down" { Stop-Services }
    "down-volumes" { Stop-WithVolumes }
    "restart" { Restart-Services }
    "logs" { Show-Logs }
    "logs-api" { Show-LogsService "api" }
    "logs-frontend" { Show-LogsService "frontend" }
    "logs-mcp" { Show-LogsService "mcp" }
    "logs-db" { Show-LogsService "postgres" }
    "status" { Show-Status }
    "health" { Show-Health }
    "clean" { Clean-Docker }
    "rebuild" { Rebuild-All }
    "dev" { Start-Dev }
    "backup-db" { Backup-Database }
    default { Show-Help }
}