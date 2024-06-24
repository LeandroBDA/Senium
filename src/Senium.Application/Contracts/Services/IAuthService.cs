using Senium.Application.Dto.V1.Auth;

namespace Senium.Application.Contracts.Services;

public interface IUsuarioAuthService
{
    Task<TokenDto?> Login(LoginUsuarioDto loginUsuario);
    Task<bool> EsqueceuSenha(string email);
    Task<bool> RedefinirSenha(ResetSenhaDto requestDto);

}

public interface IAdministradorAuthService
{
    Task<TokenDto?> Login(LoginAdministradorDto loginUsuario);
    Task<bool> EsqueceuSenha(string email);
    Task<bool> RedefinirSenha(ResetSenhaDto requestDto);
}