using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using StackShare.Application.Interfaces;
using StackShare.Domain.Entities;

namespace StackShare.Application.Features.Technologies;

public class CreateOrGetTechnology : IRequest<Guid>
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }

    public CreateOrGetTechnology(string name, string? description = null)
    {
        Name = name;
        Description = description;
    }
}

public class CreateOrGetTechnologyHandler : IRequestHandler<CreateOrGetTechnology, Guid>
{
    private readonly IStackShareDbContext _context;
    private readonly ILogger<CreateOrGetTechnologyHandler> _logger;

    public CreateOrGetTechnologyHandler(
        IStackShareDbContext context,
        ILogger<CreateOrGetTechnologyHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<Guid> Handle(CreateOrGetTechnology request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Buscando ou criando tecnologia: {Name}", request.Name);

        // Verificar se já existe uma tecnologia com o mesmo nome
        var existingTechnology = await _context.Technologies
            .FirstOrDefaultAsync(t => t.Name.ToLower() == request.Name.ToLower(), cancellationToken);

        if (existingTechnology != null)
        {
            if (!existingTechnology.IsActive)
            {
                // Reativar tecnologia se estava inativa
                _logger.LogInformation("Reativando tecnologia: {Name}", request.Name);
                existingTechnology.IsActive = true;
                existingTechnology.UpdatedAt = DateTime.UtcNow;
                await _context.SaveChangesAsync(cancellationToken);
            }

            _logger.LogInformation("Tecnologia existente encontrada: {TechnologyId} - {Name}", 
                existingTechnology.Id, existingTechnology.Name);
            return existingTechnology.Id;
        }

        // Criar nova tecnologia (não pré-registrada, criada pelo usuário)
        var technology = new Technology
        {
            Id = Guid.NewGuid(),
            Name = request.Name.Trim(),
            Description = request.Description?.Trim(),
            IsPreRegistered = false, // Tecnologias criadas por usuários não são pré-registradas
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _context.Technologies.Add(technology);
        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Nova tecnologia criada pelo usuário: {TechnologyId} - {Name}", 
            technology.Id, technology.Name);

        return technology.Id;
    }
}