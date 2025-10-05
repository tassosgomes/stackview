using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using StackShare.Application.Interfaces;
using StackShare.Domain.Entities;
using StackShare.Domain.Enums;
using System.Text.Json;

namespace StackShare.Application.Features.Stacks;

public record UpdateStackRequest(
    Guid Id,
    string Name,
    string Description,
    StackType Type,
    bool IsPublic,
    List<Guid> TechnologyIds) : IRequest<StackResponse>;

public class UpdateStackValidator : AbstractValidator<UpdateStackRequest>
{
    public UpdateStackValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("ID é obrigatório");

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

public class UpdateStackHandler : IRequestHandler<UpdateStackRequest, StackResponse>
{
    private readonly IStackShareDbContext _context;
    private readonly IValidator<UpdateStackRequest> _validator;
    private readonly ICurrentUserService _currentUserService;

    public UpdateStackHandler(
        IStackShareDbContext context,
        IValidator<UpdateStackRequest> validator,
        ICurrentUserService currentUserService)
    {
        _context = context;
        _validator = validator;
        _currentUserService = currentUserService;
    }

    public async Task<StackResponse> Handle(UpdateStackRequest request, CancellationToken cancellationToken)
    {
        var validationResult = await _validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            throw new ValidationException(validationResult.Errors);
        }

        // Buscar stack existente com tecnologias
        var stack = await _context.Stacks
            .Include(s => s.StackTechnologies)
            .ThenInclude(st => st.Technology)
            .FirstOrDefaultAsync(s => s.Id == request.Id && s.IsActive, cancellationToken);

        if (stack == null)
        {
            throw new ArgumentException("Stack não encontrado");
        }

        // Verificar se o usuário é o dono do stack
        if (stack.UserId != _currentUserService.UserId)
        {
            throw new UnauthorizedAccessException("Você não tem permissão para editar este stack");
        }

        // Verificar se todas as novas tecnologias existem
        var newTechnologies = await _context.Technologies
            .Where(t => request.TechnologyIds.Contains(t.Id) && t.IsActive)
            .ToListAsync(cancellationToken);

        if (newTechnologies.Count != request.TechnologyIds.Count)
        {
            throw new ArgumentException("Uma ou mais tecnologias não foram encontradas ou estão inativas");
        }

        // Criar histórico ANTES de modificar o stack
        await CreateStackHistory(stack, cancellationToken);

        // Atualizar stack
        stack.Name = request.Name;
        stack.Description = request.Description;
        stack.Type = request.Type;
        stack.IsPublic = request.IsPublic;
        stack.UpdatedAt = DateTime.UtcNow;

        // Remover tecnologias antigas
        _context.StackTechnologies.RemoveRange(stack.StackTechnologies);

        // Adicionar novas tecnologias
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

        // Recarregar stack com novas tecnologias
        var updatedStack = await _context.Stacks
            .Include(s => s.StackTechnologies)
            .ThenInclude(st => st.Technology)
            .FirstAsync(s => s.Id == request.Id, cancellationToken);

        return MapToResponse(updatedStack);
    }

    private async Task CreateStackHistory(Stack stack, CancellationToken cancellationToken)
    {
        // Obter o próximo número de versão
        var lastVersion = await _context.StackHistories
            .Where(sh => sh.StackId == stack.Id)
            .MaxAsync(sh => (int?)sh.Version, cancellationToken) ?? 0;

        // Serializar tecnologias atuais para JSON
        var technologiesJson = JsonSerializer.Serialize(
            stack.StackTechnologies.Select(st => new 
            { 
                Id = st.Technology.Id, 
                Name = st.Technology.Name, 
                Description = st.Technology.Description 
            }).ToList());

        var history = new StackHistory
        {
            Id = Guid.NewGuid(),
            StackId = stack.Id,
            Version = lastVersion + 1,
            Name = stack.Name,
            Description = stack.Description,
            Type = stack.Type,
            TechnologiesJson = technologiesJson,
            CreatedAt = DateTime.UtcNow,
            ModifiedByUserId = _currentUserService.UserId
        };

        _context.StackHistories.Add(history);
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