using Senium.Application.Dto.V1.Curriculo;

namespace Senium.Application.Contracts.Services;

public interface ICurriculoService
{
    Task<CurriculoDto?> ObterCurriculoPorUsuarioId(int id);
    Task<CurriculoDto?> AdicionarCurriculo(AdicionarCurriculoDto curriculodto);
    Task<CurriculoDto?> AtualizarCurriculo(int usuarioId, AtualizarCurriculoDto curriculoDto);
    Task<List<CurriculoDto>?> ObterTodosCurriculo();
}