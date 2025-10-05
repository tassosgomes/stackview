using StackShare.Domain.Enums;

namespace StackShare.Domain.Entities;

public class Stack
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public StackType Type { get; set; }
    public bool IsPublic { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    public bool IsActive { get; set; } = true;
    
    // Foreign keys
    public Guid UserId { get; set; }
    
    // Navigation properties
    public User User { get; set; } = null!;
    public ICollection<StackTechnology> StackTechnologies { get; set; } = new List<StackTechnology>();
    public ICollection<StackHistory> StackHistories { get; set; } = new List<StackHistory>();
}