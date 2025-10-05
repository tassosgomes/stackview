namespace StackShare.Domain.Entities;

public class Technology
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    public bool IsActive { get; set; } = true;
    public bool IsPreRegistered { get; set; } = false; // True para tecnologias pré-cadastradas por admin
    
    // Navigation properties
    public ICollection<StackTechnology> StackTechnologies { get; set; } = new List<StackTechnology>();
}