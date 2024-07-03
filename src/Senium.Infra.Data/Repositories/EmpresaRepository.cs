using Microsoft.EntityFrameworkCore;
using Senium.Domain.Contracts.Repositories;
using Senium.Domain.Entities;
using Senium.Infra.Data.Abstractions;
using Senium.Infra.Data.Context;

namespace Senium.Infra.Data.Repositories;

public class EmpresaRepository : Repository<Empresa>, IEmpresaRepository
{
    public EmpresaRepository(BaseApplicationDbContext context) : base(context)
    {
    }

    public void CadastrarEmpresa(Empresa empresa)
    {
        Context.Empresas.Add(empresa);
    }
    
    public async Task<List<Empresa>> ObterTodasEmpresas()
    {
        return await Context.Empresas.AsNoTrackingWithIdentityResolution().ToListAsync();
    }

    public void AtualizarEmpresa(Empresa empresa)
    {
        Context.Empresas.Update(empresa);
    }
    public async Task<Empresa?> ObterEmpresaPorId(int id)
    {
        return await Context.Empresas.AsNoTrackingWithIdentityResolution()
            .FirstOrDefaultAsync(c => Equals(c.Id, id));
    }
}