using FluentValidation;
using Senium.Domain.Entities;

namespace Senium.Domain.Validations;

public class ExperienciaValidation : AbstractValidator<Experiencia>
{
    public ExperienciaValidation()
    {
        RuleFor(e => e.Cargo)
            .NotEmpty().WithMessage("O campo Seu Cargo é obrigatório.");
            
        RuleFor(e => e.Empresa)
            .NotEmpty().WithMessage("O campo Empresa é obrigatório.");

        RuleFor(e => e.DataDeInicio)
            .NotEmpty().WithMessage("O campo Data de Início é obrigatório.");

        RuleFor(e => e.DataDeTermino)
            .NotEmpty()
            .WithMessage("O campo Data de Término é obrigatório quando Ainda trabalho aqui não está selecionado.")
            .When(e => !e.TrabalhoAtual);
            
        RuleFor(e => e.Descricao)
            .NotEmpty().WithMessage("O campo Descrição é obrigatório.")
            .MaximumLength(300).WithMessage("O campo Descrição deve ter no máximo 300 caracteres.");
    }
        
}