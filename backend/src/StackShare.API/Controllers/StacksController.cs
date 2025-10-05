using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StackShare.Application.Features.Stacks;

namespace StackShare.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class StacksController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<StacksController> _logger;

    public StacksController(IMediator mediator, ILogger<StacksController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    /// <summary>
    /// Lista stacks com filtros e paginação
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<PagedResult<StackSummaryResponse>>> GetStacks(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] Domain.Enums.StackType? type = null,
        [FromQuery] Guid? technologyId = null,
        [FromQuery] string? search = null,
        [FromQuery] bool? onlyPublic = null)
    {
        _logger.LogInformation("Listando stacks - Página: {Page}, Tamanho: {PageSize}, Tipo: {Type}, TechId: {TechnologyId}", 
            page, pageSize, type, technologyId);

        var request = new GetStacksRequest(page, pageSize, type, technologyId, search, onlyPublic);
        var result = await _mediator.Send(request);

        return Ok(result);
    }

    /// <summary>
    /// Obtém um stack específico por ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<StackResponse>> GetStack(Guid id)
    {
        _logger.LogInformation("Obtendo stack: {StackId}", id);

        var request = new GetStackByIdRequest(id);
        var result = await _mediator.Send(request);

        return Ok(result);
    }

    /// <summary>
    /// Cria um novo stack
    /// </summary>
    [HttpPost]
    [Authorize]
    public async Task<ActionResult<StackResponse>> CreateStack([FromBody] CreateStackRequest request)
    {
        _logger.LogInformation("Criando stack: {Name}", request.Name);

        var result = await _mediator.Send(request);

        _logger.LogInformation("Stack criado com sucesso: {StackId}", result.Id);

        return CreatedAtAction(nameof(GetStack), new { id = result.Id }, result);
    }

    /// <summary>
    /// Atualiza um stack existente
    /// </summary>
    [HttpPut("{id}")]
    [Authorize]
    public async Task<ActionResult<StackResponse>> UpdateStack(Guid id, [FromBody] UpdateStackRequest request)
    {
        if (id != request.Id)
        {
            return BadRequest("ID da URL não confere com ID do corpo da requisição");
        }

        _logger.LogInformation("Atualizando stack: {StackId}", id);

        var result = await _mediator.Send(request);

        _logger.LogInformation("Stack atualizado com sucesso: {StackId}", result.Id);

        return Ok(result);
    }

    /// <summary>
    /// Remove um stack (soft delete)
    /// </summary>
    [HttpDelete("{id}")]
    [Authorize]
    public async Task<IActionResult> DeleteStack(Guid id)
    {
        _logger.LogInformation("Removendo stack: {StackId}", id);

        var request = new DeleteStackRequest(id);
        await _mediator.Send(request);

        _logger.LogInformation("Stack removido com sucesso: {StackId}", id);

        return NoContent();
    }

    /// <summary>
    /// Obtém o histórico de versões de um stack
    /// </summary>
    [HttpGet("{id}/history")]
    public async Task<ActionResult<List<StackHistoryResponse>>> GetStackHistory(Guid id)
    {
        _logger.LogInformation("Obtendo histórico do stack: {StackId}", id);

        var request = new GetStackHistoryRequest(id);
        var result = await _mediator.Send(request);

        return Ok(result);
    }
}