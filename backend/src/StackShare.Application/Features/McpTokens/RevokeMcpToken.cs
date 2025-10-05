using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using StackShare.Application.Interfaces;
using StackShare.Domain.Exceptions;

namespace StackShare.Application.Features.McpTokens;

public record RevokeMcpTokenRequest(Guid TokenId) : IRequest;

public class RevokeMcpTokenHandler : IRequestHandler<RevokeMcpTokenRequest>
{
    private readonly IStackShareDbContext _context;
    private readonly ICurrentUserService _currentUserService;
    private readonly ILogger<RevokeMcpTokenHandler> _logger;

    public RevokeMcpTokenHandler(
        IStackShareDbContext context,
        ICurrentUserService currentUserService,
        ILogger<RevokeMcpTokenHandler> logger)
    {
        _context = context;
        _currentUserService = currentUserService;
        _logger = logger;
    }

    public async Task Handle(RevokeMcpTokenRequest request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Revoking MCP token {TokenId} for user {UserId}", 
            request.TokenId, _currentUserService.UserId);

        var token = await _context.McpApiTokens
            .FirstOrDefaultAsync(t => t.Id == request.TokenId && t.UserId == _currentUserService.UserId, 
                cancellationToken);

        if (token == null)
        {
            _logger.LogWarning("MCP token {TokenId} not found for user {UserId}", 
                request.TokenId, _currentUserService.UserId);
            throw new NotFoundException($"Token with ID {request.TokenId} not found");
        }

        if (token.IsRevoked)
        {
            _logger.LogWarning("MCP token {TokenId} is already revoked for user {UserId}", 
                request.TokenId, _currentUserService.UserId);
            return; // Already revoked, no action needed
        }

        token.IsRevoked = true;
        token.RevokedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Successfully revoked MCP token {TokenId} for user {UserId}", 
            request.TokenId, _currentUserService.UserId);
    }
}