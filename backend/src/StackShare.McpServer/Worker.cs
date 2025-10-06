using MCPSharp;
using StackShare.McpServer.Tools;
using StackShare.McpServer.Services;

namespace StackShare.McpServer;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;
    private readonly IConfiguration _configuration;
    private readonly IStackShareApiClient _apiClient;
    private readonly ILogger<StackShareTools> _toolsLogger;

    public Worker(
        ILogger<Worker> logger, 
        IConfiguration configuration, 
        IStackShareApiClient apiClient,
        ILogger<StackShareTools> toolsLogger)
    {
        _logger = logger;
        _configuration = configuration;
        _apiClient = apiClient;
        _toolsLogger = toolsLogger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            var serverName = _configuration.GetValue<string>("McpServer:Name") ?? "StackShare MCP Server";
            var serverVersion = _configuration.GetValue<string>("McpServer:Version") ?? "1.0.0";

            _logger.LogInformation("Inicializando ferramentas MCP...");
            
            // Initialize the StackShare tools with dependencies
            StackShareTools.Initialize(_apiClient, _toolsLogger);
            
            _logger.LogInformation("Registrando ferramentas MCP...");
            
            // Register the StackShare tools class with MCP Server
            MCPServer.Register<StackShareTools>();
            
            _logger.LogInformation("Iniciando servidor MCP: {ServerName} v{Version}", serverName, serverVersion);
            
            // Start the MCP server
            await MCPServer.StartAsync(serverName, serverVersion);
            
            _logger.LogInformation("Servidor MCP iniciado com sucesso!");
            
            // Keep the service running
            await Task.Delay(Timeout.Infinite, stoppingToken);
        }
        catch (OperationCanceledException)
        {
            _logger.LogInformation("Servidor MCP foi parado");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro fatal no servidor MCP");
            throw;
        }
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Parando servidor MCP...");
        await base.StopAsync(cancellationToken);
        _logger.LogInformation("Servidor MCP parado");
    }
}
