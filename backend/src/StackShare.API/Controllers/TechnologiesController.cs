using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StackShare.Application.Features.Technologies;
using StackShare.Application.Features.Stacks;

namespace StackShare.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TechnologiesController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<TechnologiesController> _logger;

    public TechnologiesController(IMediator mediator, ILogger<TechnologiesController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    /// <summary>
    /// Lista tecnologias com filtros e paginação
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<PagedResult<Application.Features.Technologies.TechnologyDto>>> GetTechnologies(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string? search = null,
        [FromQuery] bool? onlyPreRegistered = null)
    {
        _logger.LogInformation("Listando tecnologias - Página: {Page}, Tamanho: {PageSize}, Search: {Search}", 
            page, pageSize, search);

        var request = new GetTechnologies(page, pageSize, search, onlyPreRegistered);
        var result = await _mediator.Send(request);

        return Ok(result);
    }

    /// <summary>
    /// Sugere tecnologias baseado em fuzzy matching
    /// </summary>
    [HttpPost("suggest")]
    public async Task<ActionResult<SuggestTechnologiesResponse>> SuggestTechnologies(
        [FromBody] SuggestTechnologiesRequest request)
    {
        _logger.LogInformation("Buscando sugestões para: {Name}", request.Name);

        var query = new SuggestTechnologies(request.Name, request.MaxResults);
        var result = await _mediator.Send(query);

        return Ok(result);
    }

    /// <summary>
    /// Cria uma nova tecnologia (apenas administradores)
    /// </summary>
    [HttpPost]
    [Authorize] // TODO: Implementar autorização específica para admin
    public async Task<ActionResult<CreateTechnologyResponse>> CreateTechnology(
        [FromBody] CreateTechnologyRequest request)
    {
        _logger.LogInformation("Criando tecnologia: {Name}", request.Name);

        var command = new CreateTechnology(request.Name, request.Description);
        var result = await _mediator.Send(command);

        _logger.LogInformation("Tecnologia criada com sucesso: {TechnologyId}", result.Id);

        return CreatedAtAction(nameof(GetTechnologies), new { }, result);
    }
}