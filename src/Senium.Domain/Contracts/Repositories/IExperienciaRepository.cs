using Senium.Domain.Entities;

namespace Senium.Domain.Contracts.Repositories;

public interface IExperienciaRepository : IRepository<Experiencia>
{
    void Adicionar(Experiencia experiencia);
    void Atualizar(Experiencia experiencia);
    void Remover(Experiencia experiencia);
    Task<Experiencia?> ObterExperienciaPorId(int id, int curriculoId);
}