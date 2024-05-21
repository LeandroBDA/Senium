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
    [SwaggerOperation(Summary = "Adicionar Curriculo do Usuário", Tags = new[] { " Curriculo - Curriculos" })]
    [ProducesResponseType(typeof(CurriculoDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> Adicionar([FromBody] AdicionarCurriculoDto dto)
    {
        return CreatedResponse("", await _curriculoService.AdicionarCurriculo(dto));
    }
    
    [HttpPut]
    [SwaggerOperation(Summary = "Atualizar Curriculo do Usuário", Tags = new[] { " Curriculo - Curriculos" })]
    [ProducesResponseType(typeof(CurriculoDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Atualizar(int id,CurriculoDto dto)
    {
        var curriculo = await _curriculoService.AtualizarCurriculo(id, dto);
        return OkResponse(curriculo);
    }
    
    [HttpGet]
    [SwaggerOperation(Summary = "Obter Curriculo do Usuário por Id", Tags = new[] { " Curriculo - Curriculos" })]
    [ProducesResponseType(typeof(CurriculoDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> ObterCurriculoPorId(int id)
    {
        var curriculo = await _curriculoService.ObterCurriculoPorId(id);
        return OkResponse(curriculo);
    }
    
}