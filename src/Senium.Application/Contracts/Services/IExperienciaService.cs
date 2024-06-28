using Senium.Application.Dto.V1.Experiencia;

namespace Senium.Application.Contracts.Services;

public interface IExperienciaService
{
    Task<ExperienciaDto?> AdicionarExperiencia(AdicionarExperienciaDto dto);
    Task<ExperienciaDto?> AtualizarExperiencia(int id, AtualizarExperienciaDto dto);
    Task DeletarExperiencia(int id);
    Task<List<ExperienciaDto>?> ObterExperienciaPorUsuarioId(int usuarioId);
}