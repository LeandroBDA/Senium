namespace Senium.Application.Dto.V1.Experiencia;

public class ExperienciaDto
{
    public string Cargo { get; set; } = null!;
    public string Empresa { get; set; } = null!;
    public string Descricao { get; set; } = null!;
    public DateTime DataDeInicio { get; set; }
    public DateTime? DataDeTermino { get; set; }
    public bool TrabalhoAtual { get; set; }
}