﻿using FluentValidation;
using Senium.Domain.Entities;

namespace Senium.Domain.Validations;

public class AdministradorValidation : AbstractValidator<Administrador>
{
    public AdministradorValidation()
    {
        RuleFor(a => a.Nome)
            .NotEmpty().WithMessage("O campo Nome é obrigatório.")
            .Length(3,50).WithMessage("O campo Nome deve conter 3 a 50 caracteres")
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

public class AtualizarAdministradorValidator : AbstractValidator<Administrador>
{
    public AtualizarAdministradorValidator()
    {
        RuleFor(a => a.Nome)
            .NotEmpty().WithMessage("O campo Nome é obrigatório.")
            .Length(3, 50).WithMessage("O campo Nome deve conter entre 3 a 50 caracteres.")
            .Matches("^[a-zA-ZÀ-ÿ ]+$").When(a => !string.IsNullOrEmpty(a.Nome)).WithMessage("O campo Nome deve conter somente letras.");

        RuleFor(u => u.Email)
            .NotEmpty().WithMessage("O campo E-mail é obrigatório.")
            .Matches(@"^[^@\s]+@[^@\s]+\.(com|com\.br)$").WithMessage("O campo E-mail deve ser um endereço de e-mail válido e terminar com .com ou .com.br.");

        RuleFor(a => a.Senha)
            .Length(8, 20).When(a => !string.IsNullOrEmpty(a.Senha)).WithMessage("A senha deve ter entre 8 e 20 caracteres.")
            .Matches(@"^(?=.*[a-zA-Z])(?=.*\d)(?=.*[^\w\d]).{8,20}$").When(a => !string.IsNullOrEmpty(a.Senha)).WithMessage("A senha deve conter letras, números e símbolos.");
    }
}

public class SenhaAdministradorValidator : AbstractValidator<string>
{
    public SenhaAdministradorValidator()
    {
        RuleFor(s => s)
            .NotEmpty().WithMessage("O campo Senha é obrigatório.")
            .Length(8, 20).WithMessage("A senha deve ter entre 8 e 20 caracteres.")
            .Matches(@"^(?=.*[a-zA-Z])(?=.*\d)(?=.*[^a-zA-Z0-9]).{8,20}$")
            .WithMessage("A senha deve conter letras, números, símbolos e ter entre 8 e 20 caracteres.");
    }
}