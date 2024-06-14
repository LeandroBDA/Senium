using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Senium.Application.Contracts.Services;
using Senium.Application.Dto.V1.Administrador;
using Senium.Application.Dto.V1.Auth;
using Senium.Application.Notifications;
using Swashbuckle.AspNetCore.Annotations;

namespace Senium.API.Controllers.V1.Usuario;

[AllowAnonymous]
[Route("v{version:apiVersion}/[controller]")]
public class AdministradorAuthController : BaseController
{
    private readonly IAuthAdmService _administradorAuthService;
    
    public AdministradorAuthController(INotificator notificator, IAuthAdmService administradorAuthService) : base(notificator)
    {
        _administradorAuthService = administradorAuthService;
    }
    
    [HttpPost]
    [SwaggerOperation(Summary = "Login de addministrador.", Tags = new [] { "Adm - Auth" })]
    [ProducesResponseType(typeof(TokenDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
   
    public async Task<IActionResult> Login([FromBody] LoginAdministradorDto dto)
    {
        var token = await _administradorAuthService.Login(dto);
        return token != null ? OkResponse(token) : Unauthorized(new[] { "Email e/ou senha incorretos" });
    }
}