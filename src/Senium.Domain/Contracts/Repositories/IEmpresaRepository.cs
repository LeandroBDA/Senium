using Senium.Domain.Entities;

namespace Senium.Domain.Contracts.Repositories;

public interface IEmpresaRepository : IRepository<Empresa>
{
    void Cadastrar(Empresa empresa);
}