using Senium.Domain.Contracts.Repositories;
using Senium.Domain.Entities;

namespace Senium.Domain.Contracts.Interfaces;

public interface ICurriculoRepository : IRepository<Curriculo>
{
    void Adicionar(Curriculo curriculo);
}