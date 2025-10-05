using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using StackShare.Application.Interfaces;
using StackShare.Domain.Entities;
using StackShare.Domain.Enums;

namespace StackShare.Application.Features.Stacks;

public record GetStacksRequest(
    int Page = 1,
    int PageSize = 20,
    StackType? Type = null,
    Guid? TechnologyId = null,
    string? Search = null,
    bool? OnlyPublic = null) : IRequest<PagedResult<StackSummaryResponse>>;

public record StackSummaryResponse(
    Guid Id,
    string Name,
    string Description,
    StackType Type,
    bool IsPublic,
    DateTime CreatedAt,
    DateTime UpdatedAt,
    Guid UserId,
    string UserName,
    List<TechnologyDto> Technologies);

public record PagedResult<T>(
    List<T> Items,
    int TotalCount,
    int Page,
    int PageSize,
    int TotalPages);

public class GetStacksValidator : AbstractValidator<GetStacksRequest>
{
    public GetStacksValidator()
    {
        RuleFor(x => x.Page)
            .GreaterThan(0).WithMessage("Página deve ser maior que 0");

        RuleFor(x => x.PageSize)
            .InclusiveBetween(1, 100).WithMessage("Tamanho da página deve estar entre 1 e 100");

        RuleFor(x => x.Type)
            .IsInEnum().When(x => x.Type.HasValue)
            .WithMessage("Tipo deve ser válido");
    }
}

public class GetStacksHandler : IRequestHandler<GetStacksRequest, PagedResult<StackSummaryResponse>>
{
    private readonly IStackShareDbContext _context;
    private readonly IValidator<GetStacksRequest> _validator;
    private readonly ICurrentUserService _currentUserService;

    public GetStacksHandler(
        IStackShareDbContext context,
        IValidator<GetStacksRequest> validator,
        ICurrentUserService currentUserService)
    {
        _context = context;
        _validator = validator;
        _currentUserService = currentUserService;
    }

    public async Task<PagedResult<StackSummaryResponse>> Handle(GetStacksRequest request, CancellationToken cancellationToken)
    {
        var validationResult = await _validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            throw new ValidationException(validationResult.Errors);
        }

        var query = _context.Stacks
            .Include(s => s.User)
            .Include(s => s.StackTechnologies)
            .ThenInclude(st => st.Technology)
            .Where(s => s.IsActive);

        // Aplicar filtros
        if (request.OnlyPublic.GetValueOrDefault(true))
        {
            // Se OnlyPublic for true (ou null), mostrar apenas públicos
            // Se for false, mostrar todos que o usuário tem permissão
            if (request.OnlyPublic == true)
            {
                query = query.Where(s => s.IsPublic);
            }
            else if (_currentUserService.IsAuthenticated)
            {
                // Mostrar stacks públicos + stacks privados do próprio usuário
                var currentUserId = _currentUserService.UserId;
                query = query.Where(s => s.IsPublic || s.UserId == currentUserId);
            }
            else
            {
                // Usuário não autenticado, apenas públicos
                query = query.Where(s => s.IsPublic);
            }
        }
        else
        {
            // OnlyPublic = false: mostrar tudo que o usuário tem permissão
            if (_currentUserService.IsAuthenticated)
            {
                var currentUserId = _currentUserService.UserId;
                query = query.Where(s => s.IsPublic || s.UserId == currentUserId);
            }
            else
            {
                query = query.Where(s => s.IsPublic);
            }
        }

        if (request.Type.HasValue)
        {
            query = query.Where(s => s.Type == request.Type.Value);
        }

        if (request.TechnologyId.HasValue)
        {
            query = query.Where(s => s.StackTechnologies.Any(st => st.TechnologyId == request.TechnologyId.Value));
        }

        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var searchTerm = request.Search.ToLower();
            query = query.Where(s => 
                s.Name.ToLower().Contains(searchTerm) ||
                s.Description.ToLower().Contains(searchTerm) ||
                s.StackTechnologies.Any(st => st.Technology.Name.ToLower().Contains(searchTerm)));
        }

        // Contar total
        var totalCount = await query.CountAsync(cancellationToken);

        // Aplicar paginação
        var stacks = await query
            .OrderByDescending(s => s.UpdatedAt)
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync(cancellationToken);

        var items = stacks.Select(s => new StackSummaryResponse(
            s.Id,
            s.Name,
            s.Description.Length > 200 ? s.Description.Substring(0, 200) + "..." : s.Description,
            s.Type,
            s.IsPublic,
            s.CreatedAt,
            s.UpdatedAt,
            s.UserId,
            s.User.UserName ?? s.User.Email ?? "Usuário",
            s.StackTechnologies.Select(st => new TechnologyDto(
                st.Technology.Id,
                st.Technology.Name,
                st.Technology.Description)).ToList()
        )).ToList();

        var totalPages = (int)Math.Ceiling((double)totalCount / request.PageSize);

        return new PagedResult<StackSummaryResponse>(items, totalCount, request.Page, request.PageSize, totalPages);
    }
}