using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StackShare.Application.Features.Authentication;
using System.Security.Claims;

namespace StackShare.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<AuthController> _logger;

    public AuthController(IMediator mediator, ILogger<AuthController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    /// <summary>
    /// Registra um novo usuário
    /// </summary>
    [HttpPost("register")]
    public async Task<ActionResult<RegisterResponse>> Register([FromBody] RegisterRequest request)
    {
        _logger.LogInformation("Tentativa de registro para email: {Email}", request.Email);
        
        var result = await _mediator.Send(request);
        
        _logger.LogInformation("Usuário registrado com sucesso: {UserId}", result.UserId);
        
        return CreatedAtAction(nameof(Register), new { id = result.UserId }, result);
    }

    /// <summary>
    /// Autentica um usuário e retorna um token JWT
    /// </summary>
    [HttpPost("login")]
    public async Task<ActionResult<LoginResponse>> Login([FromBody] LoginRequest request)
    {
        _logger.LogInformation("Tentativa de login para email: {Email}", request.Email);
        
        var result = await _mediator.Send(request);
        
        _logger.LogInformation("Login realizado com sucesso para usuário: {UserId}", result.UserId);
        
        return Ok(result);
    }

    /// <summary>
    /// Retorna o perfil do usuário autenticado
    /// </summary>
    [HttpGet("profile")]
    [Authorize]
    public async Task<ActionResult<UserProfileResponse>> GetProfile()
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId) || !Guid.TryParse(userId, out var userGuid))
        {
            _logger.LogWarning("Token inválido - UserId não encontrado ou inválido");
            throw new UnauthorizedAccessException("Token inválido");
        }

        _logger.LogInformation("Obtendo perfil para usuário: {UserId}", userGuid);
        
        var request = new GetUserProfileRequest(userGuid);
        var result = await _mediator.Send(request);
        
        return Ok(result);
    }
}