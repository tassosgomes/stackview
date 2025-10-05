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
    List<Guid>? TechnologyIds,
    List<string>? TechnologyNames) : IRequest<StackResponse>;

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

        RuleFor(x => x)
            .Must(x => (x.TechnologyIds?.Any() == true) || (x.TechnologyNames?.Any() == true))
            .WithMessage("Pelo menos uma tecnologia deve ser selecionada (ID ou nome)");

        RuleFor(x => x.TechnologyIds)
            .Must(ids => ids == null || ids.All(id => id != Guid.Empty))
            .WithMessage("Todos os IDs de tecnologia devem ser válidos");

        RuleFor(x => x.TechnologyNames)
            .Must(names => names == null || names.All(name => !string.IsNullOrWhiteSpace(name)))
            .WithMessage("Todos os nomes de tecnologia devem ser válidos");
    }
}

public class CreateStackHandler : IRequestHandler<CreateStackRequest, StackResponse>
{
    private readonly IStackShareDbContext _context;
    private readonly IValidator<CreateStackRequest> _validator;
    private readonly ICurrentUserService _currentUserService;
    private readonly IMediator _mediator;

    public CreateStackHandler(
        IStackShareDbContext context, 
        IValidator<CreateStackRequest> validator,
        ICurrentUserService currentUserService,
        IMediator mediator)
    {
        _context = context;
        _validator = validator;
        _currentUserService = currentUserService;
        _mediator = mediator;
    }

    public async Task<StackResponse> Handle(CreateStackRequest request, CancellationToken cancellationToken)
    {
        var validationResult = await _validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            throw new ValidationException(validationResult.Errors);
        }

        var allTechnologyIds = new List<Guid>();

        // Processar IDs de tecnologias se fornecidos
        if (request.TechnologyIds?.Any() == true)
        {
            var existingTechById = await _context.Technologies
                .Where(t => request.TechnologyIds.Contains(t.Id) && t.IsActive)
                .ToListAsync(cancellationToken);

            if (existingTechById.Count != request.TechnologyIds.Count)
            {
                throw new InvalidOperationException("Uma ou mais tecnologias por ID não foram encontradas ou estão inativas");
            }

            allTechnologyIds.AddRange(existingTechById.Select(t => t.Id));
        }

        // Processar nomes de tecnologias se fornecidos
        if (request.TechnologyNames?.Any() == true)
        {
            foreach (var techName in request.TechnologyNames)
            {
                var techCommand = new Technologies.CreateOrGetTechnology(techName);
                var techId = await _mediator.Send(techCommand, cancellationToken);
                allTechnologyIds.Add(techId);
            }
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
        foreach (var technologyId in allTechnologyIds.Distinct()) // Distinct para evitar duplicatas
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