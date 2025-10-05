using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using StackShare.Application.Interfaces;
using StackShare.Domain.Entities;

namespace StackShare.Application.Features.Technologies;

public class CreateTechnology : IRequest<CreateTechnologyResponse>
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }

    public CreateTechnology(string name, string? description = null)
    {
        Name = name;
        Description = description;
    }
}

public class CreateTechnologyHandler : IRequestHandler<CreateTechnology, CreateTechnologyResponse>
{
    private readonly IStackShareDbContext _context;
    private readonly ILogger<CreateTechnologyHandler> _logger;

    public CreateTechnologyHandler(
        IStackShareDbContext context,
        ILogger<CreateTechnologyHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<CreateTechnologyResponse> Handle(CreateTechnology request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Criando nova tecnologia: {Name}", request.Name);

        // Verificar se já existe uma tecnologia com o mesmo nome
        var existingTechnology = await _context.Technologies
            .FirstOrDefaultAsync(t => t.Name.ToLower() == request.Name.ToLower(), cancellationToken);

        if (existingTechnology != null)
        {
            if (existingTechnology.IsActive)
            {
                _logger.LogWarning("Tentativa de criar tecnologia que já existe: {Name}", request.Name);
                throw new InvalidOperationException($"Já existe uma tecnologia com o nome '{request.Name}'");
            }
            else
            {
                // Reativar tecnologia existente se estava inativa
                _logger.LogInformation("Reativando tecnologia existente: {Name}", request.Name);
                existingTechnology.IsActive = true;
                existingTechnology.IsPreRegistered = true;
                existingTechnology.Description = request.Description ?? existingTechnology.Description;
                existingTechnology.UpdatedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync(cancellationToken);

                return new CreateTechnologyResponse
                {
                    Id = existingTechnology.Id,
                    Name = existingTechnology.Name,
                    Description = existingTechnology.Description,
                    IsPreRegistered = existingTechnology.IsPreRegistered,
                    CreatedAt = existingTechnology.CreatedAt
                };
            }
        }

        // Criar nova tecnologia
        var technology = new Technology
        {
            Id = Guid.NewGuid(),
            Name = request.Name.Trim(),
            Description = request.Description?.Trim(),
            IsPreRegistered = true, // Tecnologias criadas por admin são sempre pré-registradas
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _context.Technologies.Add(technology);
        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Tecnologia criada com sucesso: {TechnologyId} - {Name}", 
            technology.Id, technology.Name);

        return new CreateTechnologyResponse
        {
            Id = technology.Id,
            Name = technology.Name,
            Description = technology.Description,
            IsPreRegistered = technology.IsPreRegistered,
            CreatedAt = technology.CreatedAt
        };
    }
}