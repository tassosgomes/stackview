using FuzzySharp;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using StackShare.Application.Interfaces;

namespace StackShare.Application.Features.Technologies;

public class SuggestTechnologies : IRequest<SuggestTechnologiesResponse>
{
    public string Name { get; set; } = string.Empty;
    public int MaxResults { get; set; } = 10;

    public SuggestTechnologies(string name, int maxResults = 10)
    {
        Name = name;
        MaxResults = maxResults;
    }
}

public class SuggestTechnologiesHandler : IRequestHandler<SuggestTechnologies, SuggestTechnologiesResponse>
{
    private readonly IStackShareDbContext _context;
    private readonly ILogger<SuggestTechnologiesHandler> _logger;
    private const int MinimumFuzzyScore = 60; // Threshold para considerar uma sugestão relevante

    public SuggestTechnologiesHandler(
        IStackShareDbContext context,
        ILogger<SuggestTechnologiesHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<SuggestTechnologiesResponse> Handle(SuggestTechnologies request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Buscando sugestões para tecnologia: {Name}, MaxResults: {MaxResults}", 
            request.Name, request.MaxResults);

        // Buscar todas as tecnologias ativas
        var allTechnologies = await _context.Technologies
            .Where(t => t.IsActive)
            .Select(t => new { t.Id, t.Name, t.Description })
            .ToListAsync(cancellationToken);

        _logger.LogInformation("Total de tecnologias ativas encontradas: {Count}", allTechnologies.Count);

        var suggestions = new List<TechnologySuggestionDto>();

        // Aplicar FuzzySharp matching
        foreach (var tech in allTechnologies)
        {
            var nameScore = Fuzz.Ratio(request.Name.ToLower(), tech.Name.ToLower());
            var partialScore = Fuzz.PartialRatio(request.Name.ToLower(), tech.Name.ToLower());
            var tokenSetScore = Fuzz.TokenSetRatio(request.Name.ToLower(), tech.Name.ToLower());
            
            // Usar o maior score entre os diferentes algoritmos
            var maxScore = Math.Max(Math.Max(nameScore, partialScore), tokenSetScore);

            if (maxScore >= MinimumFuzzyScore)
            {
                suggestions.Add(new TechnologySuggestionDto
                {
                    Id = tech.Id,
                    Name = tech.Name,
                    Description = tech.Description,
                    Score = maxScore
                });
            }
        }

        // Ordenar por score descendente e limitar resultados
        var orderedSuggestions = suggestions
            .OrderByDescending(s => s.Score)
            .ThenBy(s => s.Name)
            .Take(request.MaxResults)
            .ToList();

        _logger.LogInformation("Retornando {Count} sugestões para '{Name}'", 
            orderedSuggestions.Count, request.Name);

        return new SuggestTechnologiesResponse
        {
            Suggestions = orderedSuggestions
        };
    }
}