using FluentValidation;
using Senium.Domain.Entities;

namespace Senium.Domain.Validations;

public class UsuarioValidation : AbstractValidator<Usuario>
{
    public UsuarioValidation()
    {
        RuleFor(u => u.Nome)
            .NotEmpty().WithMessage("O campo Nome é obrigatório.")
            .Length(3, 100).WithMessage("O campo Nome deve conter 3 a 100 caracteres")
            .Matches("^[a-zA-ZÀ-ÿ ]+$").WithMessage("O campo Nome deve conter somente letras.");

        RuleFor(u => u.Email)
            .NotEmpty().WithMessage("O campo E-mail é obrigatório.")
            .Matches(@"^[^@\s]+@[^@\s]+\.[^@\s]+$").WithMessage("O campo E-mail deve ser um endereço de e-mail válido.");

        RuleFor(u => u.DataDeNascimento)
            .NotEmpty().WithMessage("O campo Data de Nascimento é obrigatório.")
            .Must(TemMinimoDeIdade).WithMessage("Você deve ter 45 anos ou mais para se cadastrar.");

        RuleFor(u => u.Senha)
            .NotEmpty().WithMessage("O campo Senha é obrigatório.")
            .Length(8, 20).WithMessage("A senha deve ter entre 8 e 20 caracteres.")
            .Matches(@"^(?=.*[a-zA-Z])(?=.*\d)(?=.*[^a-zA-Z0-9]).{8,20}$")
            .WithMessage("A senha deve conter letras, números, símbolos e ter entre 8 e 20 caracteres.");
    }

    private bool TemMinimoDeIdade(DateTime date)
    {
        var anoAtual = DateTime.Today.Year;
        
        var idade = anoAtual - date.Year;
        
        return idade >= 45;
    }
    
}

public class SenhaUsuarioValidator : AbstractValidator<string>
{
    public SenhaUsuarioValidator()
    {
        RuleFor(s => s)
            .NotEmpty().WithMessage("O campo Senha é obrigatório.")
            .Length(8, 20).WithMessage("A senha deve ter entre 8 e 20 caracteres.")
            .Matches(@"^(?=.*[a-zA-Z])(?=.*\d)(?=.*[^a-zA-Z0-9]).{8,20}$")
            .WithMessage("A senha deve conter letras, números, símbolos e ter entre 8 e 20 caracteres.");
    }
}