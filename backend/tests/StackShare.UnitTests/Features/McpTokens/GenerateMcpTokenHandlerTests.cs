using FluentAssertions;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using StackShare.Application.Features.McpTokens;
using StackShare.Application.Interfaces;
using StackShare.Domain.Entities;
using StackShare.Infrastructure.Data;

namespace StackShare.UnitTests.Features.McpTokens;

public class GenerateMcpTokenHandlerTests : IDisposable
{
    private readonly Mock<ICurrentUserService> _currentUserServiceMock;
    private readonly Mock<ITokenService> _tokenServiceMock;
    private readonly Mock<ILogger<GenerateMcpTokenHandler>> _loggerMock;
    private readonly StackShareDbContext _context;
    private readonly GenerateMcpTokenHandler _handler;

    public GenerateMcpTokenHandlerTests()
    {
        var options = new DbContextOptionsBuilder<StackShareDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new StackShareDbContext(options);
        _currentUserServiceMock = new Mock<ICurrentUserService>();
        _tokenServiceMock = new Mock<ITokenService>();
        _loggerMock = new Mock<ILogger<GenerateMcpTokenHandler>>();

        _handler = new GenerateMcpTokenHandler(
            _context,
            _currentUserServiceMock.Object,
            _tokenServiceMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_ValidRequest_ShouldGenerateTokenSuccessfully()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var rawToken = "raw-token-12345";
        var hashedToken = "hashed-token-67890";

        _currentUserServiceMock.Setup(x => x.UserId).Returns(userId);
        _tokenServiceMock.Setup(x => x.GenerateSecureToken()).Returns(rawToken);
        _tokenServiceMock.Setup(x => x.HashToken(rawToken)).Returns(hashedToken);

        var request = new GenerateMcpTokenRequest("My Development Token");

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Name.Should().Be("My Development Token");
        result.RawToken.Should().Be(rawToken);
        result.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));

        var tokenInDb = await _context.McpApiTokens.FirstOrDefaultAsync();
        tokenInDb.Should().NotBeNull();
        tokenInDb!.Name.Should().Be("My Development Token");
        tokenInDb.TokenHash.Should().Be(hashedToken);
        tokenInDb.UserId.Should().Be(userId);
        tokenInDb.IsRevoked.Should().BeFalse();
        tokenInDb.RevokedAt.Should().BeNull();
    }

    [Fact]
    public async Task Handle_MultipleTokens_ShouldCreateSeparateTokens()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var rawToken1 = "raw-token-1";
        var rawToken2 = "raw-token-2";
        var hashedToken1 = "hashed-token-1";
        var hashedToken2 = "hashed-token-2";

        _currentUserServiceMock.Setup(x => x.UserId).Returns(userId);
        
        _tokenServiceMock.SetupSequence(x => x.GenerateSecureToken())
            .Returns(rawToken1)
            .Returns(rawToken2);
        
        _tokenServiceMock.Setup(x => x.HashToken(rawToken1)).Returns(hashedToken1);
        _tokenServiceMock.Setup(x => x.HashToken(rawToken2)).Returns(hashedToken2);

        var request1 = new GenerateMcpTokenRequest("Token 1");
        var request2 = new GenerateMcpTokenRequest("Token 2");

        // Act
        var result1 = await _handler.Handle(request1, CancellationToken.None);
        var result2 = await _handler.Handle(request2, CancellationToken.None);

        // Assert
        result1.RawToken.Should().Be(rawToken1);
        result1.Name.Should().Be("Token 1");
        
        result2.RawToken.Should().Be(rawToken2);
        result2.Name.Should().Be("Token 2");

        var tokensInDb = await _context.McpApiTokens.ToListAsync();
        tokensInDb.Should().HaveCount(2);
        tokensInDb.Should().Contain(t => t.Name == "Token 1" && t.TokenHash == hashedToken1);
        tokensInDb.Should().Contain(t => t.Name == "Token 2" && t.TokenHash == hashedToken2);
    }

    [Fact]
    public async Task Handle_ValidRequest_ShouldCallTokenServiceMethods()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var rawToken = "raw-token-12345";
        var hashedToken = "hashed-token-67890";

        _currentUserServiceMock.Setup(x => x.UserId).Returns(userId);
        _tokenServiceMock.Setup(x => x.GenerateSecureToken()).Returns(rawToken);
        _tokenServiceMock.Setup(x => x.HashToken(rawToken)).Returns(hashedToken);

        var request = new GenerateMcpTokenRequest("My Development Token");

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        _tokenServiceMock.Verify(x => x.GenerateSecureToken(), Times.Once);
        _tokenServiceMock.Verify(x => x.HashToken(rawToken), Times.Once);
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}