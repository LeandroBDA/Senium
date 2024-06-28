using Senium.Domain.Entities;

namespace Senium.Domain.Contracts.Repositories;

public interface IExperienciaRepository : IRepository<Experiencia>
{
    void AdicionarExperiencia(Experiencia experiencia);
    void AtualizarExperiencia(Experiencia experiencia);
    void RemoverExperiencia(Experiencia experiencia);
    Task<Experiencia?> ObterExperienciaPorId(int id);
    Task<List<Experiencia>> ObterExperienciaDoUsuario(int usuarioId);
}