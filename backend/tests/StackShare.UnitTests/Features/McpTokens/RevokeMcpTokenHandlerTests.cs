using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using StackShare.Application.Features.McpTokens;
using StackShare.Application.Interfaces;
using StackShare.Domain.Entities;
using StackShare.Domain.Exceptions;
using StackShare.Infrastructure.Data;

namespace StackShare.UnitTests.Features.McpTokens;

public class RevokeMcpTokenHandlerTests : IDisposable
{
    private readonly Mock<ICurrentUserService> _currentUserServiceMock;
    private readonly Mock<ILogger<RevokeMcpTokenHandler>> _loggerMock;
    private readonly StackShareDbContext _context;
    private readonly RevokeMcpTokenHandler _handler;

    public RevokeMcpTokenHandlerTests()
    {
        var options = new DbContextOptionsBuilder<StackShareDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new StackShareDbContext(options);
        _currentUserServiceMock = new Mock<ICurrentUserService>();
        _loggerMock = new Mock<ILogger<RevokeMcpTokenHandler>>();

        _handler = new RevokeMcpTokenHandler(
            _context,
            _currentUserServiceMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_ValidToken_ShouldRevokeTokenSuccessfully()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var tokenId = Guid.NewGuid();

        _currentUserServiceMock.Setup(x => x.UserId).Returns(userId);

        var token = new McpApiToken
        {
            Id = tokenId,
            Name = "Test Token",
            TokenHash = "hashed-token",
            UserId = userId,
            CreatedAt = DateTime.UtcNow,
            IsRevoked = false,
            RevokedAt = null
        };
        _context.McpApiTokens.Add(token);
        await _context.SaveChangesAsync();

        var request = new RevokeMcpTokenRequest(tokenId);

        // Act
        await _handler.Handle(request, CancellationToken.None);

        // Assert
        var revokedToken = await _context.McpApiTokens.FirstAsync();
        revokedToken.IsRevoked.Should().BeTrue();
        revokedToken.RevokedAt.Should().NotBeNull();
        revokedToken.RevokedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
    }

    [Fact]
    public async Task Handle_TokenNotFound_ShouldThrowNotFoundException()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var nonExistentTokenId = Guid.NewGuid();

        _currentUserServiceMock.Setup(x => x.UserId).Returns(userId);

        var request = new RevokeMcpTokenRequest(nonExistentTokenId);

        // Act & Assert
        var act = () => _handler.Handle(request, CancellationToken.None);
        await act.Should().ThrowAsync<NotFoundException>()
            .WithMessage($"Token with ID {nonExistentTokenId} not found");
    }

    [Fact]
    public async Task Handle_TokenBelongsToAnotherUser_ShouldThrowNotFoundException()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var otherUserId = Guid.NewGuid();
        var tokenId = Guid.NewGuid();

        _currentUserServiceMock.Setup(x => x.UserId).Returns(userId);

        // Create token belonging to another user
        var token = new McpApiToken
        {
            Id = tokenId,
            Name = "Other User Token",
            TokenHash = "hashed-token",
            UserId = otherUserId,
            CreatedAt = DateTime.UtcNow,
            IsRevoked = false,
            RevokedAt = null
        };
        _context.McpApiTokens.Add(token);
        await _context.SaveChangesAsync();

        var request = new RevokeMcpTokenRequest(tokenId);

        // Act & Assert
        var act = () => _handler.Handle(request, CancellationToken.None);
        await act.Should().ThrowAsync<NotFoundException>()
            .WithMessage($"Token with ID {tokenId} not found");
    }

    [Fact]
    public async Task Handle_AlreadyRevokedToken_ShouldDoNothing()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var tokenId = Guid.NewGuid();
        var originalRevokedAt = DateTime.UtcNow.AddHours(-1);

        _currentUserServiceMock.Setup(x => x.UserId).Returns(userId);

        var token = new McpApiToken
        {
            Id = tokenId,
            Name = "Already Revoked Token",
            TokenHash = "hashed-token",
            UserId = userId,
            CreatedAt = DateTime.UtcNow.AddHours(-2),
            IsRevoked = true,
            RevokedAt = originalRevokedAt
        };
        _context.McpApiTokens.Add(token);
        await _context.SaveChangesAsync();

        var request = new RevokeMcpTokenRequest(tokenId);

        // Act
        await _handler.Handle(request, CancellationToken.None);

        // Assert
        var tokenInDb = await _context.McpApiTokens.FirstAsync();
        tokenInDb.IsRevoked.Should().BeTrue();
        tokenInDb.RevokedAt.Should().Be(originalRevokedAt); // Should remain unchanged
    }

    [Fact]
    public async Task Handle_MultipleTokensForUser_ShouldRevokeOnlySpecifiedToken()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var tokenId1 = Guid.NewGuid();
        var tokenId2 = Guid.NewGuid();

        _currentUserServiceMock.Setup(x => x.UserId).Returns(userId);

        var token1 = new McpApiToken
        {
            Id = tokenId1,
            Name = "Token 1",
            TokenHash = "hashed-token-1",
            UserId = userId,
            CreatedAt = DateTime.UtcNow,
            IsRevoked = false,
            RevokedAt = null
        };

        var token2 = new McpApiToken
        {
            Id = tokenId2,
            Name = "Token 2",
            TokenHash = "hashed-token-2",
            UserId = userId,
            CreatedAt = DateTime.UtcNow,
            IsRevoked = false,
            RevokedAt = null
        };

        _context.McpApiTokens.AddRange(token1, token2);
        await _context.SaveChangesAsync();

        var request = new RevokeMcpTokenRequest(tokenId1);

        // Act
        await _handler.Handle(request, CancellationToken.None);

        // Assert
        var tokens = await _context.McpApiTokens.ToListAsync();
        
        var revokedToken = tokens.First(t => t.Id == tokenId1);
        revokedToken.IsRevoked.Should().BeTrue();
        revokedToken.RevokedAt.Should().NotBeNull();

        var activeToken = tokens.First(t => t.Id == tokenId2);
        activeToken.IsRevoked.Should().BeFalse();
        activeToken.RevokedAt.Should().BeNull();
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}