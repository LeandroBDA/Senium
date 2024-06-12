using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Senium.Application.Contracts.Services;
using Senium.Application.Dto.V1.Administrador;
using Senium.Application.Notifications;
using Swashbuckle.AspNetCore.Annotations;

namespace Senium.API.Controllers.V1.Administracao;

[AllowAnonymous]
public class AdministradoresController : MainController
{
    private readonly IAdministradorService _administradorService;
    
    public AdministradoresController(INotificator notificator,
        IAdministradorService administradorService) : base(notificator)
    {
        _administradorService = administradorService;
    }

    [HttpPost]
    [SwaggerOperation(Summary = "Adicionar administrador", Tags = new[] { " Administração " })]
    [ProducesResponseType(typeof(AdministradorDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> Adicionar([FromBody]AdicionarAdministradorDto dto)
    {
        return CreatedResponse("", await _administradorService.AdicionarAdm(dto));
    }

    [HttpPut("{id}")]
    [SwaggerOperation(Summary = "Atualizar administrador", Tags = new[] { " Administração " })]
    [ProducesResponseType(typeof(AdministradorDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Atualizar(int id, [FromBody] AtualizarAdministradorDto dto)
    {
        return OkResponse(await _administradorService.AtualizarAdm(id, dto));
    }
    
    [HttpGet]
    [SwaggerOperation(Summary = "Obter todos os administradores.", Tags = new [] { " Administração " })]
    [ProducesResponseType(typeof(AdministradorDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> Obtertodos()
    {
        return OkResponse(await _administradorService.ObterTodosAdm());
    }

    [HttpDelete("id")]
    [SwaggerOperation(Summary = "Remover administrador", Tags = new[] { " Administração " })]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Remover(int id)
    {
        await _administradorService.RemoverAdm(id);
        return NoContentResponse();
    }
}