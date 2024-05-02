using Senium.Application.Dto.V1.Auth;

namespace Senium.Application.Contracts.Services;

public interface IUsuarioAuthService
{
    Task<TokenDto?> Login(LoginUsuarioDto loginUsuario);
}