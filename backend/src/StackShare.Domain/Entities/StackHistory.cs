using StackShare.Domain.Enums;

namespace StackShare.Domain.Entities;

public class StackHistory
{
    public Guid Id { get; set; }
    public Guid StackId { get; set; }
    public int Version { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public StackType Type { get; set; }
    public string TechnologiesJson { get; set; } = string.Empty; // Snapshot das tecnologias em JSON
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public Guid ModifiedByUserId { get; set; }
    
    // Navigation properties
    public Stack Stack { get; set; } = null!;
    public User ModifiedByUser { get; set; } = null!;
}