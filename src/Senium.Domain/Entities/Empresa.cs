using FluentValidation.Results;
using Senium.Domain.Contracts.Interfaces;
using Senium.Domain.Validations;

namespace Senium.Domain.Entities;

public class Empresa : Entity, IAggregateRoot
{
    
    public string Nome { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string Telefone { get; set; } = null!;
    public string NomeDaEmpresa { get; set; } = null!;
    
    public override bool Validar(out ValidationResult validationResult)
    {
        validationResult = new EmpresaValidation().Validate(this);
        return validationResult.IsValid;
    }
}