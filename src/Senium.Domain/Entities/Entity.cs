using FluentValidation.Results;
using Senium.Domain.Contracts.Interfaces;

namespace Senium.Domain.Entities;

public abstract class Entity : BaseEntity, ITracking
{
    public DateTime CriadoEm { get; set; }
    public DateTime AtualizadoEm { get; set; }


    public virtual bool Validar(out ValidationResult validationResult)
    {
        validationResult = new ValidationResult();
        return validationResult.IsValid;
    }
}

public abstract class BaseEntity
{
    public int Id { get; set; }
}