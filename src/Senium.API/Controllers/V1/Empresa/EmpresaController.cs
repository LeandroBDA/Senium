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
    [SwaggerOperation(Summary = "Cadastro de empresa", Tags = new[] { " Empresa " })]
    [ProducesResponseType(typeof(EmpresaDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> Cadastrar([FromBody] AdicionarEmpresaDto dto)
    {
        return CreatedResponse("", await _empresaService.AdicionarEmpresa(dto));
    }
    
    [HttpGet]
    [SwaggerOperation(Summary = "Obter Todas as Empresas ", Tags = new[] { " Empresa " })]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> BuscarEmpresas()
    {
        return OkResponse(await _empresaService.ObterTodasEmpresas());
    }
    
    [HttpPut("{Id}")]
    [SwaggerOperation(Summary = "Atualizar empresa cadastrada", Tags = new[] { " Empresa " })]
    [ProducesResponseType(typeof(EmpresaDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Atualizar(int id, [FromForm] AtualizarEmpresaDto dto)
    {
        var empresa = await _empresaService.AtualizarEmpresa(id, dto);
        return OkResponse(empresa);
    }
}