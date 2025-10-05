using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using StackShare.Application.Interfaces;

namespace StackShare.Application.Features.Stacks;

public record DeleteStackRequest(Guid Id) : IRequest;

public class DeleteStackValidator : AbstractValidator<DeleteStackRequest>
{
    public DeleteStackValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("ID é obrigatório");
    }
}

public class DeleteStackHandler : IRequestHandler<DeleteStackRequest>
{
    private readonly IStackShareDbContext _context;
    private readonly IValidator<DeleteStackRequest> _validator;
    private readonly ICurrentUserService _currentUserService;

    public DeleteStackHandler(
        IStackShareDbContext context,
        IValidator<DeleteStackRequest> validator,
        ICurrentUserService currentUserService)
    {
        _context = context;
        _validator = validator;
        _currentUserService = currentUserService;
    }

    public async Task Handle(DeleteStackRequest request, CancellationToken cancellationToken)
    {
        var validationResult = await _validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            throw new ValidationException(validationResult.Errors);
        }

        var stack = await _context.Stacks
            .FirstOrDefaultAsync(s => s.Id == request.Id && s.IsActive, cancellationToken);

        if (stack == null)
        {
            throw new ArgumentException("Stack não encontrado");
        }

        // Verificar se o usuário é o dono do stack
        if (stack.UserId != _currentUserService.UserId)
        {
            throw new UnauthorizedAccessException("Você não tem permissão para excluir este stack");
        }

        // Soft delete: marcar como inativo ao invés de remover da base
        stack.IsActive = false;
        stack.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);
    }
}