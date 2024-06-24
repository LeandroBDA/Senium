using FluentValidation.Results;
using Senium.Domain.Contracts.Interfaces;
using Senium.Domain.Validations;

namespace Senium.Domain.Entities;

public class Usuario : Entity, IAggregateRoot
{
    public string Nome { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string Senha { get; set; } = null!;
    public DateTime DataDeNascimento { get; set; }
    public string? TokenDeResetSenha { get; set; }
    public DateTime? ExpiraResetToken { get; set; }

    public Curriculo Curriculo { get; set; } = null!;

    public override bool Validar(out ValidationResult validationResult)
    {
        validationResult = new UsuarioValidation().Validate(this);
        return validationResult.IsValid;
    }
}