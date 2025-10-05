using FluentAssertions;
using System.Net;
using System.Text.Json;
using StackShare.IntegrationTests.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using StackShare.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using StackShare.Domain.Enums;

namespace StackShare.IntegrationTests.Features.Stacks;

public class CreateStackIntegrationTests : BaseIntegrationTest
{
    public CreateStackIntegrationTests(StackShareWebApplicationFactory factory) : base(factory)
    {
    }

    [Fact]
    public async Task CreateStack_WithValidData_ShouldCreateStackSuccessfully()
    {
        // Arrange
        var token = await GetAuthTokenAsync();
        SetAuthToken(token);

        // First create a technology to use
        var technologyRequest = new
        {
            Name = "React",
            Description = "JavaScript library for building user interfaces"
        };

        var techResponse = await Client.PostAsync("/api/technologies", CreateJsonContent(technologyRequest));
        techResponse.EnsureSuccessStatusCode();

        var techResult = await DeserializeResponseAsync<JsonElement>(techResponse);
        var technologyId = Guid.Parse(techResult.GetProperty("id").GetString()!);

        var stackRequest = new
        {
            Name = "My Frontend Stack",
            Description = "A modern React-based frontend stack",
            Type = 1, // Frontend
            IsPublic = true,
            TechnologyIds = new[] { technologyId }
        };

        // Act
        var response = await Client.PostAsync("/api/stacks", CreateJsonContent(stackRequest));

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);

        var result = await DeserializeResponseAsync<JsonElement>(response);
        result.GetProperty("name").GetString().Should().Be("My Frontend Stack");
        result.GetProperty("description").GetString().Should().Be("A modern React-based frontend stack");
        result.GetProperty("type").GetInt32().Should().Be(1); // Frontend
        result.GetProperty("isPublic").GetBoolean().Should().BeTrue();

        var technologies = result.GetProperty("technologies").EnumerateArray().ToList();
        technologies.Should().HaveCount(1);
        technologies[0].GetProperty("name").GetString().Should().Be("React");

        // Verify in database
        using var scope = Factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<StackShareDbContext>();
        
        var stackInDb = await context.Stacks
            .Include(s => s.StackTechnologies)
            .ThenInclude(st => st.Technology)
            .FirstAsync();

        stackInDb.Name.Should().Be("My Frontend Stack");
        stackInDb.Type.Should().Be(StackType.Frontend);
        stackInDb.IsPublic.Should().BeTrue();
        stackInDb.StackTechnologies.Should().HaveCount(1);
        stackInDb.StackTechnologies.First().Technology.Name.Should().Be("React");
    }

    [Fact]
    public async Task CreateStack_WithNewTechnologyNames_ShouldCreateStackAndTechnologies()
    {
        // Arrange
        var token = await GetAuthTokenAsync();
        SetAuthToken(token);

        var stackRequest = new
        {
            Name = "Backend API Stack",
            Description = "A .NET backend API stack",
            Type = 2, // Backend
            IsPublic = true,
            TechnologyNames = new[] { "ASP.NET Core", "Entity Framework" }
        };

        // Act
        var response = await Client.PostAsync("/api/stacks", CreateJsonContent(stackRequest));

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);

        var result = await DeserializeResponseAsync<JsonElement>(response);
        result.GetProperty("name").GetString().Should().Be("Backend API Stack");

        var technologies = result.GetProperty("technologies").EnumerateArray().ToList();
        technologies.Should().HaveCount(2);
        
        var techNames = technologies.Select(t => t.GetProperty("name").GetString()).ToList();
        techNames.Should().Contain("ASP.NET Core");
        techNames.Should().Contain("Entity Framework");

        // Verify technologies were created in database
        using var scope = Factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<StackShareDbContext>();
        
        var technologiesInDb = await context.Technologies.ToListAsync();
        technologiesInDb.Should().HaveCount(2);
        technologiesInDb.Should().Contain(t => t.Name == "ASP.NET Core" && !t.IsPreRegistered);
        technologiesInDb.Should().Contain(t => t.Name == "Entity Framework" && !t.IsPreRegistered);
    }

    [Fact]
    public async Task CreateStack_WithInvalidData_ShouldReturnBadRequest()
    {
        // Arrange
        var token = await GetAuthTokenAsync();
        SetAuthToken(token);

        var stackRequest = new
        {
            Name = "", // Invalid - empty name
            Description = "A test stack",
            Type = 1, // Frontend
            IsPublic = true
        };

        // Act
        var response = await Client.PostAsync("/api/stacks", CreateJsonContent(stackRequest));

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task CreateStack_WithoutAuthentication_ShouldReturnUnauthorized()
    {
        // Arrange - Don't set auth token
        var stackRequest = new
        {
            Name = "Test Stack",
            Description = "A test stack",
            Type = 1, // Frontend
            IsPublic = true
        };

        // Act
        var response = await Client.PostAsync("/api/stacks", CreateJsonContent(stackRequest));

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task CreateStack_WithNonExistentTechnologyId_ShouldReturnBadRequest()
    {
        // Arrange
        var token = await GetAuthTokenAsync();
        SetAuthToken(token);

        var nonExistentTechId = Guid.NewGuid();
        var stackRequest = new
        {
            Name = "Test Stack",
            Description = "A test stack",
            Type = 1, // Frontend
            IsPublic = true,
            TechnologyIds = new[] { nonExistentTechId }
        };

        // Act
        var response = await Client.PostAsync("/api/stacks", CreateJsonContent(stackRequest));

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var content = await response.Content.ReadAsStringAsync();
        content.Should().Contain("tecnologias por ID");
    }
}