using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Senium.Application.Contracts.Services;
using Senium.Application.Dto.V1.Empresa;
using Senium.Application.Notifications;
using Swashbuckle.AspNetCore.Annotations;

namespace Senium.API.Controllers.V1.Empresa;

[AllowAnonymous]
[Route("v{version:apiVersion}/[controller]")]
public class EmpresaController : MainController
{
    private readonly IEmpresaService _empresaService;
    
    public EmpresaController(INotificator notificator, IEmpresaService empresaService) : base(notificator)
    {
        _empresaService = empresaService;
    }

    [HttpPost]
    [SwaggerOperation(Summary = "Cadastro de Empresa", Tags = new[] { " Empresa - Empresas" })]
    [ProducesResponseType(typeof(EmpresaDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> Cadastrar([FromBody] AdicionarEmpresaDto dto)
    {
        return CreatedResponse("", await _empresaService.AdicionarEmpresa(dto));
    }
}