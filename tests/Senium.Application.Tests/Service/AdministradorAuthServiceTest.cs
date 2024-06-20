using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
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

public class AdministradorAuthServiceTest : BaseServiceTest, IClassFixture<ServicesFixture>
{
    private readonly AdministradorAuthService _administradorAuthService;

    private readonly Mock<IAdministradorRepository> _administradorRepositoryMock = new();
    private readonly Mock<IPasswordHasher<Administrador>> _passwordHasherMock = new();
    private readonly Mock<IJwtService> _jwtServiceMock = new();
    
     public AdministradorAuthServiceTest(ServicesFixture fixture)
    {
        IOptions<JwtSettings> jwtSettings = Options.Create(new JwtSettings
        {
            Emissor = "http://localhost:8000",
            ExpiracaoHoras = 1
        });

        _administradorAuthService = new AdministradorAuthService(
            NotificatorMock.Object, 
            fixture.Mapper,
            _passwordHasherMock.Object, 
            _administradorRepositoryMock.Object, 
            jwtSettings, 
            _jwtServiceMock.Object
        );
    }

    #region Login

        [Fact]
        public async Task Login_AdmNaoEncontrado_HandleNotFoundResource()
        {
            //Arrange
            SetupMocks(administradorExistente: false);
    
            //Act
            var result = await _administradorAuthService.Login(
            new LoginAdministradorDto
            {
                Email = "administradornaoexiste@teste.com",
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
            SetupMocks(administradorExistente: true, senhaCorreta: false);
    
            //Act
            var result = await _administradorAuthService.Login(
            new LoginAdministradorDto 
            { 
                Email = "administrador@teste.com", 
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
            SetupMocks(administradorExistente: true, senhaCorreta: true);
    
            //Act
            var result = await _administradorAuthService.Login(
                new LoginAdministradorDto
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
        public async Task GerarToken_Admistrador_TokenComTipoComum()
        {
            // Arrange
            var administrador = new Administrador
            {
                Id = 2,
                Nome = "NomeDoUsuario",
                Email = "administrador@teste.com",
                Senha = "Senha123!",
            };

            SetupMocks(administradorExistente: true, senhaCorreta: true); 
            
            // Act
            var token = await _administradorAuthService.GerarToken(administrador);

            // Assert
            var handler = new JwtSecurityTokenHandler();
            var jsonToken = handler.ReadToken(token) as JwtSecurityToken;

            Assert.NotNull(jsonToken);
            
            Assert.True(jsonToken?.Claims.Any(c => c.Type == "TipoAdministrador" && c.Value == ETipoUsuario.AdministradorComum.ToString()));
        }   

        [Fact]
        public async Task Login_SenhaVazia_Handle()
        {
            //Arrange
            SetupMocks(administradorExistente: true, senhaCorreta: false);

            //Act
            var result = await _administradorAuthService.Login(
            new LoginAdministradorDto 
            { 
                 Email = "administrador@teste.com", 
                Senha = "" 
            });

            //Assert
            Assert.Null(result);
            NotificatorMock.Verify(c => c.HandleNotFoundResource(), Times.Once);
        }
        
        [Fact]
        public async Task Login_Administrador_EmailVazio_SenhaCorreta()
        {
            // Arrange
            var loginDto = new LoginAdministradorDto
            {
                Email = "", 
                Senha = "Senha123!" 
            };

            SetupMocks(administradorExistente: true, senhaCorreta: true);

            // Act
            var resultadoLogin = await _administradorAuthService.Login(loginDto);

            // Assert
            Assert.NotNull(resultadoLogin);
        }
        

        [Fact] public async Task Login_Usuario_TudoVazio()
        {
            // Arrange
            var administrador = new Administrador
            {
                Id = 2,
                Nome = "",
                Email = "",
                Senha = "",
            };

            SetupMocks(administradorExistente: true, senhaCorreta: true);

            // Act
            var resultadoLogin = await _administradorAuthService.Login(new LoginAdministradorDto
            {
                Email = administrador.Email,
                Senha = administrador.Senha
            });

            // Assert
            Assert.Null(resultadoLogin);
        }

    #endregion
    
    #region SetupMocks
    private void SetupMocks(bool administradorExistente, bool senhaCorreta = true)
    {
        var adm = administradorExistente ? new Administrador
        {
            Id = 2,
            Nome = "NomeDoAdministrador",
            Email = "administrador@teste.com", 
            Senha = "Senha123!",
        } : null;

        _administradorRepositoryMock
            .Setup(c => c.ObterAdmPorEmail(It.IsAny<string>()))
            .ReturnsAsync(adm);

        _passwordHasherMock
            .Setup(c => c.VerifyHashedPassword(It.IsAny<Administrador>(), It.IsAny<string>(), It.IsAny<string>()))
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