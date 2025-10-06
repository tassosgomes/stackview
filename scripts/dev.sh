#!/bin/bash

# StackView Docker Management Script
# Alternative to Makefile for cross-platform compatibility

set -e

# Colors
RESET='\033[0m'
BOLD='\033[1m'
GREEN='\033[32m'
YELLOW='\033[33m'
BLUE='\033[34m'
RED='\033[31m'

# Variables
COMPOSE_FILE="docker-compose.yml"
ENV_FILE=".env"

show_help() {
    echo -e "${BOLD}StackView Development Environment${RESET}"
    echo ""
    echo -e "${BOLD}Usage:${RESET} ./scripts/dev.sh [command]"
    echo ""
    echo -e "${BOLD}Available commands:${RESET}"
    echo -e "  ${BLUE}setup${RESET}              Setup development environment"
    echo -e "  ${BLUE}build${RESET}              Build all Docker images"
    echo -e "  ${BLUE}up${RESET}                 Start all services"
    echo -e "  ${BLUE}up-logs${RESET}            Start all services with logs"
    echo -e "  ${BLUE}up-observability${RESET}   Start with observability stack"
    echo -e "  ${BLUE}down${RESET}               Stop all services"
    echo -e "  ${BLUE}down-volumes${RESET}       Stop services and remove volumes"
    echo -e "  ${BLUE}restart${RESET}            Restart all services"
    echo -e "  ${BLUE}logs${RESET}               Show logs from all services"
    echo -e "  ${BLUE}logs-api${RESET}           Show API logs"
    echo -e "  ${BLUE}logs-frontend${RESET}      Show frontend logs"
    echo -e "  ${BLUE}logs-mcp${RESET}           Show MCP server logs"
    echo -e "  ${BLUE}logs-db${RESET}            Show database logs"
    echo -e "  ${BLUE}status${RESET}             Show status of all services"
    echo -e "  ${BLUE}health${RESET}             Check health of all services"
    echo -e "  ${BLUE}shell-api${RESET}          Open shell in API container"
    echo -e "  ${BLUE}shell-frontend${RESET}     Open shell in frontend container"
    echo -e "  ${BLUE}shell-db${RESET}           Open PostgreSQL shell"
    echo -e "  ${BLUE}migrate${RESET}            Run database migrations"
    echo -e "  ${BLUE}clean${RESET}              Clean up Docker resources"
    echo -e "  ${BLUE}rebuild${RESET}            Full rebuild of all services"
    echo -e "  ${BLUE}dev${RESET}                Start development environment only"
    echo -e "  ${BLUE}backup-db${RESET}          Backup database"
    echo -e "  ${BLUE}help${RESET}               Show this help message"
}

setup_env() {
    echo -e "${YELLOW}Setting up development environment...${RESET}"
    if [ ! -f "$ENV_FILE" ]; then
        cp .env.example "$ENV_FILE"
        echo -e "${GREEN}Created $ENV_FILE from template${RESET}"
    else
        echo -e "${YELLOW}$ENV_FILE already exists${RESET}"
    fi
    echo -e "${GREEN}Setup completed!${RESET}"
}

build_images() {
    echo -e "${YELLOW}Building Docker images...${RESET}"
    docker compose -f "$COMPOSE_FILE" build
    echo -e "${GREEN}Build completed!${RESET}"
}

start_services() {
    setup_env
    echo -e "${YELLOW}Starting all services...${RESET}"
    docker compose -f "$COMPOSE_FILE" up -d
    echo -e "${GREEN}All services are running!${RESET}"
    echo ""
    echo -e "${BOLD}Services:${RESET}"
    echo -e "  ${BLUE}Frontend:${RESET} http://localhost:3000"
    echo -e "  ${BLUE}API:${RESET}      http://localhost:5001"
    echo -e "  ${BLUE}MCP:${RESET}      http://localhost:5002"
    echo -e "  ${BLUE}Database:${RESET} postgresql://localhost:5432"
}

start_with_logs() {
    setup_env
    echo -e "${YELLOW}Starting all services with logs...${RESET}"
    docker compose -f "$COMPOSE_FILE" up
}

start_with_observability() {
    setup_env
    echo -e "${YELLOW}Starting services with observability...${RESET}"
    docker compose -f "$COMPOSE_FILE" --profile observability up -d
    echo -e "${GREEN}All services including observability are running!${RESET}"
    echo ""
    echo -e "${BOLD}Services:${RESET}"
    echo -e "  ${BLUE}Frontend:${RESET}   http://localhost:3000"
    echo -e "  ${BLUE}API:${RESET}        http://localhost:5001"
    echo -e "  ${BLUE}MCP:${RESET}        http://localhost:5002"
    echo -e "  ${BLUE}Database:${RESET}   postgresql://localhost:5432"
    echo -e "  ${BLUE}Jaeger UI:${RESET}  http://localhost:16686"
}

stop_services() {
    echo -e "${YELLOW}Stopping all services...${RESET}"
    docker compose -f "$COMPOSE_FILE" down
    echo -e "${GREEN}All services stopped!${RESET}"
}

stop_with_volumes() {
    echo -e "${YELLOW}Stopping all services and removing volumes...${RESET}"
    docker compose -f "$COMPOSE_FILE" down -v
    echo -e "${GREEN}All services stopped and volumes removed!${RESET}"
}

restart_services() {
    stop_services
    start_services
}

show_logs() {
    docker compose -f "$COMPOSE_FILE" logs -f
}

show_logs_service() {
    local service=$1
    docker compose -f "$COMPOSE_FILE" logs -f "$service"
}

show_status() {
    echo -e "${BOLD}Service Status:${RESET}"
    docker compose -f "$COMPOSE_FILE" ps
}

show_health() {
    echo -e "${BOLD}Health Checks:${RESET}"
    docker compose -f "$COMPOSE_FILE" ps --format "table {{.Service}}\t{{.Status}}\t{{.Ports}}"
}

open_shell() {
    local service=$1
    local shell=${2:-/bin/bash}
    docker compose -f "$COMPOSE_FILE" exec "$service" "$shell"
}

run_migrations() {
    echo -e "${YELLOW}Running database migrations...${RESET}"
    docker compose -f "$COMPOSE_FILE" exec api dotnet ef database update
    echo -e "${GREEN}Migrations completed!${RESET}"
}

clean_docker() {
    echo -e "${YELLOW}Cleaning up Docker resources...${RESET}"
    docker compose -f "$COMPOSE_FILE" down -v --remove-orphans
    docker system prune -f
    echo -e "${GREEN}Cleanup completed!${RESET}"
}

rebuild_all() {
    stop_services
    clean_docker
    build_images
    start_services
}

start_dev() {
    echo -e "${YELLOW}Starting development environment...${RESET}"
    docker compose -f "$COMPOSE_FILE" up -d postgres
    echo -e "${GREEN}Development environment ready!${RESET}"
    echo ""
    echo -e "${BOLD}Next steps:${RESET}"
    echo -e "  1. Start API: cd backend && dotnet run --project src/StackShare.API"
    echo -e "  2. Start Frontend: cd frontend && npm run dev"
    echo -e "  3. Start MCP: cd backend && dotnet run --project src/StackShare.McpServer"
}

backup_database() {
    echo -e "${YELLOW}Creating database backup...${RESET}"
    mkdir -p backups
    timestamp=$(date +%Y%m%d_%H%M%S)
    docker compose -f "$COMPOSE_FILE" exec postgres pg_dump -U stackshare stackshare | gzip > "backups/stackshare_${timestamp}.sql.gz"
    echo -e "${GREEN}Database backup created in backups/ directory${RESET}"
}

# Main command dispatcher
case "${1:-help}" in
    "setup")
        setup_env
        ;;
    "build")
        build_images
        ;;
    "up")
        start_services
        ;;
    "up-logs")
        start_with_logs
        ;;
    "up-observability")
        start_with_observability
        ;;
    "down")
        stop_services
        ;;
    "down-volumes")
        stop_with_volumes
        ;;
    "restart")
        restart_services
        ;;
    "logs")
        show_logs
        ;;
    "logs-api")
        show_logs_service "api"
        ;;
    "logs-frontend")
        show_logs_service "frontend"
        ;;
    "logs-mcp")
        show_logs_service "mcp"
        ;;
    "logs-db")
        show_logs_service "postgres"
        ;;
    "status")
        show_status
        ;;
    "health")
        show_health
        ;;
    "shell-api")
        open_shell "api"
        ;;
    "shell-frontend")
        open_shell "frontend" "/bin/sh"
        ;;
    "shell-db")
        docker compose -f "$COMPOSE_FILE" exec postgres psql -U stackshare -d stackshare
        ;;
    "migrate")
        run_migrations
        ;;
    "clean")
        clean_docker
        ;;
    "rebuild")
        rebuild_all
        ;;
    "dev")
        start_dev
        ;;
    "backup-db")
        backup_database
        ;;
    "help"|*)
        show_help
        ;;
esac