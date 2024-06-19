using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using NetDevPack.Security.Jwt.Core.Interfaces;
using Senium.Application.Contracts.Services;
using Senium.Application.Dto.V1.Auth;
using Senium.Application.Notifications;
using Senium.Core.Enums;
using Senium.Core.Extensions;
using Senium.Core.Settings;
using Senium.Domain.Contracts.Repositories;
using Senium.Domain.Entities;

namespace Senium.Application.Services;

public class UsuarioAuthService : BaseService, IUsuarioAuthService
{
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly IPasswordHasher<Usuario> _passwordHasher;
    private readonly IJwtService _jwtService;
    private readonly JwtSettings _jwtSettings;
    
    public UsuarioAuthService(INotificator notificator, IMapper mapper, IPasswordHasher<Usuario> passwordHasher, IUsuarioRepository usuarioRepository, IOptions<JwtSettings> jwtSettings, IJwtService jwtService) : base(notificator, mapper)
    {
        _passwordHasher = passwordHasher;
        _usuarioRepository = usuarioRepository;
        _jwtService = jwtService;
        _jwtSettings = jwtSettings.Value;
    }

    public async Task<TokenDto?> Login(LoginUsuarioDto loginUsuario)
    {
        if (string.IsNullOrEmpty(loginUsuario.Senha))
        {
            Notificator.HandleNotFoundResource();
            return null;
        }

        var usuario = await _usuarioRepository.ObterUsuarioPorEmail(loginUsuario.Email);
        if (usuario == null )
        {
            Notificator.HandleNotFoundResource();
            return null;
        }
        
        if (_passwordHasher.VerifyHashedPassword(usuario, usuario.Senha, loginUsuario.Senha) !=
            PasswordVerificationResult.Failed)
            return new TokenDto
            {
                Token = await GerarToken(usuario)
            };
        
        Notificator.Handle("Não foi possível fazer o login");
        return null;
    }

    public Task<TokenDto?> Login(LoginAdministradorDto loginAdministrador)
    {
        throw new NotImplementedException();
    }

    public async Task<string> GerarToken(Usuario usuario)
    {
        var tokenHandler = new JwtSecurityTokenHandler();

        var claimsIdentity = new ClaimsIdentity();
        claimsIdentity.AddClaim(new Claim(ClaimTypes.NameIdentifier, usuario.Id.ToString()));
        claimsIdentity.AddClaim(new Claim("TipoUsuario", ETipoUsuario.Comum.ToDescriptionString()));
        claimsIdentity.AddClaim(new Claim(ClaimTypes.Name, usuario.Nome));
        claimsIdentity.AddClaim(new Claim(ClaimTypes.Email, usuario.Email));

        
        var key = await _jwtService.GetCurrentSigningCredentials();
        var token = tokenHandler.CreateToken(new SecurityTokenDescriptor
        {
            Issuer = _jwtSettings.Emissor,
            Subject = claimsIdentity,
            Expires = DateTime.UtcNow.AddHours(_jwtSettings.ExpiracaoHoras),
            SigningCredentials = key,
            Audience = _jwtSettings.ComumValidoEm
        });

        return tokenHandler.WriteToken(token);
    }
}