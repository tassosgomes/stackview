using Microsoft.EntityFrameworkCore;
using StackShare.Domain.Entities;

namespace StackShare.Application.Interfaces;

public interface IStackShareDbContext
{
    DbSet<Stack> Stacks { get; }
    DbSet<Technology> Technologies { get; }
    DbSet<StackTechnology> StackTechnologies { get; }
    DbSet<StackHistory> StackHistories { get; }
    DbSet<McpApiToken> McpApiTokens { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}