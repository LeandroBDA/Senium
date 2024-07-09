using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Senium.Application.Contracts.Services;
using Senium.Application.Dto.V1.Curriculo;
using Senium.Application.Notifications;
using Swashbuckle.AspNetCore.Annotations;

namespace Senium.API.Controllers.V1.Curriculo;

[AllowAnonymous]
[Route("v{version:apiVersion}/[controller]")]
public class CurriculoController : BaseController
{
    private readonly ICurriculoService _curriculoService;
    
    public CurriculoController(INotificator notificator, ICurriculoService curriculoService) : base(notificator)
    {
        _curriculoService = curriculoService;
    }

    [HttpPost]
    [SwaggerOperation(Summary = "Adicionar curriculo do usuário", Tags = new[] { " Curriculo " })]
    [ProducesResponseType(typeof(CurriculoDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> Adicionar([FromForm] AdicionarCurriculoDto dto)
    {
        return CreatedResponse("", await _curriculoService.AdicionarCurriculo(dto));
    }
    
    [HttpPut("{id}")]
    [SwaggerOperation(Summary = "Atualizar curriculo por id do usuario", Tags = new[] { " Curriculo " })]
    [ProducesResponseType(typeof(CurriculoDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Atualizar(int id, [FromForm] AtualizarCurriculoDto dto)
    {
        var curriculo = await _curriculoService.AtualizarCurriculo(id, dto);
        return OkResponse(curriculo);
    }
    
    [HttpGet("id/{id:int}")]
    [SwaggerOperation(Summary = "Obter curriculo por id do usuário", Tags = new[] { " Curriculo " })]
    [ProducesResponseType(typeof(CurriculoDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ObterCurriculoPorId(int id)
    {
        var curriculo = await _curriculoService.ObterCurriculoPorUsuarioId(id);
        return OkResponse(curriculo);
    }
    
    [HttpGet]
    [SwaggerOperation(Summary = "Obter todos os curriculo", Tags = new[] { " Curriculo " })]
    [ProducesResponseType(typeof(CurriculoDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ObterTodosCurriculo()
    {
        var curriculo = await _curriculoService.ObterTodosCurriculo();
        return OkResponse(curriculo);
    }
    
}