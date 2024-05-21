using Senium.Application.Dto.V1.Experiencia;

namespace Senium.Application.Contracts.Services;

public interface IExperienciaService
{
    Task<ExperienciaDto?> AdicionarExperiencia(AdicionarExperienciaDto dto);
    Task<List<ExperienciaDto>?> ObterExperiencia(int curriculoId);
}