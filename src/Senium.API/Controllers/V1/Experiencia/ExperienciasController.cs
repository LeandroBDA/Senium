using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Senium.Application.Contracts.Services;
using Senium.Application.Dto.V1.Experiencia;
using Senium.Application.Notifications;
using Swashbuckle.AspNetCore.Annotations;

namespace Senium.API.Controllers.V1.Experiencia;

[Authorize]
[Route("v{version:apiVersion}/[controller]")]
public class ExperienciasController : MainController
{
    private readonly IExperienciaService _experienciaService;
    
    public ExperienciasController(INotificator notificator, IExperienciaService experienciaService) : base(notificator)
    {
        _experienciaService = experienciaService;
    }
    
    [HttpPost]
    [SwaggerOperation(Summary = "Adicionar experiência.", Tags = new[] { " Experiência - Experiências" })]
    [ProducesResponseType(typeof(ExperienciaDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> Adicionar([FromBody] AdicionarExperienciaDto dto)
    {
        return CreatedResponse("", await _experienciaService.AdicionarExperiencia(dto));
    }
    
    [HttpGet("{id}")]
    [SwaggerOperation(Summary = "Obter experiencia(s)", Tags = new[] { " Experiência - Experiências" })]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ObterPorId(int id)
    {
        return OkResponse(await _experienciaService.ObterExperiencia(id));
    }
}