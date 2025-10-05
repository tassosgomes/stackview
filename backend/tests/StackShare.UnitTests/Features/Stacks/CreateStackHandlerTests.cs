using FluentAssertions;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Moq;
using StackShare.Application.Features.Stacks;
using StackShare.Application.Interfaces;
using StackShare.Domain.Entities;
using StackShare.Domain.Enums;
using StackShare.Infrastructure.Data;

namespace StackShare.UnitTests.Features.Stacks;

public class CreateStackHandlerTests : IDisposable
{
    private readonly Mock<ICurrentUserService> _currentUserServiceMock;
    private readonly Mock<IValidator<CreateStackRequest>> _validatorMock;
    private readonly StackShareDbContext _context;
    private readonly CreateStackHandler _handler;

    public CreateStackHandlerTests()
    {
        var options = new DbContextOptionsBuilder<StackShareDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new StackShareDbContext(options);
        _currentUserServiceMock = new Mock<ICurrentUserService>();
        _validatorMock = new Mock<IValidator<CreateStackRequest>>();

        _handler = new CreateStackHandler(
            _context,
            _validatorMock.Object,
            _currentUserServiceMock.Object,
            Mock.Of<MediatR.IMediator>());
    }

    [Fact]
    public async Task Handle_ValidRequest_ShouldCreateStackSuccessfully()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var technologyId = Guid.NewGuid();
        
        _currentUserServiceMock.Setup(x => x.UserId).Returns(userId);
        _validatorMock.Setup(x => x.ValidateAsync(It.IsAny<CreateStackRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new FluentValidation.Results.ValidationResult());

        // Add test technology to context
        var technology = new Technology
        {
            Id = technologyId,
            Name = "React",
            Description = "JavaScript library",
            IsPreRegistered = true,
            IsActive = true
        };
        _context.Technologies.Add(technology);
        await _context.SaveChangesAsync();

        var request = new CreateStackRequest(
            "My Stack",
            "A test stack",
            StackType.Frontend,
            true,
            [technologyId],
            null);

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Name.Should().Be("My Stack");
        result.Description.Should().Be("A test stack");
        result.Type.Should().Be(StackType.Frontend);
        result.IsPublic.Should().BeTrue();
        result.UserId.Should().Be(userId);
        result.Technologies.Should().HaveCount(1);
        result.Technologies.First().Name.Should().Be("React");

        var stackInDb = await _context.Stacks.FirstOrDefaultAsync();
        stackInDb.Should().NotBeNull();
        stackInDb!.Name.Should().Be("My Stack");
    }

    [Fact]
    public async Task Handle_InvalidTechnologyId_ShouldThrowArgumentException()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var nonExistentTechnologyId = Guid.NewGuid();
        
        _currentUserServiceMock.Setup(x => x.UserId).Returns(userId);
        _validatorMock.Setup(x => x.ValidateAsync(It.IsAny<CreateStackRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new FluentValidation.Results.ValidationResult());

        var request = new CreateStackRequest(
            "My Stack",
            "A test stack",
            StackType.Frontend,
            true,
            [nonExistentTechnologyId],
            null);

        // Act & Assert
        var act = () => _handler.Handle(request, CancellationToken.None);
        await act.Should().ThrowAsync<ArgumentException>()
            .WithMessage("Uma ou mais tecnologias por ID não foram encontradas ou estão inativas");
    }

    [Fact]
    public async Task Handle_ValidationFailure_ShouldThrowValidationException()
    {
        // Arrange
        var userId = Guid.NewGuid();
        _currentUserServiceMock.Setup(x => x.UserId).Returns(userId);
        
        var validationErrors = new List<FluentValidation.Results.ValidationFailure>
        {
            new("Name", "Name is required")
        };
        var validationResult = new FluentValidation.Results.ValidationResult(validationErrors);
        
        _validatorMock.Setup(x => x.ValidateAsync(It.IsAny<CreateStackRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(validationResult);

        var request = new CreateStackRequest(
            "",
            "A test stack",
            StackType.Frontend,
            true,
            null,
            null);

        // Act & Assert
        var act = () => _handler.Handle(request, CancellationToken.None);
        await act.Should().ThrowAsync<ValidationException>();
    }

    [Fact]
    public async Task Handle_EmptyTechnologyList_ShouldCreateStackWithoutTechnologies()
    {
        // Arrange
        var userId = Guid.NewGuid();
        _currentUserServiceMock.Setup(x => x.UserId).Returns(userId);
        _validatorMock.Setup(x => x.ValidateAsync(It.IsAny<CreateStackRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new FluentValidation.Results.ValidationResult());

        var request = new CreateStackRequest(
            "My Stack",
            "A test stack",
            StackType.Frontend,
            true,
            null,
            null);

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Name.Should().Be("My Stack");
        result.Description.Should().Be("A test stack");
        result.Type.Should().Be(StackType.Frontend);
        result.IsPublic.Should().BeTrue();
        result.UserId.Should().Be(userId);
        result.Technologies.Should().BeEmpty();

        var stackInDb = await _context.Stacks.FirstOrDefaultAsync();
        stackInDb.Should().NotBeNull();
        stackInDb!.Name.Should().Be("My Stack");
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}