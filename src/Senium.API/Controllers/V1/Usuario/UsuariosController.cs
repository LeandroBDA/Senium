﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Senium.Application.Contracts.Services;
using Senium.Application.Dto.V1.Usuario;
using Senium.Application.Notifications;
using Swashbuckle.AspNetCore.Annotations;

namespace Senium.API.Controllers.V1.Usuario;

[Route("v{version:apiVersion}/[controller]")]
public class UsuariosController : MainController
{
    private readonly IUsuarioService _usuarioService;
    
    public UsuariosController(INotificator notificator, IUsuarioService usuarioService) : base(notificator)
    {
        _usuarioService = usuarioService;
    }
    
    [AllowAnonymous]
    [HttpPost]
    [SwaggerOperation(Summary = "Cadastro de usuario.", Tags = new[] { " Usuário " })]
    [ProducesResponseType(typeof(UsuarioDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> Cadastrar([FromBody] AdicionarUsuarioDto dto)
    {
        return CreatedResponse("", await _usuarioService.AdicionarUsuario(dto));
    }

    [Authorize]
    [HttpPut("{id}")]
    [SwaggerOperation(Summary = "Atualizar usuário", Tags = new[] { " Usuário " })]
    [ProducesResponseType(typeof(UsuarioDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Atualizar( int id, [FromBody] AtualizarUsuarioDto dto)
    {
        return OkResponse(await _usuarioService.AtualizarUsuario(id, dto));
    }
    
    [Authorize]
    [HttpGet("{id}")]
    [SwaggerOperation(Summary = "Obter um usuário", Tags = new[] { " Usuário " })]
    [ProducesResponseType(typeof(UsuarioDto),StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ObterPorId(int id)
    {
        return OkResponse(await _usuarioService.ObterUsuarioPorId(id));
    }
}