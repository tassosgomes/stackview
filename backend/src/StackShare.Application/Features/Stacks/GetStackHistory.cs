using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using StackShare.Application.Interfaces;
using StackShare.Domain.Enums;
using System.Text.Json;

namespace StackShare.Application.Features.Stacks;

public record GetStackHistoryRequest(Guid StackId) : IRequest<List<StackHistoryResponse>>;

public record StackHistoryResponse(
    Guid Id,
    int Version,
    string Name,
    string Description,
    StackType Type,
    DateTime CreatedAt,
    string ModifiedByUserName,
    List<TechnologyDto> Technologies);

public class GetStackHistoryValidator : AbstractValidator<GetStackHistoryRequest>
{
    public GetStackHistoryValidator()
    {
        RuleFor(x => x.StackId)
            .NotEmpty().WithMessage("ID do stack é obrigatório");
    }
}

public class GetStackHistoryHandler : IRequestHandler<GetStackHistoryRequest, List<StackHistoryResponse>>
{
    private readonly IStackShareDbContext _context;
    private readonly IValidator<GetStackHistoryRequest> _validator;
    private readonly ICurrentUserService _currentUserService;

    public GetStackHistoryHandler(
        IStackShareDbContext context,
        IValidator<GetStackHistoryRequest> validator,
        ICurrentUserService currentUserService)
    {
        _context = context;
        _validator = validator;
        _currentUserService = currentUserService;
    }

    public async Task<List<StackHistoryResponse>> Handle(GetStackHistoryRequest request, CancellationToken cancellationToken)
    {
        var validationResult = await _validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            throw new ValidationException(validationResult.Errors);
        }

        // Verificar se o stack existe e se o usuário tem permissão para ver o histórico
        var stack = await _context.Stacks
            .FirstOrDefaultAsync(s => s.Id == request.StackId && s.IsActive, cancellationToken);

        if (stack == null)
        {
            throw new ArgumentException("Stack não encontrado");
        }

        // Verificar permissão: apenas o dono pode ver histórico de stacks privados
        if (!stack.IsPublic && (_currentUserService.UserId != stack.UserId))
        {
            throw new UnauthorizedAccessException("Você não tem permissão para visualizar o histórico deste stack");
        }

        // Buscar histórico do stack
        var histories = await _context.StackHistories
            .Include(sh => sh.ModifiedByUser)
            .Where(sh => sh.StackId == request.StackId)
            .OrderByDescending(sh => sh.Version)
            .ToListAsync(cancellationToken);

        var result = new List<StackHistoryResponse>();

        foreach (var history in histories)
        {
            // Deserializar tecnologias do JSON
            var technologies = new List<TechnologyDto>();
            if (!string.IsNullOrEmpty(history.TechnologiesJson))
            {
                try
                {
                    using var doc = JsonDocument.Parse(history.TechnologiesJson);
                    foreach (var element in doc.RootElement.EnumerateArray())
                    {
                        var id = Guid.Parse(element.GetProperty("Id").GetString()!);
                        var name = element.GetProperty("Name").GetString()!;
                        var description = element.TryGetProperty("Description", out var descProp) 
                            ? descProp.GetString() 
                            : null;
                        
                        technologies.Add(new TechnologyDto(id, name, description));
                    }
                }
                catch (JsonException)
                {
                    // Se der erro na deserialização, continua com lista vazia
                    technologies = new List<TechnologyDto>();
                }
            }

            result.Add(new StackHistoryResponse(
                history.Id,
                history.Version,
                history.Name,
                history.Description,
                history.Type,
                history.CreatedAt,
                history.ModifiedByUser.UserName ?? history.ModifiedByUser.Email ?? "Usuário",
                technologies
            ));
        }

        return result;
    }
}