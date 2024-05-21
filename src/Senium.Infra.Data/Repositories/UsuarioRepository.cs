using Microsoft.EntityFrameworkCore;
using Senium.Domain.Contracts.Repositories;
using Senium.Domain.Entities;
using Senium.Infra.Data.Abstractions;
using Senium.Infra.Data.Context;

namespace Senium.Infra.Data.Repositories;

public class UsuarioRepository : Repository<Usuario>, IUsuarioRepository
{
    public UsuarioRepository(BaseApplicationDbContext context) : base(context)
    {
    }

    public void CadastrarUsuario(Usuario usuario)
    {
        Context.Usuarios.Add(usuario);
    }

    public void AtualizarUsuario(Usuario usuario)
    {
        Context.Usuarios.Update(usuario);
    }

    public async Task<Usuario?> ObterUsuarioPorId(int id)
    {
        return await Context.Usuarios.AsNoTrackingWithIdentityResolution().FirstOrDefaultAsync(u => u.Id == id);
    }

    public async Task<Usuario?> ObterUsuarioPorEmail(string email)
    {
        return await Context.Usuarios.AsNoTrackingWithIdentityResolution().FirstOrDefaultAsync(u => u.Email == email);
    }

    public async Task<List<Usuario>> ObterTodosUsuarios()
    {
        return await Context.Usuarios.AsNoTrackingWithIdentityResolution().ToListAsync();
    }
}