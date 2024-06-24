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
using Senium.Core.Extensions;
using Senium.Core.Settings;
using Senium.Domain.Contracts.Repositories;
using Senium.Domain.Entities;
using Senium.Domain.Validations;

namespace Senium.Application.Services;

public class UsuarioAuthService : BaseService, IUsuarioAuthService
{
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly IPasswordHasher<Usuario> _passwordHasher;
    private readonly IJwtService _jwtService;
    private readonly JwtSettings _jwtSettings;
    private readonly IEmailService _emailService;
    
    public UsuarioAuthService(INotificator notificator, 
        IMapper mapper, 
        IPasswordHasher<Usuario> passwordHasher, 
        IUsuarioRepository usuarioRepository, 
        IOptions<JwtSettings> jwtSettings, 
        IJwtService jwtService, 
        IEmailService emailService) : base(notificator, mapper)
    {
        _passwordHasher = passwordHasher;
        _usuarioRepository = usuarioRepository;
        _jwtService = jwtService;
        _emailService = emailService;
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
    
    public async Task<bool> EsqueceuSenha(string email)
    {
        var usuario = await _usuarioRepository.ObterUsuarioPorEmail(email);
        if (usuario == null)
        {
            Notificator.HandleNotFoundResource();
            return false;
        }

        var pedidoResetSenha = await _usuarioRepository.ObterPedidoResetSenhaValido(usuario.Id);
        
        if (pedidoResetSenha != null)
        {
            Notificator.Handle("Já existe um pedido de recuperação de senha em andamento para este usuário.");
            return false;
        }
        
        usuario.TokenDeResetSenha = GerarTokenEsqueceuSenha();
        usuario.ExpiraResetToken = DateTime.Now.AddMinutes(5);
        _usuarioRepository.AtualizarUsuario(usuario);

        if (await _usuarioRepository.UnitOfWork.Commit())
        {
            await _emailService.EnviarEmailRecuperarSenhaUsuario(usuario);
            return true;
        }

        Notificator.Handle("Não foi possível solicitar a recuperação de senha");
        return false;
    }

    public async Task<bool> RedefinirSenha(ResetSenhaDto requestDto)
    {
        var usuario = await _usuarioRepository.ObterPorTokenDeResetSenha(requestDto.Token);
        if (usuario == null)
        {
            Notificator.HandleNotFoundResource();
            return false;
        }
        
        if (usuario.ExpiraResetToken.HasValue && usuario.ExpiraResetToken.Value < DateTime.Now)
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
        
        usuario.Senha = _passwordHasher.HashPassword(usuario, requestDto.Senha);

        usuario.TokenDeResetSenha = null;
        usuario.ExpiraResetToken = null;
        
        _usuarioRepository.AtualizarUsuario(usuario);
        if (await _usuarioRepository.UnitOfWork.Commit())
        {
            return true;
        }
        
        Notificator.Handle("Não foi possível alterar a senha.");
        return false;
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
    
    private bool ValidarSenha(string senha)
         {
             var senhaValidator = new SenhaUsuarioValidator();
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