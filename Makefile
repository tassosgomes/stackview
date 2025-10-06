# StackView Docker Management
.DEFAULT_GOAL := help

# Variables
COMPOSE_FILE := docker-compose.yml
ENV_FILE := .env

# Colors for output
RESET := \033[0m
BOLD := \033[1m
GREEN := \033[32m
YELLOW := \033[33m
BLUE := \033[34m
RED := \033[31m

.PHONY: help
help: ## Show this help message
	@echo "$(BOLD)StackView Development Environment$(RESET)"
	@echo ""
	@echo "$(BOLD)Available commands:$(RESET)"
	@awk 'BEGIN {FS = ":.*##"} /^[a-zA-Z0-9_-]+:.*##/ {printf "  $(BLUE)%-15s$(RESET) %s\n", $$1, $$2}' $(MAKEFILE_LIST)

.PHONY: setup
setup: ## Setup development environment (copy .env file if needed)
	@echo "$(YELLOW)Setting up development environment...$(RESET)"
	@if [ ! -f $(ENV_FILE) ]; then \
		cp .env.example $(ENV_FILE); \
		echo "$(GREEN)Created $(ENV_FILE) from template$(RESET)"; \
	else \
		echo "$(YELLOW)$(ENV_FILE) already exists$(RESET)"; \
	fi
	@echo "$(GREEN)Setup completed!$(RESET)"

.PHONY: build
build: ## Build all Docker images
	@echo "$(YELLOW)Building Docker images...$(RESET)"
	@docker compose -f $(COMPOSE_FILE) build
	@echo "$(GREEN)Build completed!$(RESET)"

.PHONY: up
up: setup ## Start all services
	@echo "$(YELLOW)Starting all services...$(RESET)"
	@docker compose -f $(COMPOSE_FILE) up -d
	@echo "$(GREEN)All services are running!$(RESET)"
	@echo ""
	@echo "$(BOLD)Services:$(RESET)"
	@echo "  $(BLUE)Frontend:$(RESET) http://localhost:3000"
	@echo "  $(BLUE)API:$(RESET)      http://localhost:5001"
	@echo "  $(BLUE)MCP:$(RESET)      http://localhost:5002"
	@echo "  $(BLUE)Database:$(RESET) postgresql://localhost:5432"

.PHONY: up-with-logs
up-logs: setup ## Start all services with logs
	@echo "$(YELLOW)Starting all services with logs...$(RESET)"
	@docker compose -f $(COMPOSE_FILE) up

.PHONY: up-observability
up-observability: setup ## Start all services including observability stack
	@echo "$(YELLOW)Starting services with observability...$(RESET)"
	@docker compose -f $(COMPOSE_FILE) --profile observability up -d
	@echo "$(GREEN)All services including observability are running!$(RESET)"
	@echo ""
	@echo "$(BOLD)Services:$(RESET)"
	@echo "  $(BLUE)Frontend:$(RESET)   http://localhost:3000"
	@echo "  $(BLUE)API:$(RESET)        http://localhost:5001"
	@echo "  $(BLUE)MCP:$(RESET)        http://localhost:5002"
	@echo "  $(BLUE)Database:$(RESET)   postgresql://localhost:5432"
	@echo "  $(BLUE)Jaeger UI:$(RESET)  http://localhost:16686"

.PHONY: down
down: ## Stop all services
	@echo "$(YELLOW)Stopping all services...$(RESET)"
	@docker compose -f $(COMPOSE_FILE) down
	@echo "$(GREEN)All services stopped!$(RESET)"

.PHONY: down-volumes
down-volumes: ## Stop all services and remove volumes
	@echo "$(YELLOW)Stopping all services and removing volumes...$(RESET)"
	@docker compose -f $(COMPOSE_FILE) down -v
	@echo "$(GREEN)All services stopped and volumes removed!$(RESET)"

.PHONY: restart
restart: down up ## Restart all services

.PHONY: logs
logs: ## Show logs from all services
	@docker compose -f $(COMPOSE_FILE) logs -f

.PHONY: logs-api
logs-api: ## Show API logs
	@docker compose -f $(COMPOSE_FILE) logs -f api

.PHONY: logs-frontend
logs-frontend: ## Show frontend logs
	@docker compose -f $(COMPOSE_FILE) logs -f frontend

.PHONY: logs-mcp
logs-mcp: ## Show MCP server logs
	@docker compose -f $(COMPOSE_FILE) logs -f mcp

.PHONY: logs-db
logs-db: ## Show database logs
	@docker compose -f $(COMPOSE_FILE) logs -f postgres

.PHONY: status
status: ## Show status of all services
	@echo "$(BOLD)Service Status:$(RESET)"
	@docker compose -f $(COMPOSE_FILE) ps

.PHONY: health
health: ## Check health of all services
	@echo "$(BOLD)Health Checks:$(RESET)"
	@docker compose -f $(COMPOSE_FILE) ps --format "table {{.Service}}\t{{.Status}}\t{{.Ports}}"

.PHONY: shell-api
shell-api: ## Open shell in API container
	@docker compose -f $(COMPOSE_FILE) exec api /bin/bash

.PHONY: shell-frontend
shell-frontend: ## Open shell in frontend container  
	@docker compose -f $(COMPOSE_FILE) exec frontend /bin/sh

.PHONY: shell-db
shell-db: ## Open PostgreSQL shell
	@docker compose -f $(COMPOSE_FILE) exec postgres psql -U stackshare -d stackshare

.PHONY: migrate
migrate: ## Run database migrations (when API is running)
	@echo "$(YELLOW)Running database migrations...$(RESET)"
	@docker compose -f $(COMPOSE_FILE) exec api dotnet ef database update
	@echo "$(GREEN)Migrations completed!$(RESET)"

.PHONY: clean
clean: ## Clean up Docker resources
	@echo "$(YELLOW)Cleaning up Docker resources...$(RESET)"
	@docker compose -f $(COMPOSE_FILE) down -v --remove-orphans
	@docker system prune -f
	@echo "$(GREEN)Cleanup completed!$(RESET)"

.PHONY: rebuild
rebuild: down clean build up ## Full rebuild of all services

.PHONY: dev
dev: ## Start development environment (frontend + api + db)
	@echo "$(YELLOW)Starting development environment...$(RESET)"
	@docker compose -f $(COMPOSE_FILE) up -d postgres
	@echo "$(GREEN)Development environment ready!$(RESET)"
	@echo ""
	@echo "$(BOLD)Next steps:$(RESET)"
	@echo "  1. Start API: cd backend && dotnet run --project src/StackShare.API"
	@echo "  2. Start Frontend: cd frontend && npm run dev"
	@echo "  3. Start MCP: cd backend && dotnet run --project src/StackShare.McpServer"

# Backup and restore
.PHONY: backup-db
backup-db: ## Backup database
	@echo "$(YELLOW)Creating database backup...$(RESET)"
	@mkdir -p backups
	@docker compose -f $(COMPOSE_FILE) exec postgres pg_dump -U stackshare stackshare | gzip > backups/stackshare_$(shell date +%Y%m%d_%H%M%S).sql.gz
	@echo "$(GREEN)Database backup created in backups/ directory$(RESET)"

.PHONY: restore-db
restore-db: ## Restore database (usage: make restore-db BACKUP_FILE=backups/file.sql.gz)
ifndef BACKUP_FILE
	@echo "$(RED)Error: Please specify BACKUP_FILE$(RESET)"
	@echo "Usage: make restore-db BACKUP_FILE=backups/file.sql.gz"
	@exit 1
endif
	@echo "$(YELLOW)Restoring database from $(BACKUP_FILE)...$(RESET)"
	@gunzip -c $(BACKUP_FILE) | docker compose -f $(COMPOSE_FILE) exec -T postgres psql -U stackshare stackshare
	@echo "$(GREEN)Database restored!$(RESET)"