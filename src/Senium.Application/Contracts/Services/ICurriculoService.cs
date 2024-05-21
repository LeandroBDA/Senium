using Senium.Application.Dto.V1.Curriculo;

namespace Senium.Application.Contracts.Services;

public interface ICurriculoService
{
    Task<CurriculoDto?> ObterCurriculoPorId(int id);
    Task<CurriculoDto?> AdicionarCurriculo(AdicionarCurriculoDto curriculodto);
    Task<CurriculoDto?> AtualizarCurriculo(int id, CurriculoDto curriculoDto);
    Task<CurriculoDto?> RemoveCurriculo(int id);
}