using Microsoft.AspNetCore.Identity;

namespace StackShare.Domain.Entities;

public class User : IdentityUser<Guid>
{
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    
    // Navigation properties
    public ICollection<Stack> Stacks { get; set; } = new List<Stack>();
    public ICollection<McpApiToken> McpApiTokens { get; set; } = new List<McpApiToken>();
}