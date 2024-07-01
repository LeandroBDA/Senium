using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Senium.Application.Contracts.Services;
using Senium.Application.Dto.V1.Auth;
using Senium.Application.Notifications;
using Swashbuckle.AspNetCore.Annotations;

namespace Senium.API.Controllers.V1.Usuario;

[AllowAnonymous]
[Route("v{version:apiVersion}/[controller]")]
public class UsuariosAuthController : BaseController
{
    private readonly IUsuarioAuthService _usuarioAuthService;
    
    public UsuariosAuthController(INotificator notificator, IUsuarioAuthService usuarioAuthService) : base(notificator)
    {
        _usuarioAuthService = usuarioAuthService;
    }
    
    [HttpPost]
    [SwaggerOperation(Summary = "Login de usuário.", Tags = new [] { "Usuário - Auth" })]
    [ProducesResponseType(typeof(TokenDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Login([FromBody] LoginUsuarioDto dto)
    {
        var token = await _usuarioAuthService.Login(dto);
        return token != null ? OkResponse(token) : Unauthorized(new[] { "Email e/ou senha incorretos" });
    }
    
    [HttpPost("esqueceu-senha")]
    [SwaggerOperation(Summary = "Esqueceu a senha", Tags = new[] { "Usuário - Auth" })]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> EsqueceuSenha([FromQuery] string email)
    {
        await _usuarioAuthService.EsqueceuSenha(email);
        return OkResponse("Um e-mail foi enviado com instruções para redefinir sua senha.");
    }
    
    [HttpPost("resetar-senha")]
    [SwaggerOperation(Summary = "Redefinir senha", Tags = new[] { "Usuário - Auth" })]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> RedefinirSenha([FromForm] ResetSenhaDto requestDto)
    {
        await _usuarioAuthService.RedefinirSenha(requestDto);
        return OkResponse("Senha alterada com sucesso.");
    }
}