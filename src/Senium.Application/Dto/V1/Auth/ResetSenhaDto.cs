namespace Senium.Application.Dto.V1.Auth;

public class ResetSenhaDto
{
    public string Token { get; set; } = null!;
    public string Senha { get; set; } = null!;
    public string ConfirmarSenha { get; set; } = null!;
}