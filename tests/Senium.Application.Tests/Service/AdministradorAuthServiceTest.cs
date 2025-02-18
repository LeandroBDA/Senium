using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Moq;
using NetDevPack.Security.Jwt.Core.Interfaces;
using Senium.Application.Dto.V1.Auth;
using Senium.Application.Email;
using Senium.Application.Services;
using Senium.Application.Tests.Fixtures;
using Senium.Core.Enums;
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
    private readonly Mock<IEmailService> _emailServiceMock = new();

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
            _jwtServiceMock.Object,
            _emailServiceMock.Object
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

        Assert.True(jsonToken?.Claims.Any(c =>
            c.Type == "TipoUsuario" && c.Value == ETipoUsuario.AdministradorComum.ToString()));
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


    [Fact]
    public async Task Login_Usuario_TudoVazio()
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

    #region Esqueceu Senha

    [Fact]
    public async Task EsqueceuSenha_AdministradorNaoEncontrado_ReturnsFalseAndHandlesNotFound()
    {
        // Arrange
        SetupMocks(administradorExistente: false);

        // Act
        var result = await _administradorAuthService.EsqueceuSenha("naoexiste@teste.com");

        // Assert
        Assert.False(result);
        NotificatorMock.Verify(n => n.HandleNotFoundResource(), Times.Once);
        _administradorRepositoryMock.Verify(r => r.ObterAdmPorEmail(It.IsAny<string>()), Times.Once);
        _administradorRepositoryMock.Verify(r => r.AtualizarAdm(It.IsAny<Administrador>()), Times.Never);
        _administradorRepositoryMock.Verify(r => r.UnitOfWork.Commit(), Times.Never);
        _emailServiceMock.Verify(e => e.EnviarEmailRecuperarSenhaAdministrador(It.IsAny<Administrador>()), Times.Never);
    }

    [Fact]
    public async Task EsqueceuSenha_PedidoResetValido_EnviaEmailERetornaTrue()
    {
        // Arrange
        SetupMocks(administradorExistente: true);

        var administrador = new Administrador
        {
            Id = 1,
            Email = "administrador@teste.com",
            ExpiraResetToken = DateTime.Now.AddMinutes(1)
        };

        _administradorRepositoryMock
            .Setup(r => r.ObterAdmPorEmail(administrador.Email))
            .ReturnsAsync(administrador);

        _administradorRepositoryMock
            .Setup(r => r.UnitOfWork.Commit())
            .ReturnsAsync(true);

        // Act
        var result = await _administradorAuthService.EsqueceuSenha(administrador.Email);

        // Assert
        Assert.True(result);
        NotificatorMock.Verify(n => n.HandleNotFoundResource(), Times.Never);
        NotificatorMock.Verify(n => n.Handle(It.IsAny<string>()), Times.Never);
        _administradorRepositoryMock.Verify(r => r.AtualizarAdm(administrador), Times.Once);
        _administradorRepositoryMock.Verify(r => r.UnitOfWork.Commit(), Times.Once);
        _emailServiceMock.Verify(e => e.EnviarEmailRecuperarSenhaAdministrador(administrador), Times.Once);
    }

    [Fact]
    public async Task EsqueceuSenha_NovoPedido_ResetTokenGeradoEEmailEnviado()
    {
        // Arrange
        SetupMocks(administradorExistente: true);

        var administrador = new Administrador
        {
            Id = 1,
            Email = "administrador@teste.com"
        };

        _administradorRepositoryMock
            .Setup(r => r.ObterAdmPorEmail(administrador.Email))
            .ReturnsAsync(administrador);

        _administradorRepositoryMock
            .Setup(r => r.ObterPorTokenDeResetSenha(administrador.TokenDeResetSenha ?? string.Empty))
            .ReturnsAsync((Administrador)null);

        _administradorRepositoryMock
            .Setup(r => r.UnitOfWork.Commit())
            .ReturnsAsync(true);

        // Act
        var result = await _administradorAuthService.EsqueceuSenha(administrador.Email);

        // Assert
        Assert.True(result);
        NotificatorMock.Verify(n => n.HandleNotFoundResource(), Times.Never);
        NotificatorMock.Verify(n => n.Handle(It.IsAny<string>()), Times.Never);
        _administradorRepositoryMock.Verify(r => r.AtualizarAdm(administrador), Times.Once);
        _administradorRepositoryMock.Verify(r => r.UnitOfWork.Commit(), Times.Once);
        _emailServiceMock.Verify(e => e.EnviarEmailRecuperarSenhaAdministrador(administrador), Times.Once);
    }

    [Fact]
    public async Task EsqueceuSenha_PedidoResetSenhaExpirado_ReturnsTrueAndSendsEmail()
    {
        // Arrange
        SetupMocks(administradorExistente: true);

        var administrador = new Administrador
        {
            Id = 1,
            Email = "administrador@teste.com",
            ExpiraResetToken = DateTime.Now.AddMinutes(-1)
        };

        _administradorRepositoryMock
            .Setup(r => r.ObterAdmPorEmail(administrador.Email))
            .ReturnsAsync(administrador);

        _administradorRepositoryMock
            .Setup(r => r.UnitOfWork.Commit())
            .ReturnsAsync(true);

        // Act
        var result = await _administradorAuthService.EsqueceuSenha(administrador.Email);

        // Assert
        Assert.True(result);
        NotificatorMock.Verify(n => n.HandleNotFoundResource(), Times.Never);
        NotificatorMock.Verify(
            n => n.Handle("Já existe um pedido de recuperação de senha em andamento para este administrador."),
            Times.Never);
        NotificatorMock.Verify(n => n.Handle("Não foi possível solicitar a recuperação de senha"), Times.Never);
        _administradorRepositoryMock.Verify(r => r.AtualizarAdm(administrador), Times.Once);
        _administradorRepositoryMock.Verify(r => r.UnitOfWork.Commit(), Times.Once);
        _emailServiceMock.Verify(e => e.EnviarEmailRecuperarSenhaAdministrador(administrador), Times.Once);
    }

    [Fact]
    public async Task EsqueceuSenha_FalhaAoSolicitarRecuperacao_ReturnsFalseAndHandlesFailure()
    {
        // Arrange
        SetupMocks(administradorExistente: true);

        var administrador = new Administrador
        {
            Id = 1,
            Email = "administrador@teste.com",
            ExpiraResetToken = null
        };

        _administradorRepositoryMock
            .Setup(r => r.ObterAdmPorEmail(administrador.Email))
            .ReturnsAsync(administrador);

        _administradorRepositoryMock
            .Setup(r => r.UnitOfWork.Commit())
            .ReturnsAsync(false);

        // Act
        var result = await _administradorAuthService.EsqueceuSenha(administrador.Email);

        // Assert
        Assert.False(result);
        NotificatorMock.Verify(n => n.HandleNotFoundResource(), Times.Never);
        NotificatorMock.Verify(
            n => n.Handle("Já existe um pedido de recuperação de senha em andamento para este administrador."),
            Times.Never);
        NotificatorMock.Verify(n => n.Handle("Não foi possível solicitar a recuperação de senha"), Times.Once);
        _administradorRepositoryMock.Verify(r => r.AtualizarAdm(administrador), Times.Once);
        _administradorRepositoryMock.Verify(r => r.UnitOfWork.Commit(), Times.Once);
        _emailServiceMock.Verify(e => e.EnviarEmailRecuperarSenhaAdministrador(administrador), Times.Never);
    }

    [Fact]
    public async Task EsqueceuSenha_PedidoResetSenhaValido_ReturnsFalseAndHandlesExistingRequest()
    {
        // Arrange
        SetupMocks(administradorExistente: true);

        var administrador = new Administrador
        {
            Id = 1,
            Email = "administrador@teste.com",
            ExpiraResetToken = DateTime.Now.AddMinutes(10)
        };

        _administradorRepositoryMock
            .Setup(r => r.ObterPedidoResetSenhaValido(administrador.Id))
            .ReturnsAsync(administrador);

        // Act
        var result = await _administradorAuthService.EsqueceuSenha(administrador.Email);

        // Assert
        Assert.False(result);
        NotificatorMock.Verify(n => n.HandleNotFoundResource(), Times.Never);
        NotificatorMock.Verify(
            n => n.Handle("Já existe um pedido de recuperação de senha em andamento para este administrador."),
            Times.Once);
        NotificatorMock.Verify(n => n.Handle("Não foi possível solicitar a recuperação de senha"), Times.Never);
        _administradorRepositoryMock.Verify(r => r.AtualizarAdm(administrador), Times.Never);
        _administradorRepositoryMock.Verify(r => r.UnitOfWork.Commit(), Times.Never);
        _emailServiceMock.Verify(e => e.EnviarEmailRecuperarSenhaAdministrador(administrador), Times.Never);
    }

    #endregion

    #region Redefinir Senha

    [Fact]
    public async Task ResetSenha_TokenExpirado_ReturnsFalseAndHandlesExpiration()
    {
        // Arrange
        SetupMocks(administradorExistente: true);

        var resetDto = new ResetSenhaDto
        {
            Token = "tokeninvalido"
        };

        var administrador = new Administrador
        {
            Id = 1,
            ExpiraResetToken = DateTime.Now.AddMinutes(-1)
        };

        _administradorRepositoryMock
            .Setup(r => r.ObterPorTokenDeResetSenha(resetDto.Token))
            .ReturnsAsync(administrador);

        // Act
        var result = await _administradorAuthService.RedefinirSenha(resetDto);

        // Assert
        Assert.False(result);
        NotificatorMock.Verify(n => n.Handle("O token de redefinição de senha expirou."), Times.Once);
        _administradorRepositoryMock.Verify(r => r.AtualizarAdm(It.IsAny<Administrador>()), Times.Never);
        _administradorRepositoryMock.Verify(r => r.UnitOfWork.Commit(), Times.Never);
    }


    [Fact]
    public async Task ResetSenha_AdministradorNaoEncontrado_ReturnsFalseAndHandlesNotFound()
    {
        // Arrange
        SetupMocks(administradorExistente: false);

        var resetDto = new ResetSenhaDto
        {
            Token = "tokeninvalido"
        };

        // Act
        var result = await _administradorAuthService.RedefinirSenha(resetDto);

        // Assert
        Assert.False(result);
        NotificatorMock.Verify(n => n.HandleNotFoundResource(), Times.Once);
        _administradorRepositoryMock.Verify(r => r.AtualizarAdm(It.IsAny<Administrador>()), Times.Never);
        _administradorRepositoryMock.Verify(r => r.UnitOfWork.Commit(), Times.Never);
    }

    [Fact]
    public async Task ResetSenha_PedidoValido_RedefineSenhaERetornaTrue()
    {
        // Arrange
        SetupMocks(administradorExistente: true);

        var resetDto = new ResetSenhaDto
        {
            Token = "tokenvalido",
            Senha = "Senha123!",
            ConfirmarSenha = "Senha123!"
        };

        var administrador = new Administrador
        {
            Id = 1,
            ExpiraResetToken = DateTime.Now.AddMinutes(1)
        };

        _administradorRepositoryMock
            .Setup(r => r.ObterPorTokenDeResetSenha(resetDto.Token))
            .ReturnsAsync(administrador);

        _administradorRepositoryMock
            .Setup(r => r.UnitOfWork.Commit())
            .ReturnsAsync(true);

        // Act
        var result = await _administradorAuthService.RedefinirSenha(resetDto);

        // Assert
        Assert.True(result);
        NotificatorMock.Verify(n => n.Handle(It.IsAny<string>()), Times.Never);
        _administradorRepositoryMock.Verify(r => r.AtualizarAdm(It.IsAny<Administrador>()), Times.Once);
        _administradorRepositoryMock.Verify(r => r.UnitOfWork.Commit(), Times.Once);
    }

    [Fact]
    public async Task ResetSenha_SenhasNaoCoincidentes_ReturnsFalseAndHandlesMismatch()
    {
        // Arrange
        SetupMocks(administradorExistente: true);

        var resetDto = new ResetSenhaDto
        {
            Token = "tokenvalido",
            Senha = "Senha123!",
            ConfirmarSenha = "Senha456!"
        };

        var administrador = new Administrador
        {
            Id = 1
        };

        _administradorRepositoryMock
            .Setup(r => r.ObterPorTokenDeResetSenha(resetDto.Token))
            .ReturnsAsync(administrador);

        // Act
        var result = await _administradorAuthService.RedefinirSenha(resetDto);

        // Assert
        Assert.False(result);
        NotificatorMock.Verify(n => n.Handle("As senhas informadas não coincidem."), Times.Once);
        _administradorRepositoryMock.Verify(r => r.AtualizarAdm(It.IsAny<Administrador>()),
            Times.Never);
        _administradorRepositoryMock.Verify(r => r.UnitOfWork.Commit(),
            Times.Never);
    }

    [Fact]
    public async Task ResetSenha_SenhaInvalida_ReturnsFalseAndHandlesInvalidPassword()
    {
        // Arrange
        SetupMocks(administradorExistente: true);

        var resetDto = new ResetSenhaDto
        {
            Token = "tokenvalido",
            Senha = "123"
        };

        var administrador = new Administrador
        {
            Id = 1
        };

        _administradorRepositoryMock
            .Setup(r => r.ObterPorTokenDeResetSenha(resetDto.Token))
            .ReturnsAsync(administrador);

        // Act
        var result = await _administradorAuthService.RedefinirSenha(resetDto);

        // Assert
        Assert.False(result);
        NotificatorMock.Verify(n => n.Handle("As senhas informadas não coincidem."),
            Times.Once);
        _administradorRepositoryMock.Verify(r => r.AtualizarAdm(It.IsAny<Administrador>()),
            Times.Never);
        _administradorRepositoryMock.Verify(r => r.UnitOfWork.Commit(),
            Times.Never);
    }

    [Fact]
    public async Task ResetSenha_SenhaInvalida_ReturnsFalseAndHandlesNotification()
    {
        // Arrange
        var requestDto = new ResetSenhaDto
        {
            Token = "token_valido",
            Senha = "novaSenha123",
            ConfirmarSenha = "novaSenha123"
        };

        var administrador = new Administrador
        {
            Id = 1,
            TokenDeResetSenha = requestDto.Token,
            ExpiraResetToken = DateTime.Now.AddMinutes(5)
        };

        _administradorRepositoryMock.Setup(r => r.ObterPorTokenDeResetSenha(requestDto.Token))
            .ReturnsAsync(administrador);

        _administradorRepositoryMock.Setup(r => r.UnitOfWork.Commit())
            .ReturnsAsync(true);

        // Act
        var result = await _administradorAuthService.RedefinirSenha(requestDto);

        // Assert
        Assert.False(result);
        NotificatorMock.Verify(n => n.Handle(It.IsAny<string>()), Times.AtLeastOnce);
    }

    [Fact]
    public async Task ResetSenha_UnableToChangePassword_ReturnsFalseAndHandlesNotification()
    {
        // Arrange
        var requestDto = new ResetSenhaDto
        {
            Token = "token_valido",
            Senha = "novaSenha123!",
            ConfirmarSenha = "novaSenha123!"
        };

        var administrador = new Administrador
        {
            Id = 1,
            TokenDeResetSenha = requestDto.Token,
            ExpiraResetToken = DateTime.Now.AddHours(5)
        };

        _administradorRepositoryMock.Setup(r => r.ObterPorTokenDeResetSenha(requestDto.Token))
            .ReturnsAsync(administrador);

        _administradorRepositoryMock.Setup(r => r.UnitOfWork.Commit())
            .ReturnsAsync(false);

        // Act
        var result = await _administradorAuthService.RedefinirSenha(requestDto);

        // Assert
        Assert.False(result);
        NotificatorMock.Verify(n => n.Handle("Não foi possível alterar a senha."), Times.Once);
    }

    #endregion


    #region SetupMocks

    private void SetupMocks(bool administradorExistente, bool senhaCorreta = true)
    {
        var adm = administradorExistente
            ? new Administrador
            {
                Id = 1,
                Nome = "NomeDoAdministrador",
                Email = "administrador@teste.com",
                Senha = "Senha123!",
                TokenDeResetSenha = "tokenvalido",
                ExpiraResetToken = DateTime.Now.AddMinutes(5)
            }
            : null;

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

        _administradorRepositoryMock
            .Setup(c => c.ObterPorTokenDeResetSenha(It.IsAny<string>()))
            .ReturnsAsync(adm);

        _administradorRepositoryMock
            .Setup(c => c.AtualizarAdm(It.IsAny<Administrador>()))
            .Verifiable();

        _administradorRepositoryMock
            .Setup(c => c.UnitOfWork.Commit())
            .ReturnsAsync(true);
    }

    #endregion
}