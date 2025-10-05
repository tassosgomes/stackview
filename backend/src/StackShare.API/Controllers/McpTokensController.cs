using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StackShare.Application.Features.McpTokens;

namespace StackShare.API.Controllers;

[ApiController]
[Route("api/users/me/mcp-tokens")]
[Authorize]
public class McpTokensController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<McpTokensController> _logger;

    public McpTokensController(
        IMediator mediator, 
        ILogger<McpTokensController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    /// <summary>
    /// Lista todos os tokens MCP do usuário atual
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<List<McpTokenSummaryResponse>>> GetMcpTokens()
    {
        _logger.LogInformation("Listing MCP tokens for current user");

        var request = new GetMcpTokensRequest();
        var result = await _mediator.Send(request);

        return Ok(result);
    }

    /// <summary>
    /// Gera um novo token MCP para o usuário atual
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<GenerateMcpTokenResponse>> GenerateMcpToken([FromBody] GenerateMcpTokenRequest request)
    {
        _logger.LogInformation("Generating new MCP token for current user with name: {TokenName}", request.Name);

        var result = await _mediator.Send(request);

        return CreatedAtAction(nameof(GetMcpTokens), new { }, result);
    }

    /// <summary>
    /// Revoga um token MCP específico do usuário atual
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<ActionResult> RevokeMcpToken(Guid id)
    {
        _logger.LogInformation("Revoking MCP token: {TokenId}", id);

        var request = new RevokeMcpTokenRequest(id);
        await _mediator.Send(request);

        return NoContent();
    }
}