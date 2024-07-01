using Senium.Domain.Entities;

namespace Senium.Domain.Contracts.Repositories;

public interface IEmpresaRepository : IRepository<Empresa>
{
    void CadastrarEmpresa(Empresa empresa);
    Task<List<Empresa>> ObterTodasEmpresas();
}