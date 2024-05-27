using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Moq;
using NetDevPack.Security.Jwt.Core.Interfaces;
using Senium.Application.Dto.V1.Auth;
using Senium.Application.Services;
using Senium.Application.Tests.Fixtures;
using Senium.Core.Enums;
using Senium.Core.Extensions;
using Senium.Core.Settings;
using Senium.Domain.Contracts.Repositories;
using Senium.Domain.Entities;

namespace Senium.Application.Tests.Service;

public class UsuarioAuthServiceTest : BaseServiceTest, IClassFixture<ServicesFixture>
{
    private readonly UsuarioAuthService _usuarioAuthService;

    private readonly Mock<IUsuarioRepository> _usuarioRepositoryMock = new();
    private readonly Mock<IPasswordHasher<Usuario>> _passwordHasherMock = new();
    private readonly Mock<IJwtService> _jwtServiceMock = new();
    
    public UsuarioAuthServiceTest(ServicesFixture fixture)
    {
        IOptions<JwtSettings> jwtSettings = Options.Create(new JwtSettings
        {
            Emissor = "http://localhost:8000",
            ExpiracaoHoras = 1
        });

        _usuarioAuthService = new UsuarioAuthService(
            NotificatorMock.Object, 
            fixture.Mapper,
            _passwordHasherMock.Object, 
            _usuarioRepositoryMock.Object, 
            jwtSettings, 
            _jwtServiceMock.Object
        );
    }

    #region Login

        [Fact]
        public async Task Login_UsuarioNaoEncontrado_HandleNotFoundResource()
        {
            //Arrange
            SetupMocks(usuarioExistente: false);
    
            //Act
            var result = await _usuarioAuthService.Login(
            new LoginUsuarioDto
            {
                Email = "usuarionaoexiste@teste.com",
                Senha = "Senha123"
            });
    
            //Assert
            Assert.Null(result);
            NotificatorMock.Verify(c => c.HandleNotFoundResource(), Times.Once);
        }
        

        [Fact]
        public async Task Login_SenhaIncorreta_Handle()
        {
            //Arrege
            SetupMocks(usuarioExistente: true, senhaCorreta: false);
    
            //Act
            var result = await _usuarioAuthService.Login(
            new LoginUsuarioDto 
            { 
                Email = "usuario@teste.com", 
                Senha = "SenhaIncorreta" 
            });
    
            //Assert
            Assert.Null(result);
            NotificatorMock.Verify(c => c.Handle("Não foi possível fazer o login"), Times.Once);
        }

        [Fact]
        public async Task Login_SucessoGeracaoToken_LoginRetornaToken()
        {
            //Arrege
            SetupMocks(usuarioExistente: true, senhaCorreta: true);
    
            //Act
            var result = await _usuarioAuthService.Login(
            new LoginUsuarioDto 
            { 
                Email = "usuario@teste.com", 
                Senha = "Senha123!" 
            });
    
            //Assert
            Assert.NotNull(result);
            NotificatorMock.Verify(c => c.HandleNotFoundResource(), Times.Never);
            NotificatorMock.Verify(c => c.Handle("Não foi possível fazer o login"), Times.Never);
        }

        [Fact]
        public async Task GerarToken_Usuario_TokenComTipoComum()
        {
            // Arrange
            var usuario = new Usuario
            {
                Id = 2,
                Nome = "NomeDoUsuario",
                Email = "usuario@teste.com",
                Senha = "Senha123!",
            };

            SetupMocks(usuarioExistente: true, senhaCorreta: true);

            // Act
            var token = await _usuarioAuthService.GerarToken(usuario);

            // Assert
            var handler = new JwtSecurityTokenHandler();
            var jsonToken = handler.ReadToken(token) as JwtSecurityToken;

            Assert.NotNull(jsonToken);
            
            Assert.True(jsonToken?.Claims.Any(c => c.Type == "TipoUsuario" && c.Value == ETipoUsuario.Comum.ToDescriptionString()));
        }

        [Fact]
        public async Task Login_SenhaVazia_Handle()
        {
            //Arrange
            SetupMocks(usuarioExistente: true, senhaCorreta: false);

            //Act
            var result = await _usuarioAuthService.Login(
            new LoginUsuarioDto 
            { 
                 Email = "usuario@teste.com", 
                Senha = "" 
            });

            //Assert
            Assert.Null(result);
            NotificatorMock.Verify(c => c.HandleNotFoundResource(), Times.Once);
        }
        
        [Fact]
        public async Task Login_Usuario_EmailVazio_SenhaCorreta()
        {
            // Arrange
            var loginDto = new LoginUsuarioDto
            {
                Email = "", 
                Senha = "Senha123!" 
            };

            SetupMocks(usuarioExistente: true, senhaCorreta: true);

            // Act
            var resultadoLogin = await _usuarioAuthService.Login(loginDto);

            // Assert
            Assert.NotNull(resultadoLogin);
        }
        

        [Fact]
        public async Task Login_Usuario_TudoVazio()
        {
            // Arrange
            var usuario = new Usuario
            {
                Id = 2,
                Nome = "",
                Email = "",
                Senha = "",
            };

            SetupMocks(usuarioExistente: true, senhaCorreta: true);

            // Act
            var resultadoLogin = await _usuarioAuthService.Login(new LoginUsuarioDto
            {
                Email = usuario.Email,
                Senha = usuario.Senha
            });

            // Assert
            Assert.Null(resultadoLogin);
        }

        


    #endregion
    
    
    #region SetupMocks
    private void SetupMocks(bool usuarioExistente, bool senhaCorreta = true)
    {
        var usuario = usuarioExistente ? new Usuario
        {
            Id = 1,
            Nome = "NomeDoUsuario",
            Email = "usuario@teste.com", 
            Senha = "Senha123!",
        } : null;

        _usuarioRepositoryMock
            .Setup(c => c.ObterUsuarioPorEmail(It.IsAny<string>()))
            .ReturnsAsync(usuario);

        _passwordHasherMock
            .Setup(c => c.VerifyHashedPassword(It.IsAny<Usuario>(), It.IsAny<string>(), It.IsAny<string>()))
            .Returns(senhaCorreta ? PasswordVerificationResult.Success : PasswordVerificationResult.Failed);

        _jwtServiceMock
            .Setup(c => c.GetCurrentSigningCredentials())
            .ReturnsAsync(new SigningCredentials(
                new SymmetricSecurityKey(Encoding.UTF8.GetBytes("YXNqZGhzYWpkaHFpZGhhc2R1YWh3MTMyaTNqb3BhZGph")),
                SecurityAlgorithms.HmacSha256
            ));
    }
    #endregion

}