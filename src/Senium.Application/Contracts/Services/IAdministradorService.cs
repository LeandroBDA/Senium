using Senium.Application.Dto.V1.Administrador;

namespace Senium.Application.Contracts.Services;

public interface IAdministradorService
{
    Task<AdministradorDto?> AdicionarAdm(AdicionarAdministradorDto dto);
    Task<AdministradorDto?> AtualizarAdm(int id, AtualizarAdministradorDto dto);
    Task RemoverAdm(int id);
    Task<List<AdministradorDto>> ObterTodosAdm();
}