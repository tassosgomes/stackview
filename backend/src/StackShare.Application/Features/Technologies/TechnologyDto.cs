namespace StackShare.Application.Features.Technologies;

public class TechnologyDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool IsPreRegistered { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class TechnologySummaryDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
}

public class CreateTechnologyRequest
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
}

public class CreateTechnologyResponse
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool IsPreRegistered { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class SuggestTechnologiesRequest
{
    public string Name { get; set; } = string.Empty;
    public int MaxResults { get; set; } = 10;
}

public class SuggestTechnologiesResponse
{
    public List<TechnologySuggestionDto> Suggestions { get; set; } = new();
}

public class TechnologySuggestionDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int Score { get; set; } // FuzzySharp score
}

public class GetTechnologiesRequest
{
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? Search { get; set; }
    public bool? OnlyPreRegistered { get; set; }
}