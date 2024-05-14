using Senium.Application.Dto.V1.Experiencia;

namespace Senium.Application.Contracts.Services;

public interface IExperienciaService
{
    Task<ExperienciaDto?> Adicionar(AdicionarExperienciaDto dto);
}