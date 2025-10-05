using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using StackShare.Application.Interfaces;

namespace StackShare.Application.Features.Stacks;

public record GetStackByIdRequest(Guid Id) : IRequest<StackResponse>;

public class GetStackByIdValidator : AbstractValidator<GetStackByIdRequest>
{
    public GetStackByIdValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("ID é obrigatório");
    }
}

public class GetStackByIdHandler : IRequestHandler<GetStackByIdRequest, StackResponse>
{
    private readonly IStackShareDbContext _context;
    private readonly IValidator<GetStackByIdRequest> _validator;
    private readonly ICurrentUserService _currentUserService;

    public GetStackByIdHandler(
        IStackShareDbContext context,
        IValidator<GetStackByIdRequest> validator,
        ICurrentUserService currentUserService)
    {
        _context = context;
        _validator = validator;
        _currentUserService = currentUserService;
    }

    public async Task<StackResponse> Handle(GetStackByIdRequest request, CancellationToken cancellationToken)
    {
        var validationResult = await _validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            throw new ValidationException(validationResult.Errors);
        }

        var stack = await _context.Stacks
            .Include(s => s.StackTechnologies)
            .ThenInclude(st => st.Technology)
            .FirstOrDefaultAsync(s => s.Id == request.Id && s.IsActive, cancellationToken);

        if (stack == null)
        {
            throw new ArgumentException("Stack não encontrado");
        }

        // Verificar permissão: stacks privados só podem ser visualizados pelo dono
        if (!stack.IsPublic)
        {
            if (!_currentUserService.IsAuthenticated || _currentUserService.UserId != stack.UserId)
            {
                throw new UnauthorizedAccessException("Você não tem permissão para visualizar este stack");
            }
        }

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