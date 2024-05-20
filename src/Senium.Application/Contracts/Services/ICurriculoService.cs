using Senium.Application.Dto.V1.Curriculo;

namespace Senium.Application.Contracts.Services;

public interface ICurriculoService
{
    Task<CurriculoDto?> ObterPorId(int id);
    Task<CurriculoDto?> Adicionar(AdicionarCurriculoDto curriculodto);
    Task<CurriculoDto?> Editar(int id, CurriculoDto curriculoDto);
    Task<CurriculoDto?> Remove(int id);
}