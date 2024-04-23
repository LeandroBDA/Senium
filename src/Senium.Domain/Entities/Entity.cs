using Senium.Domain.Contracts.Interfaces;

namespace Senium.Domain.Entities;

public abstract class Entity : BaseEntity, ITracking
{
    public DateTime CriadoEm { get; set; }
    public DateTime AtualizadoEm { get; set; }
    
}

public abstract class BaseEntity
{
    public int Id { get; set; }
}