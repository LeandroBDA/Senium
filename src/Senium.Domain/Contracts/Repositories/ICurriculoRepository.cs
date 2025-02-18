using Senium.Domain.Entities;

namespace Senium.Domain.Contracts.Repositories;

public interface ICurriculoRepository : IRepository<Curriculo>
{
    Task<Curriculo?> ObterCurriculoPorUsuarioId(int id);
    void AdicionarCurriculo(Curriculo curriculo);
    void AtualizarCurriculo(Curriculo curriculo);
    void RemoverCurriculo(Curriculo curriculo);
    Task<List<Curriculo>> ObterTodosCurriculo();
}