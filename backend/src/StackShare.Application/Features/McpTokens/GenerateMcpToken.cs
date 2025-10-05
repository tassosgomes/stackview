using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using StackShare.Application.Interfaces;
using StackShare.Domain.Entities;

namespace StackShare.Application.Features.McpTokens;

public record GenerateMcpTokenRequest(string Name) : IRequest<GenerateMcpTokenResponse>;

public record GenerateMcpTokenResponse(
    Guid Id,
    string Name,
    string RawToken,
    DateTime CreatedAt);

public class GenerateMcpTokenValidator : AbstractValidator<GenerateMcpTokenRequest>
{
    public GenerateMcpTokenValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Nome é obrigatório")
            .MaximumLength(100).WithMessage("Nome deve ter no máximo 100 caracteres");
    }
}

public class GenerateMcpTokenHandler : IRequestHandler<GenerateMcpTokenRequest, GenerateMcpTokenResponse>
{
    private readonly IStackShareDbContext _context;
    private readonly ICurrentUserService _currentUserService;
    private readonly ITokenService _tokenService;
    private readonly ILogger<GenerateMcpTokenHandler> _logger;

    public GenerateMcpTokenHandler(
        IStackShareDbContext context,
        ICurrentUserService currentUserService,
        ITokenService tokenService,
        ILogger<GenerateMcpTokenHandler> logger)
    {
        _context = context;
        _currentUserService = currentUserService;
        _tokenService = tokenService;
        _logger = logger;
    }

    public async Task<GenerateMcpTokenResponse> Handle(GenerateMcpTokenRequest request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Generating MCP token for user {UserId} with name {TokenName}", 
            _currentUserService.UserId, request.Name);

        // Generate secure token
        var rawToken = _tokenService.GenerateSecureToken();
        var tokenHash = _tokenService.HashToken(rawToken);

        // Create token entity
        var mcpToken = new McpApiToken
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            TokenHash = tokenHash,
            UserId = _currentUserService.UserId,
            CreatedAt = DateTime.UtcNow
        };

        _context.McpApiTokens.Add(mcpToken);
        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Successfully generated MCP token {TokenId} for user {UserId}", 
            mcpToken.Id, _currentUserService.UserId);

        return new GenerateMcpTokenResponse(
            mcpToken.Id,
            mcpToken.Name,
            rawToken,
            mcpToken.CreatedAt);
    }
}