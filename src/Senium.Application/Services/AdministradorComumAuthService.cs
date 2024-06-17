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

public class AdministradorComumAuthService : BaseService, IAuthAdmService 
{
    private readonly IAdministradorRepository _administradorRepository;
    private readonly IPasswordHasher<Administrador> _passwordHasher;
    private readonly IJwtService _jwtService;
    private readonly JwtSettings _jwtSettings;
    
    public AdministradorComumAuthService(INotificator notificator, IMapper mapper, IPasswordHasher<Administrador> passwordHasher, IAdministradorRepository administradorRepository, IOptions<JwtSettings> jwtSettings, IJwtService jwtService) : base(notificator, mapper)
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

        var administradorComum = await _administradorRepository.ObterAdmPorEmail(loginAdministrador.Email);
        
       
        if (administradorComum == null )
        {
            Notificator.HandleNotFoundResource();
            return null;
        }
        
        var senhaVerificada = _passwordHasher.VerifyHashedPassword(administradorComum, administradorComum.Senha, loginAdministrador.Senha);
        if (senhaVerificada == PasswordVerificationResult.Success)
        {
         
            return new TokenDto
            {
                Token = await GerarToken(administradorComum)
            };
        }
        else
        {
            Notificator.Handle("Não foi possível fazer o login para esse tipo de Administrador");
            return null;
        }
    }
    public async Task<string> GerarToken(Administrador AdministradorComum)
    {
        var tokenHandler = new JwtSecurityTokenHandler();

        var claimsIdentity = new ClaimsIdentity();
        claimsIdentity.AddClaim(new Claim(ClaimTypes.NameIdentifier, AdministradorComum.Id.ToString()));
        claimsIdentity.AddClaim(new Claim("TipoAdministrador", ETipoUsuario.AdministradorComum.ToDescriptionString()));
        claimsIdentity.AddClaim(new Claim(ClaimTypes.Name, AdministradorComum.Nome));
        claimsIdentity.AddClaim(new Claim(ClaimTypes.Email, AdministradorComum.Email));

        claimsIdentity.AddClaim(new Claim("TipoAdministrador", ETipoUsuario.AdministradorComum.ToString()));
       
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