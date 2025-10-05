using FluentAssertions;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Moq;
using StackShare.Application.Features.Stacks;
using StackShare.Application.Interfaces;
using StackShare.Domain.Entities;
using StackShare.Domain.Enums;
using StackShare.Domain.Exceptions;
using StackShare.Infrastructure.Data;

namespace StackShare.UnitTests.Features.Stacks;

public class GetStackByIdHandlerTests : IDisposable
{
    private readonly Mock<ICurrentUserService> _currentUserServiceMock;
    private readonly Mock<IValidator<GetStackByIdRequest>> _validatorMock;
    private readonly StackShareDbContext _context;
    private readonly GetStackByIdHandler _handler;

    public GetStackByIdHandlerTests()
    {
        var options = new DbContextOptionsBuilder<StackShareDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new StackShareDbContext(options);
        _currentUserServiceMock = new Mock<ICurrentUserService>();
        _validatorMock = new Mock<IValidator<GetStackByIdRequest>>();

        _handler = new GetStackByIdHandler(
            _context,
            _validatorMock.Object,
            _currentUserServiceMock.Object);
    }

    [Fact]
    public async Task Handle_PublicStack_ShouldReturnStackForAnyUser()
    {
        // Arrange
        var stackOwner = Guid.NewGuid();
        var currentUser = Guid.NewGuid();
        var stackId = Guid.NewGuid();
        var technologyId = Guid.NewGuid();

        _currentUserServiceMock.Setup(x => x.UserId).Returns(currentUser);
        _validatorMock.Setup(x => x.ValidateAsync(It.IsAny<GetStackByIdRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new FluentValidation.Results.ValidationResult());

        // Add test data
        var technology = new Technology
        {
            Id = technologyId,
            Name = "React",
            Description = "JavaScript library",
            IsPreRegistered = true,
            IsActive = true
        };
        _context.Technologies.Add(technology);

        var stack = new Stack
        {
            Id = stackId,
            Name = "Public Stack",
            Description = "A public test stack",
            Type = StackType.Frontend,
            IsPublic = true,
            UserId = stackOwner,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            IsActive = true
        };
        _context.Stacks.Add(stack);

        var stackTechnology = new StackTechnology
        {
            StackId = stackId,
            TechnologyId = technologyId,
            Stack = stack,
            Technology = technology
        };
        _context.StackTechnologies.Add(stackTechnology);

        await _context.SaveChangesAsync();

        var request = new GetStackByIdRequest(stackId);

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(stackId);
        result.Name.Should().Be("Public Stack");
        result.IsPublic.Should().BeTrue();
        result.UserId.Should().Be(stackOwner);
        result.Technologies.Should().HaveCount(1);
        result.Technologies.First().Name.Should().Be("React");
    }

    [Fact]
    public async Task Handle_PrivateStackOwner_ShouldReturnStackForOwner()
    {
        // Arrange
        var stackOwner = Guid.NewGuid();
        var stackId = Guid.NewGuid();

        _currentUserServiceMock.Setup(x => x.UserId).Returns(stackOwner);
        _currentUserServiceMock.Setup(x => x.IsAuthenticated).Returns(true);
        _validatorMock.Setup(x => x.ValidateAsync(It.IsAny<GetStackByIdRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new FluentValidation.Results.ValidationResult());

        var stack = new Stack
        {
            Id = stackId,
            Name = "Private Stack",
            Description = "A private test stack",
            Type = StackType.Frontend,
            IsPublic = false,
            UserId = stackOwner,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            IsActive = true,
            StackTechnologies = new List<StackTechnology>()
        };
        _context.Stacks.Add(stack);
        await _context.SaveChangesAsync();

        var request = new GetStackByIdRequest(stackId);

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(stackId);
        result.Name.Should().Be("Private Stack");
        result.IsPublic.Should().BeFalse();
        result.UserId.Should().Be(stackOwner);
    }

    [Fact]
    public async Task Handle_PrivateStackNonOwner_ShouldThrowUnauthorizedAccessException()
    {
        // Arrange
        var stackOwner = Guid.NewGuid();
        var currentUser = Guid.NewGuid();
        var stackId = Guid.NewGuid();

        _currentUserServiceMock.Setup(x => x.UserId).Returns(currentUser);
        _currentUserServiceMock.Setup(x => x.IsAuthenticated).Returns(true);
        _validatorMock.Setup(x => x.ValidateAsync(It.IsAny<GetStackByIdRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new FluentValidation.Results.ValidationResult());

        var stack = new Stack
        {
            Id = stackId,
            Name = "Private Stack",
            Description = "A private test stack",
            Type = StackType.Frontend,
            IsPublic = false,
            UserId = stackOwner,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            IsActive = true,
            StackTechnologies = new List<StackTechnology>()
        };
        _context.Stacks.Add(stack);
        await _context.SaveChangesAsync();

        var request = new GetStackByIdRequest(stackId);

        // Act & Assert
        var act = () => _handler.Handle(request, CancellationToken.None);
        await act.Should().ThrowAsync<UnauthorizedAccessException>()
            .WithMessage("Você não tem permissão para visualizar este stack");
    }

    [Fact]
    public async Task Handle_NonExistentStack_ShouldThrowNotFoundException()
    {
        // Arrange
        var currentUser = Guid.NewGuid();
        var nonExistentStackId = Guid.NewGuid();

        _currentUserServiceMock.Setup(x => x.UserId).Returns(currentUser);
        _validatorMock.Setup(x => x.ValidateAsync(It.IsAny<GetStackByIdRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new FluentValidation.Results.ValidationResult());

        var request = new GetStackByIdRequest(nonExistentStackId);

        // Act & Assert
        var act = () => _handler.Handle(request, CancellationToken.None);
        await act.Should().ThrowAsync<ArgumentException>()
            .WithMessage("Stack não encontrado");
    }

    [Fact]
    public async Task Handle_InactiveStack_ShouldThrowArgumentException()
    {
        // Arrange
        var stackOwner = Guid.NewGuid();
        var stackId = Guid.NewGuid();

        _currentUserServiceMock.Setup(x => x.UserId).Returns(stackOwner);
        _validatorMock.Setup(x => x.ValidateAsync(It.IsAny<GetStackByIdRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new FluentValidation.Results.ValidationResult());

        var stack = new Stack
        {
            Id = stackId,
            Name = "Inactive Stack",
            Description = "An inactive test stack",
            Type = StackType.Frontend,
            IsPublic = true,
            UserId = stackOwner,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            IsActive = false,
            StackTechnologies = new List<StackTechnology>()
        };
        _context.Stacks.Add(stack);
        await _context.SaveChangesAsync();

        var request = new GetStackByIdRequest(stackId);

        // Act & Assert
        var act = () => _handler.Handle(request, CancellationToken.None);
        await act.Should().ThrowAsync<ArgumentException>()
            .WithMessage("Stack não encontrado");
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}