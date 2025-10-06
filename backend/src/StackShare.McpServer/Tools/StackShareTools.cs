using MCPSharp;
using StackShare.McpServer.Services;
using StackShare.McpServer.Models;
using System.Text.Json;

namespace StackShare.McpServer.Tools;

/// <summary>
/// Ferramentas MCP para buscar e consultar stacks do StackShare
/// </summary>
public class StackShareTools
{
    private static IStackShareApiClient? _apiClient;
    private static ILogger<StackShareTools>? _logger;

    public static void Initialize(IStackShareApiClient apiClient, ILogger<StackShareTools> logger)
    {
        _apiClient = apiClient;
        _logger = logger;
    }

    /// <summary>
    /// Busca stacks no StackShare com filtros opcionais
    /// </summary>
    /// <param name="search">Termo de busca para filtrar stacks por nome ou descrição</param>
    /// <param name="type">Tipo do stack (Frontend, Backend, Mobile, DevOps, Data, Testing)</param>
    /// <param name="technologyName">Nome da tecnologia para filtrar</param>
    /// <param name="page">Página da paginação (default: 1)</param>
    /// <param name="pageSize">Tamanho da página (default: 10)</param>
    /// <returns>Lista paginada de stacks encontrados</returns>
    [McpTool("search_stacks", "Busca stacks no StackShare com filtros opcionais como tipo, tecnologia ou termo de busca")]
    public static async Task<string> SearchStacks(
        [McpParameter(false, "Termo de busca para filtrar stacks por nome ou descrição")] string? search = null,
        [McpParameter(false, "Tipo do stack: Frontend, Backend, Mobile, DevOps, Data, Testing")] string? type = null,
        [McpParameter(false, "Nome da tecnologia para filtrar stacks")] string? technologyName = null,
        [McpParameter(false, "Página da paginação")] int page = 1,
        [McpParameter(false, "Tamanho da página")] int pageSize = 10)
    {
        try
        {
            if (_apiClient == null || _logger == null)
                throw new InvalidOperationException("StackShareTools não foi inicializado");

            _logger.LogInformation("Buscando stacks - Search: {Search}, Type: {Type}, Technology: {Technology}, Page: {Page}", 
                search, type, technologyName, page);

            Guid? technologyId = null;
            if (!string.IsNullOrEmpty(technologyName))
            {
                // Buscar tecnologia por nome para obter o ID
                var technologies = await _apiClient.GetTechnologiesAsync(search: technologyName, pageSize: 1);
                var technology = technologies.Items.FirstOrDefault(t => 
                    t.Name.Equals(technologyName, StringComparison.OrdinalIgnoreCase));
                
                if (technology != null)
                {
                    technologyId = technology.Id;
                }
                else
                {
                    return JsonSerializer.Serialize(new
                    {
                        success = false,
                        message = $"Tecnologia '{technologyName}' não encontrada",
                        stacks = new object[0],
                        totalCount = 0
                    });
                }
            }

            var result = await _apiClient.GetStacksAsync(page, pageSize, type, technologyId, search, onlyPublic: true);

            var response = new
            {
                success = true,
                stacks = result.Items.Select(s => new
                {
                    id = s.Id,
                    name = s.Name,
                    description = s.Description,
                    type = s.Type,
                    createdBy = s.CreatedBy,
                    createdAt = s.CreatedAt,
                    isPublic = s.IsPublic,
                    technologies = s.Technologies
                }).ToArray(),
                pagination = new
                {
                    totalCount = result.TotalCount,
                    page = result.Page,
                    pageSize = result.PageSize,
                    totalPages = result.TotalPages,
                    hasNextPage = result.HasNextPage,
                    hasPreviousPage = result.HasPreviousPage
                }
            };

            return JsonSerializer.Serialize(response, new JsonSerializerOptions { WriteIndented = true });
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Erro ao buscar stacks");
            return JsonSerializer.Serialize(new
            {
                success = false,
                error = "Erro interno do servidor ao buscar stacks",
                message = ex.Message
            });
        }
    }

    /// <summary>
    /// Obtém detalhes completos de um stack específico pelo ID
    /// </summary>
    /// <param name="stackId">ID único do stack</param>
    /// <returns>Detalhes completos do stack incluindo tecnologias</returns>
    [McpTool("get_stack_details", "Obtém detalhes completos de um stack específico pelo ID")]
    public static async Task<string> GetStackDetails(
        [McpParameter(true, "ID único do stack")] string stackId)
    {
        try
        {
            if (_apiClient == null || _logger == null)
                throw new InvalidOperationException("StackShareTools não foi inicializado");

            if (!Guid.TryParse(stackId, out var id))
            {
                return JsonSerializer.Serialize(new
                {
                    success = false,
                    error = "ID do stack inválido",
                    message = "O ID fornecido não é um GUID válido"
                });
            }

            _logger.LogInformation("Buscando detalhes do stack: {StackId}", id);

            var stack = await _apiClient.GetStackByIdAsync(id);

            var response = new
            {
                success = true,
                stack = new
                {
                    id = stack.Id,
                    name = stack.Name,
                    description = stack.Description,
                    type = stack.Type,
                    createdBy = stack.CreatedBy,
                    createdAt = stack.CreatedAt,
                    updatedAt = stack.UpdatedAt,
                    isPublic = stack.IsPublic,
                    technologies = stack.Technologies.Select(t => new
                    {
                        id = t.Id,
                        name = t.Name,
                        description = t.Description,
                        isPreRegistered = t.IsPreRegistered
                    }).ToArray()
                }
            };

            return JsonSerializer.Serialize(response, new JsonSerializerOptions { WriteIndented = true });
        }
        catch (HttpRequestException ex) when (ex.Message.Contains("404"))
        {
            return JsonSerializer.Serialize(new
            {
                success = false,
                error = "Stack não encontrado",
                message = $"Stack com ID {stackId} não foi encontrado"
            });
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Erro ao obter detalhes do stack: {StackId}", stackId);
            return JsonSerializer.Serialize(new
            {
                success = false,
                error = "Erro interno do servidor ao obter detalhes do stack",
                message = ex.Message
            });
        }
    }

    /// <summary>
    /// Lista todas as tecnologias disponíveis na plataforma
    /// </summary>
    /// <param name="search">Termo de busca para filtrar tecnologias por nome</param>
    /// <param name="page">Página da paginação (default: 1)</param>
    /// <param name="pageSize">Tamanho da página (default: 20)</param>
    /// <returns>Lista paginada de tecnologias disponíveis</returns>
    [McpTool("list_technologies", "Lista todas as tecnologias disponíveis na plataforma")]
    public static async Task<string> ListTechnologies(
        [McpParameter(false, "Termo de busca para filtrar tecnologias por nome")] string? search = null,
        [McpParameter(false, "Página da paginação")] int page = 1,
        [McpParameter(false, "Tamanho da página")] int pageSize = 20)
    {
        try
        {
            if (_apiClient == null || _logger == null)
                throw new InvalidOperationException("StackShareTools não foi inicializado");

            _logger.LogInformation("Listando tecnologias - Search: {Search}, Page: {Page}", search, page);

            var result = await _apiClient.GetTechnologiesAsync(page, pageSize, search);

            var response = new
            {
                success = true,
                technologies = result.Items.Select(t => new
                {
                    id = t.Id,
                    name = t.Name,
                    description = t.Description,
                    isPreRegistered = t.IsPreRegistered
                }).ToArray(),
                pagination = new
                {
                    totalCount = result.TotalCount,
                    page = result.Page,
                    pageSize = result.PageSize,
                    totalPages = result.TotalPages,
                    hasNextPage = result.HasNextPage,
                    hasPreviousPage = result.HasPreviousPage
                }
            };

            return JsonSerializer.Serialize(response, new JsonSerializerOptions { WriteIndented = true });
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Erro ao listar tecnologias");
            return JsonSerializer.Serialize(new
            {
                success = false,
                error = "Erro interno do servidor ao listar tecnologias",
                message = ex.Message
            });
        }
    }
}