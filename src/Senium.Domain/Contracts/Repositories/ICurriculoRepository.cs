using Senium.Domain.Entities;

namespace Senium.Domain.Contracts.Repositories;

public interface ICurriculoRepository : IRepository<Curriculo>
{
    Task<Curriculo?> ObterPorId(int id);
    void Adicionar(Curriculo curriculo);
    void Editar(Curriculo curriculo);
    void Remover(Curriculo curriculo);
}