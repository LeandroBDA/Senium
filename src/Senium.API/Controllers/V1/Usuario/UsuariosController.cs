using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Senium.Application.Contracts.Services;
using Senium.Application.Dto.V1.Usuario;
using Senium.Application.Notifications;
using Swashbuckle.AspNetCore.Annotations;

namespace Senium.API.Controllers.V1.Usuario;

[AllowAnonymous]
[Route("v{version:apiVersion}/[controller]")]
public class UsuariosController : MainController
{
    private readonly IUsuarioService _usuarioService;
    
    public UsuariosController(INotificator notificator, IUsuarioService usuarioService) : base(notificator)
    {
        _usuarioService = usuarioService;
    }

    [HttpPost]
    [SwaggerOperation(Summary = "Cadastro de Usuario.", Tags = new[] { " Usuário - Usuários" })]
    [ProducesResponseType(typeof(UsuarioDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> Cadastrar([FromBody] AdicionarUsuarioDto dto)
    {
        return CreatedResponse("", await _usuarioService.Adicionar(dto));
    }
}