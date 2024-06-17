using FluentValidation.Results;
using Senium.Domain.Contracts.Interfaces;
using Senium.Domain.Validations;

namespace Senium.Domain.Entities;

public class Administrador : Entity, IAggregateRoot
{
    public string Nome { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string Senha { get; set; } = null!;

    public override bool Validar(out ValidationResult validationResult)
    {
        validationResult = new AdministradorValidation().Validate(this);
        return validationResult.IsValid;
    }

  
}