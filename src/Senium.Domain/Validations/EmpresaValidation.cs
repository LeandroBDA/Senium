using FluentValidation;
using Senium.Domain.Entities;

namespace Senium.Domain.Validations;

public class EmpresaValidation : AbstractValidator<Empresa>
{
    public EmpresaValidation()
    {
        RuleFor(e => e.Nome)
            .NotEmpty().WithMessage("O campo Nome é obrigatório.")
            .Matches("^[a-zA-ZÀ-ÿ\\s]+$").WithMessage("O campo Nome deve conter apenas letras.")
            .MaximumLength(100).WithMessage("O campo Nome deve conter no máximo 100 caracteres.");
        
        RuleFor(u => u.Email)
            .NotEmpty().WithMessage("O campo E-mail é obrigatório.")
            .Matches(@"^[^@\s]+@[^@\s]+\.[^@\s]+$").WithMessage("O campo E-mail deve ser um endereço de e-mail válido.");

        RuleFor(e => e.Telefone)
            .NotEmpty().WithMessage("O campo Telefone é obrigatório.")
            .Matches(@"^[\d\(\)]+$").WithMessage("O campo Telefone deve conter apenas números e parênteses.")
            .Length(12, 13).WithMessage("O campo Telefone deve conter entre 12 e 13 caracteres incluindo o DDD: (XX)9XXXXXXXX ou (XX)XXXXXXXX");
        
        RuleFor(e => e.NomeDaEmpresa)
            .NotEmpty().WithMessage("O campo Nome da Empresa é obritgatório.");
    }
}