using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using NetDevPack.Security.Jwt.Core.Interfaces;
using Senium.Application.Contracts.Services;
using Senium.Application.Dto.V1.Auth;
using Senium.Application.Email;
using Senium.Application.Notifications;
using Senium.Core.Enums;
using Senium.Core.Settings;
using Senium.Domain.Contracts.Repositories;
using Senium.Domain.Entities;
using Senium.Domain.Validations;

namespace Senium.Application.Services;

public class AdministradorAuthService : BaseService, IAdministradorAuthService
{
    private readonly IAdministradorRepository _administradorRepository;
    private readonly IPasswordHasher<Administrador> _passwordHasher;
    private readonly IJwtService _jwtService;
    private readonly JwtSettings _jwtSettings;
    private readonly IEmailService _emailService;
    
    public AdministradorAuthService(INotificator notificator,
        IMapper mapper, 
        IPasswordHasher<Administrador> passwordHasher, 
        IAdministradorRepository administradorRepository, 
        IOptions<JwtSettings> jwtSettings, 
        IJwtService jwtService, 
        IEmailService emailService) : base(notificator, mapper)
    {
        _passwordHasher = passwordHasher;
        _administradorRepository = administradorRepository;
        _jwtService = jwtService;
        _emailService = emailService;
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
    
    public async Task<bool> EsqueceuSenha(string email)
    {
        var administrador = await _administradorRepository.ObterAdmPorEmail(email);
        if (administrador == null)
        {
            Notificator.HandleNotFoundResource();
            return false;
        }

        var pedidoResetSenha = await _administradorRepository.ObterPedidoResetSenhaValido(administrador.Id);
        
        if (pedidoResetSenha != null)
        {
            Notificator.Handle("Já existe um pedido de recuperação de senha em andamento para este administrador.");
            return false;
        }
        
        administrador.TokenDeResetSenha = GerarTokenEsqueceuSenha();
        administrador.ExpiraResetToken = DateTime.Now.AddMinutes(5);
        _administradorRepository.AtualizarAdm(administrador);

        if (await _administradorRepository.UnitOfWork.Commit())
        {
            await _emailService.EnviarEmailRecuperarSenhaAdministrador(administrador);
            return true;
        }

        Notificator.Handle("Não foi possível solicitar a recuperação de senha");
        return false;
    }

    public async Task<bool> RedefinirSenha(ResetSenhaDto requestDto)
    {
        var administrador = await _administradorRepository.ObterPorTokenDeResetSenha(requestDto.Token);
        if (administrador == null)
        {
            Notificator.HandleNotFoundResource();
            return false;
        }
        
        if (administrador.ExpiraResetToken.HasValue && administrador.ExpiraResetToken.Value < DateTime.Now)
        {
            Notificator.Handle("O token de redefinição de senha expirou.");
            return false;
        }
        
        if (!string.IsNullOrEmpty(requestDto.Senha) && requestDto.Senha != requestDto.ConfirmarSenha)
        {
            Notificator.Handle("As senhas informadas não coincidem.");
            return false;
        }

        if (!ValidarSenha(requestDto.Senha))
        {
            return false;
        }
        
        administrador.Senha = _passwordHasher.HashPassword(administrador, requestDto.Senha);

        administrador.TokenDeResetSenha = null;
        administrador.ExpiraResetToken = null;
        
        _administradorRepository.AtualizarAdm(administrador);
        if (await _administradorRepository.UnitOfWork.Commit())
        {
            return true;
        }
        
        Notificator.Handle("Não foi possível alterar a senha.");
        return false;
    }
  
    public async Task<string> GerarToken(Administrador administrador)
    {
        var tokenHandler = new JwtSecurityTokenHandler();

        var claimsIdentity = new ClaimsIdentity();
        claimsIdentity.AddClaim(new Claim(ClaimTypes.NameIdentifier, administrador.Id.ToString()));
        claimsIdentity.AddClaim(new Claim(ClaimTypes.Name, administrador.Nome));
        claimsIdentity.AddClaim(new Claim(ClaimTypes.Email, administrador.Email));
        
        if(administrador.Id == 1)
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
            Audience = _jwtSettings.GestaoValidoEm
        });

        return tokenHandler.WriteToken(token);
    }
    
    private bool ValidarSenha(string senha)
    {
        var senhaValidator = new SenhaAdministradorValidator();
        var result = senhaValidator.Validate(senha);

        if (!result.IsValid)
        {
            foreach (var error in result.Errors)
            {
                Notificator.Handle(error.ErrorMessage);
            }
            return false;
        }

        return true;
    }
    
    private string GerarTokenEsqueceuSenha()
    {
        return Convert.ToHexString(RandomNumberGenerator.GetBytes(8));
    }
}