using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Identity;
using StackShare.Domain.Entities;

namespace StackShare.Application.Features.Authentication;

public record RegisterRequest(
    string Email,
    string Password,
    string ConfirmPassword) : IRequest<RegisterResponse>;

public record RegisterResponse(
    Guid UserId,
    string Email,
    string Message);

public class RegisterValidator : AbstractValidator<RegisterRequest>
{
    public RegisterValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email é obrigatório")
            .EmailAddress().WithMessage("Email deve ter um formato válido");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Senha é obrigatória")
            .MinimumLength(6).WithMessage("Senha deve ter pelo menos 6 caracteres");

        RuleFor(x => x.ConfirmPassword)
            .NotEmpty().WithMessage("Confirmação de senha é obrigatória")
            .Equal(x => x.Password).WithMessage("Senha e confirmação devem ser iguais");
    }
}

public class RegisterHandler : IRequestHandler<RegisterRequest, RegisterResponse>
{
    private readonly UserManager<User> _userManager;
    private readonly IValidator<RegisterRequest> _validator;

    public RegisterHandler(UserManager<User> userManager, IValidator<RegisterRequest> validator)
    {
        _userManager = userManager;
        _validator = validator;
    }

    public async Task<RegisterResponse> Handle(RegisterRequest request, CancellationToken cancellationToken)
    {
        var validationResult = await _validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            throw new ValidationException(validationResult.Errors);
        }

        var existingUser = await _userManager.FindByEmailAsync(request.Email);
        if (existingUser != null)
        {
            throw new InvalidOperationException("Email já está em uso");
        }

        var user = new User
        {
            Id = Guid.NewGuid(),
            UserName = request.Email,
            Email = request.Email,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        var result = await _userManager.CreateAsync(user, request.Password);
        if (!result.Succeeded)
        {
            var errors = string.Join(", ", result.Errors.Select(e => e.Description));
            throw new InvalidOperationException($"Falha ao criar usuário: {errors}");
        }

        return new RegisterResponse(user.Id, user.Email!, "Usuário criado com sucesso");
    }
}