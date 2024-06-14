using Senium.Application.Dto.V1.Administrador;
using Senium.Application.Dto.V1.Auth;

namespace Senium.Application.Contracts.Services;

public interface IAuthAdmService
{
    Task<TokenDto?> Login(LoginAdministradorDto loginAdministrador);
}