using MediatR;
using StackShare.Domain.Entities;

namespace StackShare.Application.Features.Authentication;

public record GetUserProfileRequest(Guid UserId) : IRequest<UserProfileResponse>;

public record UserProfileResponse(
    Guid Id,
    string Email,
    DateTime CreatedAt,
    DateTime UpdatedAt);

public class GetUserProfileHandler : IRequestHandler<GetUserProfileRequest, UserProfileResponse>
{
    private readonly Microsoft.AspNetCore.Identity.UserManager<User> _userManager;

    public GetUserProfileHandler(Microsoft.AspNetCore.Identity.UserManager<User> userManager)
    {
        _userManager = userManager;
    }

    public async Task<UserProfileResponse> Handle(GetUserProfileRequest request, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByIdAsync(request.UserId.ToString());
        if (user == null)
        {
            throw new InvalidOperationException("Usuário não encontrado");
        }

        return new UserProfileResponse(
            user.Id,
            user.Email!,
            user.CreatedAt,
            user.UpdatedAt);
    }
}