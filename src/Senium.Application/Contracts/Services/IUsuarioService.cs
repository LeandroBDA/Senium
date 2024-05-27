using Senium.Application.Dto.V1.Usuario;

namespace Senium.Application.Contracts.Services;

public interface IUsuarioService
{
    Task<UsuarioDto?> AdicionarUsuario(AdicionarUsuarioDto dto);
    Task<UsuarioDto?> ObterUsuarioPorId(int id);
    Task<UsuarioDto?> ObterUsuarioPorEmail(string email);
    Task<List<UsuarioDto>> ObterTodosUsuarios();
}