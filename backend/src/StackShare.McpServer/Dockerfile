# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy solution and project files
COPY ["StackShare.sln", "."]
COPY ["src/StackShare.McpServer/StackShare.McpServer.csproj", "src/StackShare.McpServer/"]
COPY ["src/StackShare.Application/StackShare.Application.csproj", "src/StackShare.Application/"]
COPY ["src/StackShare.Domain/StackShare.Domain.csproj", "src/StackShare.Domain/"]
COPY ["src/StackShare.Infrastructure/StackShare.Infrastructure.csproj", "src/StackShare.Infrastructure/"]

# Restore dependencies
RUN dotnet restore "src/StackShare.McpServer/StackShare.McpServer.csproj"

# Copy source code
COPY . .

# Build the application
WORKDIR "/src/src/StackShare.McpServer"
RUN dotnet build "StackShare.McpServer.csproj" -c Release -o /app/build

# Publish stage
FROM build AS publish
RUN dotnet publish "StackShare.McpServer.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

# Create non-root user for security
RUN adduser --disabled-password --gecos '' --shell /bin/bash --home /app appuser

# Copy published app
COPY --from=publish /app/publish .

# Set ownership and permissions
RUN chown -R appuser:appuser /app
USER appuser

# Expose port (MCP typically uses stdio, but may need HTTP)
EXPOSE 8081

ENTRYPOINT ["dotnet", "StackShare.McpServer.dll"]