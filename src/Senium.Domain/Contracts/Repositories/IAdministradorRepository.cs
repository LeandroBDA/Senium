using Senium.Domain.Entities;

namespace Senium.Domain.Contracts.Repositories;

public interface IAdministradorRepository : IRepository<Administrador>
{
    void AdicionarAdm(Administrador administrador);
    void AtualizarAdm(Administrador administrador);
    void RemoverAdm(Administrador administrador);
    Task<Administrador?> ObterAdmPorId(int id);
    Task<List<Administrador>> ObterTodosAdm();
}