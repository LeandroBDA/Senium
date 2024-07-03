namespace Senium.Application.Dto.V1.Empresa;

public class AtualizarEmpresaDto
{
    public int Id { get; set; } 
    public string Nome { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string Telefone { get; set; } = null!;
    public string NomeDaEmpresa { get; set; } = null!;
}