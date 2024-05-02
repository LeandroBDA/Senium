using Senium.Application.Dto.V1.Usuario;

namespace Senium.Application.Contracts.Services;

public interface IUsuarioService
{
    Task<UsuarioDto?> Adicionar(AdicionarUsuarioDto dto);
    Task<UsuarioDto?> ObterPorId(int id);
    Task<UsuarioDto?> ObterPorEmail(string email);
    Task<List<UsuarioDto>> ObterTodos();
}