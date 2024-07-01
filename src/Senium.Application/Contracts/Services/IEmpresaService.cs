using Senium.Application.Dto.V1.Empresa;

namespace Senium.Application.Contracts.Services;

public interface IEmpresaService
{
    Task<EmpresaDto?> AdicionarEmpresa(AdicionarEmpresaDto dto);
    Task<List<EmpresaDto>?> ObterTodasEmpresas();
}