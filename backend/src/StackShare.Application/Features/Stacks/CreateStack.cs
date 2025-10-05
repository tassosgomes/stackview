using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using StackShare.Application.Interfaces;
using StackShare.Domain.Entities;
using StackShare.Domain.Enums;
using System.Text.Json;

namespace StackShare.Application.Features.Stacks;

public record CreateStackRequest(
    string Name,
    string Description,
    StackType Type,
    bool IsPublic,
    List<Guid> TechnologyIds) : IRequest<StackResponse>;

public record StackResponse(
    Guid Id,
    string Name,
    string Description,
    StackType Type,
    bool IsPublic,
    DateTime CreatedAt,
    DateTime UpdatedAt,
    Guid UserId,
    List<TechnologyDto> Technologies);

public record TechnologyDto(
    Guid Id,
    string Name,
    string? Description);

public class CreateStackValidator : AbstractValidator<CreateStackRequest>
{
    public CreateStackValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Nome é obrigatório")
            .MaximumLength(200).WithMessage("Nome deve ter no máximo 200 caracteres");

        RuleFor(x => x.Description)
            .NotEmpty().WithMessage("Descrição é obrigatória")
            .MaximumLength(5000).WithMessage("Descrição deve ter no máximo 5000 caracteres");

        RuleFor(x => x.Type)
            .IsInEnum().WithMessage("Tipo deve ser válido");

        RuleFor(x => x.TechnologyIds)
            .NotEmpty().WithMessage("Pelo menos uma tecnologia deve ser selecionada")
            .Must(ids => ids.All(id => id != Guid.Empty))
            .WithMessage("Todas as tecnologias devem ter IDs válidos");
    }
}

public class CreateStackHandler : IRequestHandler<CreateStackRequest, StackResponse>
{
    private readonly IStackShareDbContext _context;
    private readonly IValidator<CreateStackRequest> _validator;
    private readonly ICurrentUserService _currentUserService;

    public CreateStackHandler(
        IStackShareDbContext context, 
        IValidator<CreateStackRequest> validator,
        ICurrentUserService currentUserService)
    {
        _context = context;
        _validator = validator;
        _currentUserService = currentUserService;
    }

    public async Task<StackResponse> Handle(CreateStackRequest request, CancellationToken cancellationToken)
    {
        var validationResult = await _validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            throw new ValidationException(validationResult.Errors);
        }

        // Verificar se todas as tecnologias existem
        var technologies = await _context.Technologies
            .Where(t => request.TechnologyIds.Contains(t.Id) && t.IsActive)
            .ToListAsync(cancellationToken);

        if (technologies.Count != request.TechnologyIds.Count)
        {
            throw new ArgumentException("Uma ou mais tecnologias não foram encontradas ou estão inativas");
        }

        // Criar o stack
        var stack = new Stack
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            Description = request.Description,
            Type = request.Type,
            IsPublic = request.IsPublic,
            UserId = _currentUserService.UserId,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _context.Stacks.Add(stack);

        // Adicionar tecnologias ao stack
        foreach (var technologyId in request.TechnologyIds)
        {
            var stackTechnology = new StackTechnology
            {
                Id = Guid.NewGuid(),
                StackId = stack.Id,
                TechnologyId = technologyId,
                CreatedAt = DateTime.UtcNow
            };
            _context.StackTechnologies.Add(stackTechnology);
        }

        await _context.SaveChangesAsync(cancellationToken);

        // Carregar stack com tecnologias para resposta
        var createdStack = await _context.Stacks
            .Include(s => s.StackTechnologies)
            .ThenInclude(st => st.Technology)
            .FirstAsync(s => s.Id == stack.Id, cancellationToken);

        return MapToResponse(createdStack);
    }



    private static StackResponse MapToResponse(Stack stack)
    {
        return new StackResponse(
            stack.Id,
            stack.Name,
            stack.Description,
            stack.Type,
            stack.IsPublic,
            stack.CreatedAt,
            stack.UpdatedAt,
            stack.UserId,
            stack.StackTechnologies.Select(st => new TechnologyDto(
                st.Technology.Id,
                st.Technology.Name,
                st.Technology.Description)).ToList()
        );
    }
}