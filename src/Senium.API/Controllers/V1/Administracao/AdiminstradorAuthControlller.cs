using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Senium.Application.Contracts.Services;
using Senium.Application.Dto.V1.Auth;
using Senium.Application.Notifications;
using Swashbuckle.AspNetCore.Annotations;

namespace Senium.API.Controllers.V1.Administracao;

[AllowAnonymous]
[Route("v{version:apiVersion}/[controller]")]
public class AdministradorAuthController : BaseController
{
    private readonly IAdministradorAuthService _administradorAuthService;
    
    public AdministradorAuthController(INotificator notificator, IAdministradorAuthService administradorAuthService) : base(notificator)
    {
        _administradorAuthService = administradorAuthService;
    }
    
    [HttpPost]
    [SwaggerOperation(Summary = "Login de administrador.", Tags = new [] { "Administração - Auth" })]
    [ProducesResponseType(typeof(TokenDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
   
    public async Task<IActionResult> Login([FromBody] LoginAdministradorDto dto)
    {
        var token = await _administradorAuthService.Login(dto);
        return token != null ? OkResponse(token) : Unauthorized(new[] { "Email e/ou senha incorretos" });
    }
    
    [HttpPost("esqueceu-senha")]
    [SwaggerOperation(Summary = "Esqueceu a Senha", Tags = new[] { "Administração - Auth" })]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> EsqueceuSenha([FromQuery] string email)
    {
        await _administradorAuthService.EsqueceuSenha(email);
        return OkResponse("Um e-mail foi enviado com instruções para redefinir sua senha.");
    }
    
    [HttpPost("resetar-senha")]
    [SwaggerOperation(Summary = "Redefinir Senha", Tags = new[] { "Administração - Auth" })]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> RedefinirSenha([FromForm] ResetSenhaDto requestDto)
    {
        await _administradorAuthService.RedefinirSenha(requestDto);
        return OkResponse("Senha alterada com sucesso.");
    }
}