﻿using System.Linq.Expressions;
using FluentAssertions;
using FluentAssertions.Execution;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Identity;
using Moq;
using Senium.Application.Dto.V1.Usuario;
using Senium.Application.Services;
using Senium.Application.Tests.Fixtures;
using Senium.Domain.Contracts.Repositories;
using Senium.Domain.Entities;

namespace Senium.Application.Tests.Service;

public class UsuarioServiceTest : BaseServiceTest, IClassFixture<ServicesFixture>
{
    private readonly UsuarioService _usuarioService;

    private readonly Mock<IUsuarioRepository> _usuarioRepositoryMock = new();
    private readonly Mock<IPasswordHasher<Usuario>> _passwordHasherMock = new();

    public UsuarioServiceTest(ServicesFixture fixture)
    {
        _usuarioService = new UsuarioService(NotificatorMock.Object,fixture.Mapper,
            _usuarioRepositoryMock.Object, _passwordHasherMock.Object);
    }

    #region Cadastrar

    [Fact]
    public async Task Adicionar_UsuarioInvalido_HandleErros()
    {
        //Arrange
        SetupMocks();
        
        var dto = new AdicionarUsuarioDto
        {
            Email = "",
            Nome = "",
            Senha = "",
            ConfirmarSenha = "",
            DataDeNascimento = DateTime.MinValue
        };

        //Act
        var usuario = await _usuarioService.AdicionarUsuario(dto);

        //Assert
        using (new AssertionScope())
        {
            usuario.Should().BeNull();
            
            NotificatorMock
                .Verify(c => c.Handle(It.IsAny<List<ValidationFailure>>()), Times.Once);
            NotificatorMock
                .Verify(c => c.Handle(It.IsAny<string>()), Times.Never);
            
            _usuarioRepositoryMock.Verify(c => c.CadastrarUsuario(It.IsAny<Usuario>()), Times.Never);
            _usuarioRepositoryMock.Verify(c => c.UnitOfWork.Commit(), Times.Never);
        }
    }

    [Fact]
    public async Task Adicionar_UsuarioExistente_Handle()
    {
        //Arrange
        SetupMocks(true);
        
        var dto = new AdicionarUsuarioDto
        {
            Email = "email@teste.com",
            Nome = "UsuarioTeste",
            Senha = "SenhaTeste123!",
            ConfirmarSenha = "SenhaTeste123!",
            DataDeNascimento = DateTime.Parse("06/03/1979")
            
        };

        //Act
        var usuario = await _usuarioService.AdicionarUsuario(dto);

        //Assert
        using (new AssertionScope())
        {
            usuario.Should().BeNull();
            
            NotificatorMock
                .Verify(c => c.Handle(It.IsAny<List<ValidationFailure>>()), Times.Never);
            NotificatorMock
                .Verify(c => c.Handle(It.IsAny<string>()), Times.Once);
            
            _usuarioRepositoryMock
                .Verify(c => c.CadastrarUsuario(It.IsAny<Usuario>()), Times.Never);
            _usuarioRepositoryMock
                .Verify(c => c.UnitOfWork.Commit(), Times.Never);
        }
    }
    
    [Fact]
    public async Task Adicionar_ErroAoSalvar_Handle()
    {
        //Arrange
        SetupMocks();
        
        var dto = new AdicionarUsuarioDto
        {
            Email = "email@teste.com",
            Nome = "UsuarioTeste",
            Senha = "SenhaTeste123!",
            ConfirmarSenha = "SenhaTeste123!",
            DataDeNascimento = DateTime.Parse("06/03/1979")
        };

        //Act
        var usuario = await _usuarioService.AdicionarUsuario(dto);

        //Assert
        using (new AssertionScope())
        {
            usuario.Should().BeNull();
            
            NotificatorMock
                .Verify(c => c.Handle(It.IsAny<List<ValidationFailure>>()), Times.Never);
            NotificatorMock
                .Verify(c => c.Handle(It.IsAny<string>()), Times.Once);
            
            _usuarioRepositoryMock
                .Verify(c => c.CadastrarUsuario(It.IsAny<Usuario>()), Times.Once);
            _usuarioRepositoryMock
                .Verify(c => c.UnitOfWork.Commit(), Times.Once);
        }
    }
    
    [Fact]
    public async Task Adicionar_SucessoAoSalvar_UsuarioDto()
    {
        //Arrange
        SetupMocks(commit: true);
        
        var dto = new AdicionarUsuarioDto
        {
            Email = "email@teste.com",
            Nome = "UsuarioTeste",
            Senha = "SenhaTeste123!",
            ConfirmarSenha = "SenhaTeste123!",
            DataDeNascimento = DateTime.Parse("06/03/1979")
        };

        //Act
        var usuario = await _usuarioService.AdicionarUsuario(dto);

        //Assert
        using (new AssertionScope())
        {
            usuario.Should().NotBeNull();
            
            
            NotificatorMock
                .Verify(c => c.Handle(It.IsAny<List<ValidationFailure>>()), Times.Never);
            NotificatorMock
                .Verify(c => c.Handle(It.IsAny<string>()), Times.Never);
            
            _usuarioRepositoryMock
                .Verify(c => c.CadastrarUsuario(It.IsAny<Usuario>()), Times.Once);
            _usuarioRepositoryMock
                .Verify(c => c.UnitOfWork.Commit(), Times.Once);
        }
    }
    
    [Fact]
    public async Task Adicionar_UsuarioSenhaInvalida_HandleErros()
    {
        // Arrange
        SetupMocks();

        var dto = new AdicionarUsuarioDto
        {
            Email = "email@teste.com",
            Nome = "UsuarioTeste",
            Senha = "SenhaInvalida", 
            ConfirmarSenha = "SenhaInvalida", 
            DataDeNascimento = DateTime.Now.AddYears(-30)
        };

        // Act
        var usuario = await _usuarioService.AdicionarUsuario(dto);

        // Assert
        using (new AssertionScope())
        {
            usuario.Should().BeNull();

            NotificatorMock
            .Verify(c => c.Handle(It.IsAny<List<ValidationFailure>>()), Times.Once);
            NotificatorMock
            .Verify(c => c.Handle(It.IsAny<string>()), Times.Never);

            _usuarioRepositoryMock.Verify(c => c.CadastrarUsuario(It.IsAny<Usuario>()), Times.Never);
            _usuarioRepositoryMock.Verify(c => c.UnitOfWork.Commit(), Times.Never);
        }
    }
    
    [Fact]
    public async Task Adicionar_UsuarioSenhasDiferentes_HandleErros()
    {
        // Arrange
        SetupMocks();

        var dto = new AdicionarUsuarioDto
        {
            Email = "email@teste.com",
            Nome = "UsuarioTeste",
            Senha = "SenhaTeste123!", 
            ConfirmarSenha = "SenhaDiferente", 
            DataDeNascimento = DateTime.Now.AddYears(-30) 
        };

        // Act
        var usuario = await _usuarioService.AdicionarUsuario(dto);

        // Assert
        using (new AssertionScope())
        {
            usuario.Should().BeNull();

            NotificatorMock
                .Verify(c => c.Handle("As senhas informadas não coincidem"), Times.Once, "Mensagem incorreta ou não chamada");
        }
    }
    
    [Fact]
    public async Task Adicionar_UsuarioEmailInvalido_HandleErros()
    {
        // Arrange
        SetupMocks();

        var dto = new AdicionarUsuarioDto
        { 
            Email = "email_invalido", 
            Nome = "UsuarioTeste",
            Senha = "SenhaTeste123!",
            ConfirmarSenha = "SenhaTeste123!",
            DataDeNascimento = DateTime.Now.AddYears(-30)
        };

        // Act
        var usuario = await _usuarioService.AdicionarUsuario(dto);

        // Assert
         using (new AssertionScope())
        {
            usuario.Should().BeNull();

            NotificatorMock
            .Verify(c => c.Handle(It.IsAny<List<ValidationFailure>>()), Times.Once);
            NotificatorMock
            .Verify(c => c.Handle(It.IsAny<string>()), Times.Never);

            _usuarioRepositoryMock.Verify(c => c.CadastrarUsuario(It.IsAny<Usuario>()), Times.Never);
            _usuarioRepositoryMock.Verify(c => c.UnitOfWork.Commit(), Times.Never);
        }
    }

    [Fact]
    public async Task Adicionar_UsuarioDataNascimentoAbaixo45Anos_HandleErros()
    {
        // Arrange
        SetupMocks();

        var dto = new AdicionarUsuarioDto
        {
            Email = "email@teste.com",
            Nome = "UsuarioTeste",
            Senha = "SenhaTeste123!",
            ConfirmarSenha = "SenhaTeste123!",
            DataDeNascimento = DateTime.Now.AddYears(-44)
        };

        // Act
        var usuario = await _usuarioService.AdicionarUsuario(dto);

        // Assert
        using (new AssertionScope())
        {
            usuario.Should().BeNull();

             NotificatorMock
            .Verify(c => c.Handle(It.IsAny<List<ValidationFailure>>()), Times.Once);
            NotificatorMock
            .Verify(c => c.Handle(It.IsAny<string>()), Times.Never);

            _usuarioRepositoryMock.Verify(c => c.CadastrarUsuario(It.IsAny<Usuario>()), Times.Never);
            _usuarioRepositoryMock.Verify(c => c.UnitOfWork.Commit(), Times.Never);
        }
    }


    #endregion
    
    #region Atualizar

    [Fact]
    public async Task AtualizarUsuario_IdNaoConfere_HandleErro()
    {
        // Arrange
        SetupMocks();

        var dto = new AtualizarUsuarioDto
        {
            Id = 2,
            Email = "email@teste.com",
            Nome = "UsuarioTesteAtualizado"
        };

        // Act
        var usuario = await _usuarioService.AtualizarUsuario(1, dto);

        // Assert
        using (new AssertionScope())
        {
            usuario.Should().BeNull();
            NotificatorMock.Verify(c => c.Handle("Ids não conferem"), Times.Once);
        }
    }

    [Fact]
    public async Task AtualizarUsuario_UsuarioNaoEncontrado_HandleNotFoundResource()
    {
        // Arrange
        SetupMocks();

        var dto = new AtualizarUsuarioDto
        {
            Id = 1,
            Email = "email@teste.com",
            Nome = "UsuarioTesteAtualizado"
        };

        _usuarioRepositoryMock.Setup(c => c.ObterUsuarioPorId(It.IsAny<int>())).ReturnsAsync((Usuario?)null);

        // Act
        var usuario = await _usuarioService.AtualizarUsuario(1, dto);

        // Assert
        using (new AssertionScope())
        {
            usuario.Should().BeNull();
            NotificatorMock.Verify(c => c.HandleNotFoundResource(), Times.Once);
        }
    }

    [Fact]
    public async Task AtualizarUsuario_UsuarioExistente_Handle()
    {
        // Arrange
        SetupMocks(usuarioExistente: true);

        var dto = new AtualizarUsuarioDto
        {
            Id = 1,
            Email = "email@teste.com",
            Nome = "UsuarioTesteAtualizado"
        };

        // Act
        var usuario = await _usuarioService.AtualizarUsuario(1, dto);

        // Assert
        using (new AssertionScope())
        {
            usuario.Should().BeNull();
            NotificatorMock.Verify(c => c.Handle("Já existe um usuario cadastrado com esse e-mail!"), Times.Once);
        }
    }

    #endregion

    #region ObterPorId

    [Fact]
    public async Task ObterPorId_UsuarioInexistente_HandleNotFoundResource()
    {
        // Arrange
        SetupMocks();

        const int id = 2;

        // Act
        var usuarioDto = await _usuarioService.ObterUsuarioPorId(id);

        // Assert
        using (new AssertionScope())
        {
            usuarioDto.Should().BeNull();
            NotificatorMock.Verify(c => c.HandleNotFoundResource(), Times.Once);
        }
    }

    [Fact]
    public async Task ObterPorId_UsuarioExistente_Sucesso()
    {
        // Arrange
        SetupMocks(usuarioExistente: true);

        const int id = 1;

        // Act
        var usuarioDto = await _usuarioService.ObterUsuarioPorId(id);

        // Assert
        using (new AssertionScope())
        {
            usuarioDto.Should().NotBeNull();
            usuarioDto.Should().BeOfType<UsuarioDto>();
            NotificatorMock.Verify(c => c.HandleNotFoundResource(), Times.Never);
        }
    }

    [Fact]
    public async Task AtualizarUsuario_CommitBemSucedido_RetornaUsuarioDto()
    {
        // Arrange
        SetupMocks(commit: true);

        var dto = new AtualizarUsuarioDto
        {
            Id = 1,
            Email = "email@teste.com",
            Nome = "UsuarioTesteAtualizado",
            DataDeNascimento = DateTime.Parse("06/03/1979")
        };

        // Act
        var usuario = await _usuarioService.AtualizarUsuario(1, dto);

        // Assert
        using (new AssertionScope())
        {
            NotificatorMock
                .Verify(c => c.Handle(It.IsAny<List<ValidationFailure>>()), Times.Never);
            NotificatorMock
                .Verify(c => c.Handle(It.IsAny<string>()), Times.Never);

            _usuarioRepositoryMock
                .Verify(c => c.AtualizarUsuario(It.IsAny<Usuario>()), Times.Once);
            _usuarioRepositoryMock
                .Verify(c => c.UnitOfWork.Commit(), Times.Once);
        }
    }

    [Fact]
    public async Task AtualizarUsuario_CommitFalha_HandleErro()
    {
        // Arrange
        SetupMocks(commit: false);

        var dto = new AtualizarUsuarioDto
        {
            Id = 1,
            Email = "email@teste.com",
            Nome = "UsuarioTesteAtualizado",
            DataDeNascimento = DateTime.Parse("06/03/1979")
        };

        // Act
        var usuario = await _usuarioService.AtualizarUsuario(1, dto);

        // Assert
        using (new AssertionScope())
        {
            usuario.Should().BeNull();
            NotificatorMock.Verify(c => c.Handle("Não foi possivel atualizar o usuário"), Times.Once);
        }
    }


    #endregion
    
    #region ObterPorEmail

    [Fact]
    public async Task ObterPorEmail_UsuarioInexistente_HandleNotFoundResource()
    {
        //Arrange
        SetupMocks();

        const string email = "email123@teste.com";
    
        //Act
        var usuario = await _usuarioService.ObterUsuarioPorEmail(email);

        //Assert
        using (new AssertionScope())
        {
            usuario.Should().BeNull();
            NotificatorMock.Verify(c => c.HandleNotFoundResource(), Times.Once);
        }
    }

    [Fact]
    public async Task ObterPorEmail_UsuarioExistente_Sucesso()
    {
        //Arrange
        SetupMocks();

        const string email = "email@teste.com";
    
        //Act
        var usuario = await _usuarioService.ObterUsuarioPorEmail(email);

        //Assert
        using (new AssertionScope())
        {
            usuario.Should().NotBeNull();
            usuario.Should().BeOfType<UsuarioDto>();
            NotificatorMock.Verify(c => c.HandleNotFoundResource(), Times.Never);
        }
    }

    #endregion

    #region ObterTodos
    
    [Fact]
    public async Task ObterTodos_UsuariosExistente_Sucesso()
    {
        //Arrange
        SetupMocks(possuiUsuario: true);

        //Act
        var usuarios = await _usuarioService.ObterTodosUsuarios();

        //Assert
        using (new AssertionScope())
        {
            usuarios.Should().NotBeNull();
            usuarios.Should().BeOfType<List<UsuarioDto>>();
            NotificatorMock.Verify(c => c.Handle(It.IsAny<string>()), Times.Never);
        }
    }

    #endregion
    
    #region SetupMocks

    private void SetupMocks(bool usuarioExistente = false, bool commit = false, bool possuiUsuario = true)
    {
        var usuario = new Usuario
        {
            Id = 1,
            Email = "email@teste.com",
            Nome = "UsuarioTeste",
            Senha = "SenhaTeste123!",
            DataDeNascimento = DateTime.Parse("06/03/1979")
        };

        _usuarioRepositoryMock
            .Setup(c => c.ObterUsuarioPorId(It.IsAny<int>()))
            .ReturnsAsync((int id) => id == usuario.Id ? usuario : null);

        _usuarioRepositoryMock
            .Setup(c => c.ObterUsuarioPorEmail(It.IsAny<string>()))
            .ReturnsAsync((string email) => email == usuario.Email ? usuario : null);

        _usuarioRepositoryMock
            .Setup(c => c.FirstOrDefault(It.IsAny<Expression<Func<Usuario, bool>>>()))
            .ReturnsAsync(usuarioExistente ? usuario : null);

        _usuarioRepositoryMock
            .Setup(c => c.UnitOfWork.Commit())
            .ReturnsAsync(commit);

        _usuarioRepositoryMock
            .Setup(c => c.ObterTodosUsuarios())
            .ReturnsAsync(possuiUsuario ? new List<Usuario> { usuario } : null);

        _passwordHasherMock
            .Setup(c => c.HashPassword(It.IsAny<Usuario>(), It.IsAny<string>()))
            .Returns("senha-hasheada");

        _passwordHasherMock
            .Setup(c => c.VerifyHashedPassword(It.IsAny<Usuario>(), It.IsAny<string>(), It.IsAny<string>()))
            .Returns(PasswordVerificationResult.Success);
    }

    

    #endregion
}