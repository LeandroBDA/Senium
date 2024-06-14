using Microsoft.EntityFrameworkCore;
using Senium.Domain.Contracts.Repositories;
using Senium.Domain.Entities;
using Senium.Infra.Data.Abstractions;
using Senium.Infra.Data.Context;

namespace Senium.Infra.Data.Repositories;

public class AdministradorRepository : Repository<Administrador>, IAdministradorRepository
{
    public AdministradorRepository(BaseApplicationDbContext context) : base(context)
    {
    }

    public void AdicionarAdm(Administrador administrador)
    {
        Context.Administradores.Add(administrador);
    }

    public void AtualizarAdm(Administrador administrador)
    {
        Context.Administradores.Update(administrador);
    }

    public void RemoverAdm(Administrador administrador)
    {
        Context.Administradores.Remove(administrador);
    }

    public async Task<Administrador?> ObterAdmPorId(int id)
    {
        return await Context.Administradores.AsNoTrackingWithIdentityResolution()
            .FirstOrDefaultAsync(c => c.Id == id);
    }

    public async Task<Administrador?> ObterAdmPorEmail(string email)
    {
        return await Context.Administradores.AsNoTrackingWithIdentityResolution()
            .FirstOrDefaultAsync(c => c.Email == email);
    }

    public async Task<List<Administrador>> ObterTodosAdm()
    {
        return await Context.Administradores.AsNoTrackingWithIdentityResolution().ToListAsync();
    }
}