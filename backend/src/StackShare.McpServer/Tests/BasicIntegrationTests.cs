using System.Net.Http.Json;
using System.Text.Json;
using StackShare.McpServer.Services;
using StackShare.McpServer.Models;
using Microsoft.Extensions.Logging;

namespace StackShare.McpServer.Tests;

/// <summary>
/// Testes básicos de integração para verificar comunicação com a API
/// </summary>
public class BasicIntegrationTests
{
    /// <summary>
    /// Testa se o StackShareApiClient consegue se conectar à API
    /// </summary>
    public static async Task TestApiConnection()
    {
        var httpClient = new HttpClient();
        httpClient.BaseAddress = new Uri("http://localhost:5000/");
        httpClient.DefaultRequestHeaders.Add("User-Agent", "StackShare.McpServer/1.0.0");

        using var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
        var logger = loggerFactory.CreateLogger<StackShareApiClient>();

        var apiClient = new StackShareApiClient(httpClient, logger);

        try
        {
            Console.WriteLine("Testando conexão com API...");

            // Test 1: Get Stacks
            Console.WriteLine("1. Testando busca de stacks...");
            var stacks = await apiClient.GetStacksAsync(page: 1, pageSize: 5);
            Console.WriteLine($"✓ Encontrados {stacks.Items.Count} stacks (Total: {stacks.TotalCount})");

            // Test 2: Get Technologies
            Console.WriteLine("2. Testando busca de tecnologias...");
            var technologies = await apiClient.GetTechnologiesAsync(page: 1, pageSize: 5);
            Console.WriteLine($"✓ Encontradas {technologies.Items.Count} tecnologias (Total: {technologies.TotalCount})");

            // Test 3: Get Stack Details (if any stack exists)
            if (stacks.Items.Count > 0)
            {
                var firstStack = stacks.Items.First();
                Console.WriteLine($"3. Testando busca de detalhes do stack: {firstStack.Name}");
                var stackDetails = await apiClient.GetStackByIdAsync(firstStack.Id);
                Console.WriteLine($"✓ Stack detalhado obtido: {stackDetails.Name} com {stackDetails.Technologies.Count} tecnologias");
            }

            Console.WriteLine("✅ Todos os testes passaram!");
            Console.WriteLine();
            Console.WriteLine("API está funcionando corretamente!");
            
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Erro ao testar API: {ex.Message}");
            Console.WriteLine($"Stack trace: {ex.StackTrace}");
            throw;
        }
        finally
        {
            httpClient.Dispose();
        }
    }

    /// <summary>
    /// Testa as ferramentas MCP diretamente
    /// </summary>
    public static async Task TestMcpTools()
    {
        var httpClient = new HttpClient();
        httpClient.BaseAddress = new Uri("http://localhost:5000/");
        httpClient.DefaultRequestHeaders.Add("User-Agent", "StackShare.McpServer/1.0.0");

        using var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
        var logger = loggerFactory.CreateLogger<StackShareApiClient>();
        var toolsLogger = loggerFactory.CreateLogger<Tools.StackShareTools>();

        var apiClient = new StackShareApiClient(httpClient, logger);

        try
        {
            Console.WriteLine("Testando ferramentas MCP...");

            // Initialize tools
            Tools.StackShareTools.Initialize(apiClient, toolsLogger);

            // Test search_stacks
            Console.WriteLine("1. Testando ferramenta search_stacks...");
            var searchResult = await Tools.StackShareTools.SearchStacks();
            var searchData = JsonSerializer.Deserialize<JsonDocument>(searchResult);
            Console.WriteLine($"✓ search_stacks executada: {searchResult.Substring(0, Math.Min(100, searchResult.Length))}...");

            // Test list_technologies
            Console.WriteLine("2. Testando ferramenta list_technologies...");
            var techResult = await Tools.StackShareTools.ListTechnologies();
            var techData = JsonSerializer.Deserialize<JsonDocument>(techResult);
            Console.WriteLine($"✓ list_technologies executada: {techResult.Substring(0, Math.Min(100, techResult.Length))}...");

            Console.WriteLine("✅ Ferramentas MCP funcionando!");
            
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Erro ao testar ferramentas MCP: {ex.Message}");
            Console.WriteLine($"Stack trace: {ex.StackTrace}");
            throw;
        }
        finally
        {
            httpClient.Dispose();
        }
    }

    public static async Task Main(string[] args)
    {
        Console.WriteLine("=== Teste de Integração do MCP Server StackShare ===");
        Console.WriteLine();

        try
        {
            await TestApiConnection();
            Console.WriteLine();
            await TestMcpTools();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Testes falharam: {ex.Message}");
            Environment.Exit(1);
        }

        Console.WriteLine();
        Console.WriteLine("🎉 Todos os testes de integração passaram!");
    }
}