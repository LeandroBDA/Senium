using Senium.Domain.Entities;

namespace Senium.Domain.Contracts.Repositories;

public interface IUsuarioRepository : IRepository<Usuario>
{
    void CadastrarUsuario(Usuario usuario);
    void AtualizarUsuario(Usuario usuario);
    Task<Usuario?> ObterUsuarioPorId(int id);
    Task<Usuario?> ObterUsuarioPorEmail(string email);
    Task<List<Usuario>> ObterTodosUsuarios();
}