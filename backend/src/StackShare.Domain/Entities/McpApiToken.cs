namespace StackShare.Domain.Entities;

public class McpApiToken
{
    public Guid Id { get; set; }
    public string TokenHash { get; set; } = string.Empty; // Hash do token para segurança
    public string Name { get; set; } = string.Empty; // Nome do token para o usuário identificar
    public Guid UserId { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? ExpiresAt { get; set; }
    public bool IsRevoked { get; set; } = false;
    public DateTime? RevokedAt { get; set; }
    public DateTime? LastUsedAt { get; set; }
    
    // Navigation properties
    public User User { get; set; } = null!;
}