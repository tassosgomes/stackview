using StackShare.McpServer.Models;
using System.Text.Json;

namespace StackShare.McpServer.Services;

public interface IStackShareApiClient
{
    Task<PagedResult<StackSummaryResponse>> GetStacksAsync(
        int page = 1, 
        int pageSize = 20, 
        string? type = null, 
        Guid? technologyId = null, 
        string? search = null, 
        bool? onlyPublic = true);
    
    Task<StackResponse> GetStackByIdAsync(Guid id);
    Task<PagedResult<TechnologyDto>> GetTechnologiesAsync(int page = 1, int pageSize = 20, string? search = null);
}

public class StackShareApiClient : IStackShareApiClient
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<StackShareApiClient> _logger;
    private readonly JsonSerializerOptions _jsonOptions;

    public StackShareApiClient(HttpClient httpClient, ILogger<StackShareApiClient> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            PropertyNameCaseInsensitive = true
        };
    }

    public async Task<PagedResult<StackSummaryResponse>> GetStacksAsync(
        int page = 1, 
        int pageSize = 20, 
        string? type = null, 
        Guid? technologyId = null, 
        string? search = null, 
        bool? onlyPublic = true)
    {
        try
        {
            var queryString = new List<string>();
            queryString.Add($"page={page}");
            queryString.Add($"pageSize={pageSize}");
            
            if (!string.IsNullOrEmpty(type))
                queryString.Add($"type={type}");
                
            if (technologyId.HasValue)
                queryString.Add($"technologyId={technologyId}");
                
            if (!string.IsNullOrEmpty(search))
                queryString.Add($"search={Uri.EscapeDataString(search)}");
                
            if (onlyPublic.HasValue)
                queryString.Add($"onlyPublic={onlyPublic.Value.ToString().ToLower()}");

            var url = $"api/stacks?{string.Join("&", queryString)}";
            
            _logger.LogInformation("Fazendo requisição GET para: {Url}", url);
            
            var response = await _httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<PagedResult<StackSummaryResponse>>(content, _jsonOptions);

            _logger.LogInformation("Requisição bem-sucedida. Retornando {Count} stacks", result?.Items.Count ?? 0);

            return result ?? new PagedResult<StackSummaryResponse>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao buscar stacks");
            throw;
        }
    }

    public async Task<StackResponse> GetStackByIdAsync(Guid id)
    {
        try
        {
            var url = $"api/stacks/{id}";
            
            _logger.LogInformation("Fazendo requisição GET para: {Url}", url);
            
            var response = await _httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<StackResponse>(content, _jsonOptions);

            _logger.LogInformation("Stack obtido com sucesso: {StackName}", result?.Name ?? "N/A");

            return result ?? throw new InvalidOperationException("Failed to deserialize stack response");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao buscar stack por ID: {StackId}", id);
            throw;
        }
    }

    public async Task<PagedResult<TechnologyDto>> GetTechnologiesAsync(int page = 1, int pageSize = 20, string? search = null)
    {
        try
        {
            var queryString = new List<string>();
            queryString.Add($"page={page}");
            queryString.Add($"pageSize={pageSize}");
            
            if (!string.IsNullOrEmpty(search))
                queryString.Add($"search={Uri.EscapeDataString(search)}");

            var url = $"api/technologies?{string.Join("&", queryString)}";
            
            _logger.LogInformation("Fazendo requisição GET para: {Url}", url);
            
            var response = await _httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<PagedResult<TechnologyDto>>(content, _jsonOptions);

            _logger.LogInformation("Requisição bem-sucedida. Retornando {Count} tecnologias", result?.Items.Count ?? 0);

            return result ?? new PagedResult<TechnologyDto>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao buscar tecnologias");
            throw;
        }
    }
}