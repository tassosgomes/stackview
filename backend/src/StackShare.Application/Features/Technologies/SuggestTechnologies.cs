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

        // Buscar tecnologias ativas com limite para performance
        // Primeiro tenta busca exata/parcial no banco, depois aplica fuzzy matching
        var dbTechnologies = await _context.Technologies
            .Where(t => t.IsActive && (
                t.Name.ToLower().Contains(request.Name.ToLower()) ||
                request.Name.ToLower().Contains(t.Name.ToLower())
            ))
            .Select(t => new { t.Id, t.Name, t.Description })
            .Take(100) // Limita para performance
            .ToListAsync(cancellationToken);

        // Se não encontrou suficientes, busca mais tecnologias para fuzzy matching
        List<dynamic> allTechnologies = dbTechnologies.Cast<dynamic>().ToList();
        if (dbTechnologies.Count < request.MaxResults * 2)
        {
            var additionalTechs = await _context.Technologies
                .Where(t => t.IsActive && !dbTechnologies.Select(dt => dt.Id).Contains(t.Id))
                .Select(t => new { t.Id, t.Name, t.Description })
                .Take(50) // Mais algumas para fuzzy matching
                .ToListAsync(cancellationToken);
            
            allTechnologies.AddRange(additionalTechs.Cast<dynamic>());
        }

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