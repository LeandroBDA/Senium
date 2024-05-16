namespace Senium.Application.Dto.V1.Curriculo;

public class CurriculoDto
{
    public int Id { get; set; }
    public string Telefone { get; set; } = null!;
    public string EstadoCivil { get; set; } = null!;
    public string Genero { get; set; } = null!;
    public string RacaEtnia { get; set; } = null!;
    public string GrauDeFormacao { get; set; } = null!;
    public string Cep { get; set; } = null!;
    public string Endereco { get; set; } = null!;
    public string Cidade { get; set; } = null!;
    public string Estado { get; set; } = null!;

    public string EPessoaComDeficiencia { get; set; } = null!;
       
    public bool EDeficienciaAuditiva { get; set; } 
        
    public bool EDeficienciaFisica { get; set; }
        
    public bool EDeficienciaIntelectual { get; set; }
        
    public bool EDeficienciaMotora { get; set; }
       
    public bool EDeficienciaVisual { get; set; }
        
       
    public bool ELgbtqia { get; set; }
    public bool EBaixaRenda { get; set; }
}