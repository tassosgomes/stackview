using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using StackShare.Application.Interfaces;

namespace StackShare.Application.Features.McpTokens;

public record GetMcpTokensRequest() : IRequest<List<McpTokenSummaryResponse>>;

public record McpTokenSummaryResponse(
    Guid Id,
    string Name,
    DateTime CreatedAt,
    DateTime? ExpiresAt,
    DateTime? LastUsedAt,
    bool IsRevoked,
    DateTime? RevokedAt);

public class GetMcpTokensHandler : IRequestHandler<GetMcpTokensRequest, List<McpTokenSummaryResponse>>
{
    private readonly IStackShareDbContext _context;
    private readonly ICurrentUserService _currentUserService;
    private readonly ILogger<GetMcpTokensHandler> _logger;

    public GetMcpTokensHandler(
        IStackShareDbContext context,
        ICurrentUserService currentUserService,
        ILogger<GetMcpTokensHandler> logger)
    {
        _context = context;
        _currentUserService = currentUserService;
        _logger = logger;
    }

    public async Task<List<McpTokenSummaryResponse>> Handle(GetMcpTokensRequest request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Retrieving MCP tokens for user {UserId}", _currentUserService.UserId);

        var tokens = await _context.McpApiTokens
            .Where(t => t.UserId == _currentUserService.UserId)
            .OrderByDescending(t => t.CreatedAt)
            .Select(t => new McpTokenSummaryResponse(
                t.Id,
                t.Name,
                t.CreatedAt,
                t.ExpiresAt,
                t.LastUsedAt,
                t.IsRevoked,
                t.RevokedAt))
            .ToListAsync(cancellationToken);

        _logger.LogInformation("Found {TokenCount} MCP tokens for user {UserId}", 
            tokens.Count, _currentUserService.UserId);

        return tokens;
    }
}