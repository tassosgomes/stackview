using FluentValidation;

namespace StackShare.Application.Features.Technologies;

public class CreateTechnologyRequestValidator : AbstractValidator<CreateTechnologyRequest>
{
    public CreateTechnologyRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Nome da tecnologia é obrigatório")
            .MaximumLength(100).WithMessage("Nome da tecnologia deve ter no máximo 100 caracteres");

        RuleFor(x => x.Description)
            .MaximumLength(500).WithMessage("Descrição deve ter no máximo 500 caracteres")
            .When(x => !string.IsNullOrEmpty(x.Description));
    }
}

public class SuggestTechnologiesRequestValidator : AbstractValidator<SuggestTechnologiesRequest>
{
    public SuggestTechnologiesRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Nome da tecnologia é obrigatório para sugestão")
            .MinimumLength(2).WithMessage("Nome deve ter pelo menos 2 caracteres");

        RuleFor(x => x.MaxResults)
            .GreaterThan(0).WithMessage("MaxResults deve ser maior que 0")
            .LessThanOrEqualTo(50).WithMessage("MaxResults deve ser menor ou igual a 50");
    }
}

public class GetTechnologiesRequestValidator : AbstractValidator<GetTechnologiesRequest>
{
    public GetTechnologiesRequestValidator()
    {
        RuleFor(x => x.Page)
            .GreaterThan(0).WithMessage("Page deve ser maior que 0");

        RuleFor(x => x.PageSize)
            .GreaterThan(0).WithMessage("PageSize deve ser maior que 0")
            .LessThanOrEqualTo(100).WithMessage("PageSize deve ser menor ou igual a 100");

        RuleFor(x => x.Search)
            .MinimumLength(2).WithMessage("Search deve ter pelo menos 2 caracteres")
            .When(x => !string.IsNullOrEmpty(x.Search));
    }
}