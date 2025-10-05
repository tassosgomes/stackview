namespace StackShare.Domain.Entities;

public class StackTechnology
{
    public Guid Id { get; set; }
    public Guid StackId { get; set; }
    public Guid TechnologyId { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    // Navigation properties
    public Stack Stack { get; set; } = null!;
    public Technology Technology { get; set; } = null!;
}