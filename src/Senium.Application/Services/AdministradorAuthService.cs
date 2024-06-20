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

public class AdministradorAuthService : BaseService, IAuthAdmService 
{
    private readonly IAdministradorRepository _administradorRepository;
    private readonly IPasswordHasher<Administrador> _passwordHasher;
    private readonly IJwtService _jwtService;
    private readonly JwtSettings _jwtSettings;
    
    public AdministradorAuthService(INotificator notificator, IMapper mapper, IPasswordHasher<Administrador> passwordHasher, IAdministradorRepository administradorRepository, IOptions<JwtSettings> jwtSettings, IJwtService jwtService) : base(notificator, mapper)
    {
        _passwordHasher = passwordHasher;
        _administradorRepository = administradorRepository;
        _jwtService = jwtService;
        _jwtSettings = jwtSettings.Value;
    }
    public async Task<TokenDto?> Login(LoginAdministradorDto loginAdministrador)
    {
        if (string.IsNullOrEmpty(loginAdministrador.Senha))
        {
            Notificator.HandleNotFoundResource();
            return null;
        }
        
        var administrador = await _administradorRepository.ObterAdmPorEmail(loginAdministrador.Email);
        if (administrador == null )
        {
            Notificator.HandleNotFoundResource();
            return null;
        }
        
        if (_passwordHasher.VerifyHashedPassword(administrador, administrador.Senha, loginAdministrador.Senha) !=
            PasswordVerificationResult.Failed)
            return new TokenDto
            {
                Token = await GerarToken(administrador)
            };
        
        Notificator.Handle("Não foi possível fazer o login");
        return null;
    }
  
    public async Task<string> GerarToken(Administrador administrador)
    {
        var tokenHandler = new JwtSecurityTokenHandler();

        var claimsIdentity = new ClaimsIdentity();
        claimsIdentity.AddClaim(new Claim(ClaimTypes.NameIdentifier, administrador.Id.ToString()));
        claimsIdentity.AddClaim(new Claim(ClaimTypes.Name, administrador.Nome));
        claimsIdentity.AddClaim(new Claim(ClaimTypes.Email, administrador.Email));
       
        if (administrador.Id == 1)
        {
            claimsIdentity.AddClaim(new Claim("TipoAdministrador", ETipoUsuario.AdministradorGeral.ToString()));
        }
      
        else
        {
            claimsIdentity.AddClaim(new Claim("TipoAdministrador", ETipoUsuario.AdministradorComum.ToString()));
        }
        
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