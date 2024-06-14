namespace Senium.Application.Dto.V1.Adm;

public class AdmDto
{
    public int Id { get; set; } 
    public string Email { get; set; } = null!;
    public string Senha { get; set; } = null!;
    public string HashedPassword { get; set; } = null!; // Criptografa a senha.
}