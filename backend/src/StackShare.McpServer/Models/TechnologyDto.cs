namespace StackShare.McpServer.Models;

public class TechnologyDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public bool IsPreRegistered { get; set; }
}