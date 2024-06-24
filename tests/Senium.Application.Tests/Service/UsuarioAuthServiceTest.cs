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
    private readonly Mock<IEmailService> _emailServiceMock = new();

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
            _jwtServiceMock.Object,
            _emailServiceMock.Object
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

        Assert.True(jsonToken?.Claims.Any(c =>
            c.Type == "TipoUsuario" && c.Value == ETipoUsuario.Comum.ToDescriptionString()));
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

    #region Esqueceu Senha

    [Fact]
    public async Task EsqueceuSenha_UsuarioNaoEncontrado_ReturnsFalseAndHandlesNotFound()
    {
        // Arrange
        SetupMocks(usuarioExistente: false);

        // Act
        var result = await _usuarioAuthService.EsqueceuSenha("naoexiste@teste.com");

        // Assert
        Assert.False(result);
        NotificatorMock.Verify(n => n.HandleNotFoundResource(), Times.Once);
        _usuarioRepositoryMock.Verify(r => r.ObterUsuarioPorEmail(It.IsAny<string>()),
            Times.Once);
        _usuarioRepositoryMock.Verify(r => r.AtualizarUsuario(It.IsAny<Usuario>()),
            Times.Never);
        _usuarioRepositoryMock.Verify(r => r.UnitOfWork.Commit(), Times.Never);
        _emailServiceMock.Verify(e => e.EnviarEmailRecuperarSenhaUsuario(It.IsAny<Usuario>()),
            Times.Never);
    }

    [Fact]
    public async Task EsqueceuSenha_PedidoResetValido_EnviaEmailERetornaTrue()
    {
        // Arrange
        SetupMocks(usuarioExistente: true);

        var usuario = new Usuario
        {
            Id = 1,
            Email = "usuario@teste.com",
            ExpiraResetToken = DateTime.Now.AddMinutes(1)
        };

        _usuarioRepositoryMock
            .Setup(r => r.ObterUsuarioPorEmail(usuario.Email))
            .ReturnsAsync(usuario);

        _usuarioRepositoryMock
            .Setup(r => r.ObterPorTokenDeResetSenha(usuario.TokenDeResetSenha ?? string.Empty))
            .ReturnsAsync(usuario);

        _usuarioRepositoryMock
            .Setup(r => r.UnitOfWork.Commit())
            .ReturnsAsync(true);

        // Act
        var result = await _usuarioAuthService.EsqueceuSenha(usuario.Email);

        // Assert
        Assert.True(result);
        NotificatorMock.Verify(n => n.HandleNotFoundResource(), Times.Never);
        NotificatorMock.Verify(n => n.Handle(It.IsAny<string>()), Times.Never);
        _usuarioRepositoryMock.Verify(r => r.AtualizarUsuario(usuario),
            Times.Once);
        _usuarioRepositoryMock.Verify(r => r.UnitOfWork.Commit(), Times.Once);
        _emailServiceMock.Verify(e => e.EnviarEmailRecuperarSenhaUsuario(usuario),
            Times.Once);
    }

    [Fact]
    public async Task EsqueceuSenha_NovoPedido_ResetTokenGeradoEEmailEnviado()
    {
        // Arrange
        SetupMocks(usuarioExistente: true);

        var usuario = new Usuario
        {
            Id = 1,
            Email = "usuario@teste.com"
        };

        _usuarioRepositoryMock
            .Setup(r => r.ObterUsuarioPorEmail(usuario.Email))
            .ReturnsAsync(usuario);

        _usuarioRepositoryMock
            .Setup(r => r.ObterPorTokenDeResetSenha(usuario.TokenDeResetSenha ?? string.Empty))
            .ReturnsAsync((Usuario)null);

        _usuarioRepositoryMock
            .Setup(r => r.UnitOfWork.Commit())
            .ReturnsAsync(true);

        // Act
        var result = await _usuarioAuthService.EsqueceuSenha(usuario.Email);

        // Assert
        Assert.True(result);
        NotificatorMock.Verify(n => n.HandleNotFoundResource(), Times.Never);
        NotificatorMock.Verify(n => n.Handle(It.IsAny<string>()), Times.Never);
        _usuarioRepositoryMock.Verify(r => r.AtualizarUsuario(usuario),
            Times.Once);
        _usuarioRepositoryMock.Verify(r => r.UnitOfWork.Commit(), Times.Once);
        _emailServiceMock.Verify(e => e.EnviarEmailRecuperarSenhaUsuario(usuario),
            Times.Once);
    }

    [Fact]
    public async Task EsqueceuSenha_PedidoResetSenhaExpirado_ReturnsTrueAndSendsEmail()
    {
        // Arrange
        SetupMocks(usuarioExistente: true);

        var usuario = new Usuario
        {
            Id = 1,
            Email = "usuario@teste.com",
            ExpiraResetToken = DateTime.Now.AddMinutes(-1)
        };

        _usuarioRepositoryMock
            .Setup(r => r.ObterUsuarioPorEmail(usuario.Email))
            .ReturnsAsync(usuario);

        _usuarioRepositoryMock
            .Setup(r => r.UnitOfWork.Commit())
            .ReturnsAsync(true);

        // Act
        var result = await _usuarioAuthService.EsqueceuSenha(usuario.Email);

        // Assert
        Assert.True(result);
        NotificatorMock.Verify(n => n.HandleNotFoundResource(),
            Times.Never);
        NotificatorMock.Verify(
            n => n.Handle("Já existe um pedido de recuperação de senha em andamento para este usuário."),
            Times.Never);
        NotificatorMock.Verify(n => n.Handle("Não foi possível solicitar a recuperação de senha"),
            Times.Never);
        _usuarioRepositoryMock.Verify(r => r.AtualizarUsuario(usuario),
            Times.Once);
        _usuarioRepositoryMock.Verify(r => r.UnitOfWork.Commit(),
            Times.Once);
        _emailServiceMock.Verify(e => e.EnviarEmailRecuperarSenhaUsuario(usuario),
            Times.Once);
    }

    [Fact]
    public async Task EsqueceuSenha_FalhaAoSolicitarRecuperacao_ReturnsFalseAndHandlesFailure()
    {
        // Arrange
        SetupMocks(usuarioExistente: true);

        var usuario = new Usuario
        {
            Id = 1,
            Email = "usuario@teste.com",
            ExpiraResetToken = null
        };

        _usuarioRepositoryMock
            .Setup(r => r.ObterUsuarioPorEmail(usuario.Email))
            .ReturnsAsync(usuario);

        _usuarioRepositoryMock
            .Setup(r => r.UnitOfWork.Commit())
            .ReturnsAsync(false);

        // Act
        var result = await _usuarioAuthService.EsqueceuSenha(usuario.Email);

        // Assert
        Assert.False(result);
        NotificatorMock.Verify(n => n.HandleNotFoundResource(),
            Times.Never);
        NotificatorMock.Verify(
            n => n.Handle("Já existe um pedido de recuperação de senha em andamento para este usuário."),
            Times.Never);
        NotificatorMock.Verify(n => n.Handle("Não foi possível solicitar a recuperação de senha"),
            Times.Once);
        _usuarioRepositoryMock.Verify(r => r.AtualizarUsuario(usuario),
            Times.Once);
        _usuarioRepositoryMock.Verify(r => r.UnitOfWork.Commit(),
            Times.Once);
        _emailServiceMock.Verify(e => e.EnviarEmailRecuperarSenhaUsuario(usuario),
            Times.Never);
    }


    [Fact]
    public async Task EsqueceuSenha_PedidoResetSenhaValido_ReturnsFalseAndHandlesExistingRequest()
    {
        // Arrange
        SetupMocks(usuarioExistente: true);

        var usuario = new Usuario
        {
            Id = 1,
            Email = "usuario@teste.com",
            ExpiraResetToken = DateTime.Now.AddMinutes(10)
        };

        _usuarioRepositoryMock
            .Setup(r => r.ObterPedidoResetSenhaValido(usuario.Id))
            .ReturnsAsync(usuario);

        // Act
        var result = await _usuarioAuthService.EsqueceuSenha(usuario.Email);

        // Assert
        Assert.False(result);
        NotificatorMock.Verify(n => n.HandleNotFoundResource(), Times.Never);
        NotificatorMock.Verify(
            n => n.Handle("Já existe um pedido de recuperação de senha em andamento para este usuário."), Times.Once);
        NotificatorMock.Verify(n => n.Handle("Não foi possível solicitar a recuperação de senha"), Times.Never);
        _usuarioRepositoryMock.Verify(r => r.AtualizarUsuario(usuario), Times.Never);
        _usuarioRepositoryMock.Verify(r => r.UnitOfWork.Commit(), Times.Never);
        _emailServiceMock.Verify(e => e.EnviarEmailRecuperarSenhaUsuario(usuario), Times.Never);
    }

    #endregion

    #region Redefinir Senha

    [Fact]
    public async Task ResetSenha_TokenExpirado_ReturnsFalseAndHandlesExpiration()
    {
        // Arrange
        SetupMocks(usuarioExistente: true);

        var resetDto = new ResetSenhaDto
        {
            Token = "tokeninvalido"
        };

        var usuario = new Usuario
        {
            Id = 1,
            ExpiraResetToken = DateTime.Now.AddMinutes(-1)
        };

        _usuarioRepositoryMock
            .Setup(r => r.ObterPorTokenDeResetSenha(resetDto.Token))
            .ReturnsAsync(usuario);

        // Act
        var result = await _usuarioAuthService.RedefinirSenha(resetDto);

        // Assert
        Assert.False(result);
        NotificatorMock.Verify(n => n.Handle("O token de redefinição de senha expirou."), Times.Once);
        _usuarioRepositoryMock.Verify(r => r.AtualizarUsuario(It.IsAny<Usuario>()),
            Times.Never);
        _usuarioRepositoryMock.Verify(r => r.UnitOfWork.Commit(), Times.Never);
    }

    [Fact]
    public async Task ResetSenha_SenhasNaoCoincidentes_ReturnsFalseAndHandlesMismatch()
    {
        // Arrange
        SetupMocks(usuarioExistente: true);

        var resetDto = new ResetSenhaDto
        {
            Token = "tokenvalido",
            Senha = "Senha123!",
            ConfirmarSenha = "Senha456!"
        };

        var usuario = new Usuario
        {
            Id = 1
        };

        _usuarioRepositoryMock
            .Setup(r => r.ObterPorTokenDeResetSenha(resetDto.Token))
            .ReturnsAsync(usuario);

        // Act
        var result = await _usuarioAuthService.RedefinirSenha(resetDto);

        // Assert
        Assert.False(result);
        NotificatorMock.Verify(n => n.Handle("As senhas informadas não coincidem."), Times.Once);
        _usuarioRepositoryMock.Verify(r => r.AtualizarUsuario(It.IsAny<Usuario>()),
            Times.Never);
        _usuarioRepositoryMock.Verify(r => r.UnitOfWork.Commit(), Times.Never);
    }

    [Fact]
    public async Task ResetSenha_SenhaInvalida_ReturnsFalseAndHandlesInvalidPassword()
    {
        // Arrange
        SetupMocks(usuarioExistente: true);

        var resetDto = new ResetSenhaDto
        {
            Token = "tokenvalido",
            Senha = "123"
        };

        var usuario = new Usuario
        {
            Id = 1
        };

        _usuarioRepositoryMock
            .Setup(r => r.ObterPorTokenDeResetSenha(resetDto.Token))
            .ReturnsAsync(usuario);

        // Act
        var result = await _usuarioAuthService.RedefinirSenha(resetDto);

        // Assert
        Assert.False(result);
        NotificatorMock.Verify(n => n.Handle("As senhas informadas não coincidem."),
            Times.Once);
        _usuarioRepositoryMock.Verify(r => r.AtualizarUsuario(It.IsAny<Usuario>()),
            Times.Never);
        _usuarioRepositoryMock.Verify(r => r.UnitOfWork.Commit(), Times.Never);
    }


    [Fact]
    public async Task ResetSenha_SenhaAtualizada_ReturnsTrue()
    {
        // Arrange
        SetupMocks(usuarioExistente: true);

        var resetDto = new ResetSenhaDto
        {
            Token = "tokenvalido",
            Senha = "NovaSenha123!",
            ConfirmarSenha = "NovaSenha123!"
        };

        var usuario = new Usuario
        {
            Id = 1,
            Senha = "SenhaAntiga123!",
            ExpiraResetToken = DateTime.Now.AddDays(1)
        };

        _usuarioRepositoryMock
            .Setup(r => r.ObterPorTokenDeResetSenha(resetDto.Token))
            .ReturnsAsync(usuario);

        _passwordHasherMock
            .Setup(p => p.HashPassword(usuario, resetDto.Senha))
            .Returns("HashDaNovaSenha");

        _usuarioRepositoryMock
            .Setup(r => r.UnitOfWork.Commit())
            .ReturnsAsync(true);

        // Act
        var result = await _usuarioAuthService.RedefinirSenha(resetDto);

        // Assert
        Assert.True(result);
        NotificatorMock.Verify(n => n.HandleNotFoundResource(), Times.Never);
        NotificatorMock.Verify(n => n.Handle(It.IsAny<string>()), Times.Never);
        _usuarioRepositoryMock.Verify(r => r.AtualizarUsuario(usuario), Times.Once); 
        _usuarioRepositoryMock.Verify(r => r.UnitOfWork.Commit(), Times.Once);
        Assert.Equal("HashDaNovaSenha", usuario.Senha);
        Assert.Null(usuario.TokenDeResetSenha);
        Assert.Null(usuario.ExpiraResetToken);
    }

    [Fact]
    public async Task ResetSenha_TokenInvalido_ReturnsFalseAndHandlesNotFound()
    {
        // Arrange
        var resetDto = new ResetSenhaDto
        {
            Token = "tokeninvalido",
            Senha = "NovaSenha123!",
            ConfirmarSenha = "NovaSenha123!"
        };

        _usuarioRepositoryMock
            .Setup(r => r.ObterPorTokenDeResetSenha(resetDto.Token))
            .ReturnsAsync((Usuario)null);

        // Act
        var result = await _usuarioAuthService.RedefinirSenha(resetDto);

        // Assert
        Assert.False(result);
        NotificatorMock.Verify(n => n.HandleNotFoundResource(),
            Times.Once);
        _usuarioRepositoryMock.Verify(r => r.AtualizarUsuario(It.IsAny<Usuario>()),
            Times.Never);
        _usuarioRepositoryMock.Verify(r => r.UnitOfWork.Commit(), Times.Never);
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

        var usuario = new Usuario
        {
            Id = 1,
            TokenDeResetSenha = requestDto.Token,
            ExpiraResetToken = DateTime.Now.AddMinutes(5)
        };

        _usuarioRepositoryMock.Setup(r => r.ObterPorTokenDeResetSenha(requestDto.Token))
            .ReturnsAsync(usuario);

        _usuarioRepositoryMock.Setup(r => r.UnitOfWork.Commit())
            .ReturnsAsync(true);

        // Act
        var result = await _usuarioAuthService.RedefinirSenha(requestDto);

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

        var usuario = new Usuario
        {
            Id = 1,
            TokenDeResetSenha = requestDto.Token,
            ExpiraResetToken = DateTime.Now.AddHours(5)
        };

        _usuarioRepositoryMock.Setup(r => r.ObterPorTokenDeResetSenha(requestDto.Token))
            .ReturnsAsync(usuario);

        _usuarioRepositoryMock.Setup(r => r.UnitOfWork.Commit())
            .ReturnsAsync(false);

        // Act
        var result = await _usuarioAuthService.RedefinirSenha(requestDto);

        // Assert
        Assert.False(result);
        NotificatorMock.Verify(n => n.Handle("Não foi possível alterar a senha."), Times.Once);
    }

    #endregion

    #region SetupMocks

    private void SetupMocks(bool usuarioExistente, bool senhaCorreta = true)
    {
        var usuario = usuarioExistente
            ? new Usuario
            {
                Id = 1,
                Nome = "NomeDoUsuario",
                Email = "usuario@teste.com",
                Senha = "Senha123!",
                TokenDeResetSenha = "tokenvalido",
                ExpiraResetToken = DateTime.Now.AddMinutes(5)
            }
            : null;

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

        _usuarioRepositoryMock
            .Setup(c => c.ObterPorTokenDeResetSenha(It.IsAny<string>()))
            .ReturnsAsync(usuario);

        _usuarioRepositoryMock
            .Setup(c => c.AtualizarUsuario(It.IsAny<Usuario>()))
            .Verifiable();

        _usuarioRepositoryMock
            .Setup(c => c.UnitOfWork.Commit())
            .ReturnsAsync(true);
    }

    #endregion
}