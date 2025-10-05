namespace StackShare.Application.Interfaces;

public interface ICurrentUserService
{
    Guid UserId { get; }
    string? Email { get; }
    bool IsAuthenticated { get; }
}