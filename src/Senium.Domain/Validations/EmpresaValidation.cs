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
            .EmailAddress().WithMessage("O campo E-mail deve ser um endereço de e-mail válido.");

        RuleFor(e => e.Telefone)
            .NotEmpty().WithMessage("O campo Telefone é obrigatório.")
            .Matches(@"^\(\d{2}\)\d{8}$").WithMessage("O campo Telefone deve estar no formato correto: (XX)XXXXXXXX")
            .Length(12, 13).WithMessage("O campo Telefone deve conter entre 12 e 13 caracteres incluindo o DDD: (XX)XXXXXXXX");

        RuleFor(e => e.NomeDaEmpresa)
            .NotEmpty().WithMessage("O campo Nome da Empresa é obritgatório.");
    }
}