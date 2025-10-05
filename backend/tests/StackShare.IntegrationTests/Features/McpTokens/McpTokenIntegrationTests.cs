using FluentAssertions;
using System.Net;
using System.Text.Json;
using StackShare.IntegrationTests.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using StackShare.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace StackShare.IntegrationTests.Features.McpTokens;

public class McpTokenIntegrationTests : BaseIntegrationTest
{
    public McpTokenIntegrationTests(StackShareWebApplicationFactory factory) : base(factory)
    {
    }

    [Fact]
    public async Task GenerateMcpToken_WithValidData_ShouldGenerateTokenSuccessfully()
    {
        // Arrange
        var token = await GetAuthTokenAsync();
        SetAuthToken(token);

        var tokenRequest = new
        {
            Name = "My Development Token"
        };

        // Act
        var response = await Client.PostAsync("/api/users/me/mcp-tokens", CreateJsonContent(tokenRequest));

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);

        var result = await DeserializeResponseAsync<JsonElement>(response);
        result.GetProperty("name").GetString().Should().Be("My Development Token");
        result.GetProperty("rawToken").GetString().Should().NotBeNullOrEmpty();
        result.GetProperty("createdAt").GetDateTime().Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromMinutes(1));

        var rawTokenValue = result.GetProperty("rawToken").GetString()!;
        rawTokenValue.Should().NotBeNullOrEmpty();
        rawTokenValue.Length.Should().BeGreaterThan(32); // Should be a substantial token

        // Verify in database that token hash is stored, not raw token
        using var scope = Factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<StackShareDbContext>();
        
        var tokenInDb = await context.McpApiTokens.FirstAsync();
        tokenInDb.Name.Should().Be("My Development Token");
        tokenInDb.TokenHash.Should().NotBeNullOrEmpty();
        tokenInDb.TokenHash.Should().NotBe(rawTokenValue); // Should be hashed, not raw
        tokenInDb.IsRevoked.Should().BeFalse();
        tokenInDb.RevokedAt.Should().BeNull();
    }

    [Fact]
    public async Task GetMcpTokens_AfterGeneration_ShouldListTokensWithoutRawToken()
    {
        // Arrange
        var authToken = await GetAuthTokenAsync();
        SetAuthToken(authToken);

        // Generate a token first
        var tokenRequest = new { Name = "Test Token" };
        var createResponse = await Client.PostAsync("/api/users/me/mcp-tokens", CreateJsonContent(tokenRequest));
        createResponse.EnsureSuccessStatusCode();
        
        var createResult = await DeserializeResponseAsync<JsonElement>(createResponse);
        var tokenId = Guid.Parse(createResult.GetProperty("id").GetString()!);

        // Act
        var response = await Client.GetAsync("/api/users/me/mcp-tokens");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var result = await DeserializeResponseAsync<JsonElement>(response);
        var tokens = result.EnumerateArray().ToList();
        
        tokens.Should().HaveCount(1);
        var token = tokens[0];
        
        token.GetProperty("id").GetString().Should().Be(tokenId.ToString());
        token.GetProperty("name").GetString().Should().Be("Test Token");
        token.GetProperty("isRevoked").GetBoolean().Should().BeFalse();
        token.GetProperty("revokedAt").ValueKind.Should().Be(JsonValueKind.Null);
        
        // Raw token should NOT be present in the response
        token.TryGetProperty("rawToken", out _).Should().BeFalse();
    }

    [Fact]
    public async Task RevokeMcpToken_WithValidToken_ShouldRevokeTokenSuccessfully()
    {
        // Arrange
        var authToken = await GetAuthTokenAsync();
        SetAuthToken(authToken);

        // Generate a token first
        var tokenRequest = new { Name = "Token to Revoke" };
        var createResponse = await Client.PostAsync("/api/users/me/mcp-tokens", CreateJsonContent(tokenRequest));
        createResponse.EnsureSuccessStatusCode();
        
        var createResult = await DeserializeResponseAsync<JsonElement>(createResponse);
        var tokenId = Guid.Parse(createResult.GetProperty("id").GetString()!);

        // Act
        var response = await Client.DeleteAsync($"/api/users/me/mcp-tokens/{tokenId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);

        // Verify token is revoked in database
        using var scope = Factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<StackShareDbContext>();
        
        var tokenInDb = await context.McpApiTokens.FirstAsync(t => t.Id == tokenId);
        tokenInDb.IsRevoked.Should().BeTrue();
        tokenInDb.RevokedAt.Should().NotBeNull();
        tokenInDb.RevokedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromMinutes(1));

        // Verify token appears as revoked in API response
        var listResponse = await Client.GetAsync("/api/users/me/mcp-tokens");
        listResponse.EnsureSuccessStatusCode();
        
        var listResult = await DeserializeResponseAsync<JsonElement>(listResponse);
        var tokens = listResult.EnumerateArray().ToList();
        
        tokens.Should().HaveCount(1);
        var token = tokens[0];
        token.GetProperty("isRevoked").GetBoolean().Should().BeTrue();
        token.GetProperty("revokedAt").ValueKind.Should().Be(JsonValueKind.String);
    }

    [Fact]
    public async Task RevokeMcpToken_WithNonExistentToken_ShouldReturnNotFound()
    {
        // Arrange
        var authToken = await GetAuthTokenAsync();
        SetAuthToken(authToken);

        var nonExistentTokenId = Guid.NewGuid();

        // Act
        var response = await Client.DeleteAsync($"/api/users/me/mcp-tokens/{nonExistentTokenId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GenerateAndRevokeMcpToken_CompleteFlow_ShouldWorkEndToEnd()
    {
        // Arrange
        var authToken = await GetAuthTokenAsync();
        SetAuthToken(authToken);

        // Act 1: Generate token
        var generateRequest = new { Name = "End-to-End Test Token" };
        var generateResponse = await Client.PostAsync("/api/users/me/mcp-tokens", CreateJsonContent(generateRequest));
        
        generateResponse.StatusCode.Should().Be(HttpStatusCode.Created);
        var generateResult = await DeserializeResponseAsync<JsonElement>(generateResponse);
        var tokenId = Guid.Parse(generateResult.GetProperty("id").GetString()!);
        var rawToken = generateResult.GetProperty("rawToken").GetString()!;

        // Act 2: List tokens (should show active token)
        var listResponse1 = await Client.GetAsync("/api/users/me/mcp-tokens");
        listResponse1.StatusCode.Should().Be(HttpStatusCode.OK);
        var listResult1 = await DeserializeResponseAsync<JsonElement>(listResponse1);
        var tokens1 = listResult1.EnumerateArray().ToList();
        
        // Act 3: Revoke token
        var revokeResponse = await Client.DeleteAsync($"/api/users/me/mcp-tokens/{tokenId}");
        revokeResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);

        // Act 4: List tokens (should show revoked token)
        var listResponse2 = await Client.GetAsync("/api/users/me/mcp-tokens");
        listResponse2.StatusCode.Should().Be(HttpStatusCode.OK);
        var listResult2 = await DeserializeResponseAsync<JsonElement>(listResponse2);
        var tokens2 = listResult2.EnumerateArray().ToList();

        // Assert complete flow
        rawToken.Should().NotBeNullOrEmpty();
        
        tokens1.Should().HaveCount(1);
        tokens1[0].GetProperty("isRevoked").GetBoolean().Should().BeFalse();
        
        tokens2.Should().HaveCount(1);
        tokens2[0].GetProperty("isRevoked").GetBoolean().Should().BeTrue();
        tokens2[0].GetProperty("id").GetString().Should().Be(tokenId.ToString());

        // Verify database state
        using var scope = Factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<StackShareDbContext>();
        
        var finalTokenInDb = await context.McpApiTokens.FirstAsync(t => t.Id == tokenId);
        finalTokenInDb.Name.Should().Be("End-to-End Test Token");
        finalTokenInDb.IsRevoked.Should().BeTrue();
        finalTokenInDb.TokenHash.Should().NotBe(rawToken); // Should still be hashed
    }

    [Fact]
    public async Task McpTokens_WithoutAuthentication_ShouldReturnUnauthorized()
    {
        // Arrange - Don't set auth token

        // Act & Assert - Generate token
        var generateResponse = await Client.PostAsync("/api/users/me/mcp-tokens", 
            CreateJsonContent(new { Name = "Test" }));
        generateResponse.StatusCode.Should().Be(HttpStatusCode.Unauthorized);

        // Act & Assert - List tokens
        var listResponse = await Client.GetAsync("/api/users/me/mcp-tokens");
        listResponse.StatusCode.Should().Be(HttpStatusCode.Unauthorized);

        // Act & Assert - Revoke token
        var revokeResponse = await Client.DeleteAsync($"/api/users/me/mcp-tokens/{Guid.NewGuid()}");
        revokeResponse.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
}