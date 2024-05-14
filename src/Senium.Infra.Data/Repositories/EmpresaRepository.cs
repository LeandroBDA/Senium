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

    public void Cadastrar(Empresa empresa)
    {
        Context.Empresas.Add(empresa);
    }
}