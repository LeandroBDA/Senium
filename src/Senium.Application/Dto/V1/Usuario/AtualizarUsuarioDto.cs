namespace Senium.Application.Dto.V1.Usuario;

public class AtualizarUsuarioDto
{
    public int Id { get; set; }
    public string Nome { get; set; } = null!;
    public string Email { get; set; } = null!;
    public DateTime DataDeNascimento { get; set; }
}