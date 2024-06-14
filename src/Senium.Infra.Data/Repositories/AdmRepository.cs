using Microsoft.EntityFrameworkCore;
using Senium.Domain.Contracts.Repositories;
using Senium.Domain.Entities;
using Senium.Infra.Data.Abstractions;
using Senium.Infra.Data.Context;

namespace Senium.Infra.Data.Repositories;

public class AdmRepository : Repository<Administrador>, IAdmRepository
{
    public AdmRepository(BaseApplicationDbContext context) : base(context)
    { }

    public async Task<Administrador?> VerificarAdmPorId(int id)
    {
        return await Context.Administradores.AsNoTrackingWithIdentityResolution().FirstOrDefaultAsync(x => x.Id == id);
    }
    
    public async Task<Administrador> VerificarAdmPorEmail(string email)
    {
        return await Context.Administradores.AsNoTrackingWithIdentityResolution().FirstOrDefaultAsync(x => x.Email == email);
    }
}