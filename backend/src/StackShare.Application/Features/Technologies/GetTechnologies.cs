using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using StackShare.Application.Interfaces;
using StackShare.Application.Features.Stacks;

namespace StackShare.Application.Features.Technologies;

public class GetTechnologies : IRequest<PagedResult<TechnologyDto>>
{
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? Search { get; set; }
    public bool? OnlyPreRegistered { get; set; }

    public GetTechnologies(int page = 1, int pageSize = 20, string? search = null, bool? onlyPreRegistered = null)
    {
        Page = page;
        PageSize = pageSize;
        Search = search;
        OnlyPreRegistered = onlyPreRegistered;
    }
}

public class GetTechnologiesHandler : IRequestHandler<GetTechnologies, PagedResult<TechnologyDto>>
{
    private readonly IStackShareDbContext _context;
    private readonly ILogger<GetTechnologiesHandler> _logger;

    public GetTechnologiesHandler(
        IStackShareDbContext context,
        ILogger<GetTechnologiesHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<PagedResult<TechnologyDto>> Handle(GetTechnologies request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Listando tecnologias - Página: {Page}, Tamanho: {PageSize}, Search: {Search}, OnlyPreRegistered: {OnlyPreRegistered}", 
            request.Page, request.PageSize, request.Search, request.OnlyPreRegistered);

        var query = _context.Technologies.Where(t => t.IsActive);

        // Aplicar filtro de pré-cadastradas se especificado
        if (request.OnlyPreRegistered.HasValue)
        {
            query = query.Where(t => t.IsPreRegistered == request.OnlyPreRegistered.Value);
        }

        // Aplicar busca textual se especificada
        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var searchLower = request.Search.ToLower();
            query = query.Where(t => 
                t.Name.ToLower().Contains(searchLower) || 
                (t.Description != null && t.Description.ToLower().Contains(searchLower)));
        }

        // Ordenar por nome
        query = query.OrderBy(t => t.Name);

        var totalCount = await query.CountAsync(cancellationToken);

        var technologies = await query
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(t => new TechnologyDto
            {
                Id = t.Id,
                Name = t.Name,
                Description = t.Description,
                IsPreRegistered = t.IsPreRegistered,
                CreatedAt = t.CreatedAt
            })
            .ToListAsync(cancellationToken);

        _logger.LogInformation("Retornando {Count} tecnologias de um total de {TotalCount}", 
            technologies.Count, totalCount);

        return new PagedResult<TechnologyDto>(
            technologies,
            totalCount,
            request.Page,
            request.PageSize,
            (int)Math.Ceiling(totalCount / (double)request.PageSize)
        );
    }
}