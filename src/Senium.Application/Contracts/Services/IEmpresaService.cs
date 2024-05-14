using Senium.Application.Dto.V1.Empresa;

namespace Senium.Application.Contracts.Services;

public interface IEmpresaService
{
    Task<EmpresaDto?> Adicionar(AdicionarEmpresaDto dto);
}