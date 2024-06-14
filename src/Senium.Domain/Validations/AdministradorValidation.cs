using FluentValidation;
using Senium.Domain.Entities;

namespace Senium.Domain.Validations;

public class AdministradorValidation : AbstractValidator<Administrador>
{
    public AdministradorValidation()
    {
        RuleFor(a => a.Nome)
            .NotEmpty().WithMessage("O campo Nome é obrigatório.")
            .Length(1,50).WithMessage("O campo Nome deve conter 1 a 50 caracteres")
            .Matches("^[a-zA-ZÀ-ÿ ]+$").WithMessage("O campo Nome deve conter somente letras.");
            
        RuleFor(u => u.Email)
            .NotEmpty().WithMessage("O campo E-mail é obrigatório.")
            .Matches(@"^[^@\s]+@[^@\s]+\.(com|com\.br)$").WithMessage("O campo E-mail deve ser um endereço de e-mail válido e terminar com .com ou .com.br.");
        
        RuleFor(a => a.Senha)
            .NotEmpty().WithMessage("O campo Senha é obrigatório.")
            .Length(8, 20).WithMessage("A senha deve ter entre 8 a 20 caracteres.")
            .Matches(@"^(?=.*[a-zA-Z])(?=.*\d)(?=.*[^\w\d]).{8,20}$").WithMessage("A senha deve conter letras, números e símbolos.");

    }
}