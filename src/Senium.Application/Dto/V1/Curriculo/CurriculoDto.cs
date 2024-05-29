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
    public bool EPessoaComDeficiencia { get; set; }
       
    public bool EDeficienciaAuditiva { get; set; } 
        
    public bool EDeficienciaFisica { get; set; }
        
    public bool EDeficienciaIntelectual { get; set; }
        
    public bool EDeficienciaMotora { get; set; }
       
    public bool EDeficienciaVisual { get; set; }
    public bool ELgbtqia { get; set; }
    public bool EBaixaRenda { get; set; }
    public string Titulo { get; set; } = null!;
    public string AreaDeAtuacao { get; set; } = null!;
    public string ResumoProfissional { get; set; } = null!;
    public string Linkedin { get; set; } = null!;
    public string Portfolio { get; set; } = null!;
    public string? Pdf { get; set; }
    public bool Clt { get; set; }
    public bool Pj { get; set; }
    public bool Temporario { get; set; }
    public bool Presencial { get; set; }
    public bool Remoto { get; set; }
    public bool Hibrido { get; set; }
}