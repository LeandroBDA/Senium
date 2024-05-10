using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Senium.Application.Contracts.Services;
using Senium.Application.Dto.V1.Curriculo;
using Senium.Application.Notifications;
using Swashbuckle.AspNetCore.Annotations;

namespace Senium.API.Controllers.V1.Curriculo;

[Authorize]
[Route("v{version:apiVersion}/[controller]")]
public class CurriculoController : BaseController
{
    private readonly ICurriculoService _curriculoService;
    
    public CurriculoController(INotificator notificator, ICurriculoService curriculoService) : base(notificator)
    {
        _curriculoService = curriculoService;
    }

    [HttpPost]
    [SwaggerOperation(Summary = "Formulário de Dados Pessoais.", Tags = new[] { " Curriculo - Curriculos" })]
    [ProducesResponseType(typeof(CurriculoDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> Adicionar([FromBody] AdicionarCurriculoDto curriculoDto)
    {
        return CreatedResponse("", await _curriculoService.Adicionar(curriculoDto));
    }
    
    [HttpPut]
    [SwaggerOperation(Summary = "Alterar Formulário de Dados Pessoais.", Tags = new[] { " Curriculo - Curriculos" })]
    [ProducesResponseType(typeof(CurriculoDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Editar(int id,CurriculoDto curriculoDto)
    {
        var curriculo = await _curriculoService.Editar(id, curriculoDto);
        return OkResponse(curriculo);
    }
    
    [HttpGet]
    [SwaggerOperation(Summary = "Buscar Formulário de Dados Pessoais.", Tags = new[] { " Curriculo - Curriculos" })]
    [ProducesResponseType(typeof(CurriculoDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> BuscarPorId(int id)
    {
        var curriculo = await _curriculoService.ObterPorId(id);
        return OkResponse(curriculo);
    }
    
}