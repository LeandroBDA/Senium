using FluentValidation.Results;
using Senium.Domain.Contracts.Interfaces;
using Senium.Domain.Validations;

namespace Senium.Domain.Entities;

public class Experiencia : Entity, IAggregateRoot
{
    public int UsuarioId { get; set; }
    public string Cargo { get; set; } = null!;
    public string Empresa { get; set; } = null!;
    public DateTime DataDeInicio { get; set; }
    public DateTime? DataDeTermino { get; set; }
    public bool TrabalhoAtual { get; set; }
    public string Descricao { get; set; } = null!;

    public Usuario Usuario { get; set; } = null!;

    public override bool Validar(out ValidationResult validationResult)
    {
        validationResult = new ExperienciaValidation().Validate(this);
        return validationResult.IsValid;
    }
}