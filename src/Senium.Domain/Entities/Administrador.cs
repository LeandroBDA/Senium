using FluentValidation.Results;
using Senium.Domain.Contracts.Interfaces;
using Senium.Domain.Validations;

namespace Senium.Domain.Entities;

public class Administrador : Entity, IAggregateRoot
{
    public int Id { get; set; } 
    public string Email { get; set; } = null!;
    public string Senha { get; set; } = null!;
    public string HashedPassword { get; set; } = null!; // Criptografa a senha.
    
    public override bool Validar(out ValidationResult validationResult)
    {
        validationResult = new AdmValidation().Validate(this);
        return validationResult.IsValid;
    }
}