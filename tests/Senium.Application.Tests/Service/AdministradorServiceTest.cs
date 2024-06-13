using System.Linq.Expressions;
using FluentAssertions;
using FluentAssertions.Execution;
using FluentValidation.Results;
using Microsoft.AspNetCore.Identity;
using Moq;
using Senium.Application.Dto.V1.Administrador;
using Senium.Application.Services;
using Senium.Application.Tests.Fixtures;
using Senium.Domain.Contracts.Repositories;
using Senium.Domain.Entities;

namespace Senium.Application.Tests.Service;

public class AdministradorServiceTest : BaseServiceTest, IClassFixture<ServicesFixture>
{
    private readonly AdministradorService _administradorService;

    private readonly Mock<IAdministradorRepository> _administradorRepositoryMock = new();
    private readonly Mock<IPasswordHasher<Administrador>> _passwordHasherMock = new();

    public AdministradorServiceTest(ServicesFixture fixture)
    {
        _administradorService = new AdministradorService(NotificatorMock.Object,fixture.Mapper,
            _administradorRepositoryMock.Object, _passwordHasherMock.Object);
    }

    #region Adicionar

    [Fact]
    public async Task Adicionar_AdmInvalido_HandleErros()
    {
        //Arrange
        SetupMocks();
        
        var dto = new AdicionarAdministradorDto
        {
            Email = "",
            Nome = "",
            Senha = ""
        };

        //Act
        var administrador = await _administradorService.AdicionarAdm(dto);

        //Assert
        using (new AssertionScope())
        {
            administrador.Should().BeNull();
            
            NotificatorMock
                .Verify(c => c.Handle(It.IsAny<List<ValidationFailure>>()), Times.Once);
            NotificatorMock
                .Verify(c => c.Handle(It.IsAny<string>()), Times.Never);
            
            _administradorRepositoryMock.Verify(c => c.AdicionarAdm(It.IsAny<Administrador>()), Times.Never);
            _administradorRepositoryMock.Verify(c => c.UnitOfWork.Commit(), Times.Never);
        }
    }
    
    [Fact]
    public async Task Adicionar_AdmExistente_Handle()
    {
        //Arrange
        SetupMocks(true);
        
        var dto = new AdicionarAdministradorDto
        {
            Email = "email@teste.com",
            Nome = "AdministradorTeste",
            Senha = "SenhaTeste123!"
        };

        //Act
        var administrador = await _administradorService.AdicionarAdm(dto);

        //Assert
        using (new AssertionScope())
        {
            administrador.Should().BeNull();
            
            NotificatorMock
                .Verify(c => c.Handle(It.IsAny<List<ValidationFailure>>()), Times.Never);
            NotificatorMock
                .Verify(c => c.Handle(It.IsAny<string>()), Times.Once);
            
            _administradorRepositoryMock
                .Verify(c => c.AdicionarAdm(It.IsAny<Administrador>()), Times.Never);
            _administradorRepositoryMock
                .Verify(c => c.UnitOfWork.Commit(), Times.Never);
        }
    }
    
    [Fact]
    public async Task Adicionar_ErroAoSalvar_Handle()
    {
        //Arrange
        SetupMocks();
        
        var dto = new AdicionarAdministradorDto
        {
            Email = "email@teste.com",
            Nome = "AdministradorTeste",
            Senha = "SenhaTeste123!",
        };

        //Act
        var administrador = await _administradorService.AdicionarAdm(dto);

        //Assert
        using (new AssertionScope())
        {
            administrador.Should().BeNull();
            
            NotificatorMock
                .Verify(c => c.Handle(It.IsAny<List<ValidationFailure>>()), Times.Never);
            NotificatorMock
                .Verify(c => c.Handle(It.IsAny<string>()), Times.Once);
            
            _administradorRepositoryMock
                .Verify(c => c.AdicionarAdm(It.IsAny<Administrador>()), Times.Once);
            _administradorRepositoryMock
                .Verify(c => c.UnitOfWork.Commit(), Times.Once);
        }
    }

    [Fact]
    public async Task Adicionar_SucessoAoSalvar_UsuarioDto()
    {
        //Arrange
        SetupMocks(commit: true);
        
        var dto = new AdicionarAdministradorDto
        {
            Email = "email@teste.com",
            Nome = "AdministradorTeste",
            Senha = "SenhaTeste123!"
        };

        //Act
        var administrador = await _administradorService.AdicionarAdm(dto);

        //Assert
        using (new AssertionScope())
        {
            administrador.Should().NotBeNull();
            
            NotificatorMock
                .Verify(c => c.Handle(It.IsAny<List<ValidationFailure>>()), Times.Never);
            NotificatorMock
                .Verify(c => c.Handle(It.IsAny<string>()), Times.Never);
            
            _administradorRepositoryMock
                .Verify(c => c.AdicionarAdm(It.IsAny<Administrador>()), Times.Once);
            _administradorRepositoryMock
                .Verify(c => c.UnitOfWork.Commit(), Times.Once);
        }
    }
    
    [Fact]
    public async Task Adicionar_UsuarioSenhaInvalida_HandleErros()
    {
        // Arrange
        SetupMocks();

        var dto = new AdicionarAdministradorDto
        {
            Email = "email@teste.com",
            Nome = "AdministradorTeste",
            Senha = "SenhaInvalida"
        };

        // Act
        var administrador = await _administradorService.AdicionarAdm(dto);

        // Assert
        using (new AssertionScope())
        {
            administrador.Should().BeNull();

            NotificatorMock
            .Verify(c => c.Handle(It.IsAny<List<ValidationFailure>>()), Times.Once);
            NotificatorMock
            .Verify(c => c.Handle(It.IsAny<string>()), Times.Never);

            _administradorRepositoryMock.Verify(c => c.AdicionarAdm(It.IsAny<Administrador>()), Times.Never);
            _administradorRepositoryMock.Verify(c => c.UnitOfWork.Commit(), Times.Never);
        }
    }
    
    [Fact]
    public async Task Adicionar_UsuarioEmailInvalido_HandleErros()
    {
        // Arrange
        SetupMocks();

        var dto = new AdicionarAdministradorDto
        { 
            Email = "email_invalido", 
            Nome = "AdministradorTeste",
            Senha = "SenhaTeste123!"
        };

        // Act
        var administrador = await _administradorService.AdicionarAdm(dto);

        // Assert
         using (new AssertionScope())
        {
            administrador.Should().BeNull();

            NotificatorMock
            .Verify(c => c.Handle(It.IsAny<List<ValidationFailure>>()), Times.Once);
            NotificatorMock
            .Verify(c => c.Handle(It.IsAny<string>()), Times.Never);

            _administradorRepositoryMock.Verify(c => c.AdicionarAdm(It.IsAny<Administrador>()), Times.Never);
            _administradorRepositoryMock.Verify(c => c.UnitOfWork.Commit(), Times.Never);
        }
    }
    
    #endregion

    #region Atualizar

    [Fact]
    public async Task Atualizar_IdsNaoConferem_Handle()
    {
        //Arrange
        SetupMocks();
        
        const int id = 2;
        var dto = new AtualizarAdministradorDto
        {
            Id = 1
        };
        
        //Act
        var administrador = await _administradorService.AtualizarAdm(id, dto);

        //Assert
        using (new AssertionScope())
        {
            administrador.Should().BeNull();
            
            NotificatorMock
                .Verify(c => c.Handle(It.IsAny<string>()), Times.Once);
            NotificatorMock
                .Verify(c => c.Handle(It.IsAny<List<ValidationFailure>>()), Times.Never);
            
            _administradorRepositoryMock
                .Verify(c => c.AtualizarAdm(It.IsAny<Administrador>()), Times.Never);
            _administradorRepositoryMock
                .Verify(c => c.UnitOfWork.Commit(), Times.Never);
        }
    }

    [Fact]
    public async Task Atualizar_AdminstradorInexistente_HandleNotFoundResourse()
    {
        //Arrange
        SetupMocks();
        
        const int id = 2;
        var dto = new AtualizarAdministradorDto
        {
            Id = 2
        };
        
        //Act
        var administrador = await _administradorService.AtualizarAdm(id, dto);

        //Assert
        using (new AssertionScope())
        {
            administrador.Should().BeNull();
            
            NotificatorMock
                .Verify(c => c.Handle(It.IsAny<string>()), Times.Never);
            NotificatorMock
                .Verify(c => c.Handle(It.IsAny<List<ValidationFailure>>()), Times.Never);
            NotificatorMock.
                Verify(c => c.HandleNotFoundResource(), Times.Once);
            
            _administradorRepositoryMock
                .Verify(c => c.AtualizarAdm(It.IsAny<Administrador>()), Times.Never);
            _administradorRepositoryMock
                .Verify(c => c.UnitOfWork.Commit(), Times.Never);
        }
    }
    
    [Fact]
    public async Task Atualizar_AdministradorInvalido_HandleErros()
    {
        //Arrange
        SetupMocks(true);
        
        const int id = 1;
        var dto = new AtualizarAdministradorDto
        {
            Id = 1
        };
        
        //Act
        var administrador = await _administradorService.AtualizarAdm(id, dto);
        
        //Assert
        using (new AssertionScope())
        {
            administrador.Should().BeNull();
            
            NotificatorMock
                .Verify(c => c.Handle(It.IsAny<string>()), Times.Once);
            NotificatorMock
                .Verify(c => c.Handle(It.IsAny<List<ValidationFailure>>()), Times.Once);
            
            _administradorRepositoryMock
                .Verify(c => c.AtualizarAdm(It.IsAny<Administrador>()), Times.Never);
            _administradorRepositoryMock
                .Verify(c => c.UnitOfWork.Commit(), Times.Never);
        }
    }

    [Fact]
    public async Task Atualizar_ErroAoComitar_Handle()
    {
        //Arrange
        SetupMocks();
        
        const int id = 1;
        var dto = new AtualizarAdministradorDto
        {
            Id = 1,
            Email = "email@teste.com",
            Nome = "AdministradorTeste",
            Senha = "SenhaTeste123!"
        };
        
        //Act
        var administrador = await _administradorService.AtualizarAdm(id, dto);
        
        //Assert
        using (new AssertionScope())
        {
            administrador.Should().BeNull();
            
            NotificatorMock
                .Verify(c => c.Handle(It.IsAny<string>()), Times.Once);
            NotificatorMock
                .Verify(c => c.Handle(It.IsAny<List<ValidationFailure>>()), Times.Never);
            
            _administradorRepositoryMock
                .Verify(c => c.AtualizarAdm(It.IsAny<Administrador>()), Times.Once);
            _administradorRepositoryMock
                .Verify(c => c.UnitOfWork.Commit(), Times.Once);
        }
    }

    [Fact]
    public async Task Atualizar_SucessoAoComitar_AdministradorDto()
    {
        //Arrange
        SetupMocks(commit: true);

        const int id = 1;
        var dto = new AtualizarAdministradorDto
        {
            Id = 1,
            Email = "email@teste.com",
            Nome = "AdministradorTeste",
            Senha = "SenhaTeste123!"
        };
        
        //Act
        var administrador = await _administradorService.AtualizarAdm(id, dto);
        
        //Assert
        using (new AssertionScope())
        {
            administrador.Should().NotBeNull();

            NotificatorMock
                .Verify(c => c.Handle(It.IsAny<string>()), Times.Never);
            NotificatorMock
                .Verify(c => c.Handle(It.IsAny<List<ValidationFailure>>()), Times.Never);

            _administradorRepositoryMock
                .Verify(c => c.AtualizarAdm(It.IsAny<Administrador>()), Times.Once);
            _administradorRepositoryMock
                .Verify(c => c.UnitOfWork.Commit(), Times.Once);
        }
    }


    #endregion

    #region Remover

    

    #endregion

    #region ObterTodos

    [Fact]
    public async Task ObterTodos_AdministradorExistente_Sucesso()
    {
        //Arrange
        SetupMocks(possuiAdministrador: true);

        //Act
        var administrador = await _administradorService.ObterTodosAdm();

        //Assert
        using (new AssertionScope())
        {
            administrador.Should().NotBeNull();
            administrador.Should().BeOfType<List<AdministradorDto>>();
            NotificatorMock.Verify(c => c.Handle(It.IsAny<string>()), Times.Never);
        }
    }


    #endregion
    
    #region SetupMocks

    private void SetupMocks(bool administradorExistente = false, bool commit = false, bool possuiAdministrador = true)
    {
        var administrador = new Administrador
        {
            Email = "email@teste.com",
            Nome = "AdministradorTeste",
            Senha = "SenhaTeste123!",
        };
    

        _administradorRepositoryMock
            .Setup(c => c.FirstOrDefault(It.IsAny<Expression<Func<Administrador, bool>>>()))
            .ReturnsAsync(administradorExistente ? administrador : null);

        _administradorRepositoryMock
            .Setup(c => c.UnitOfWork.Commit())
            .ReturnsAsync(commit);
        
        _administradorRepositoryMock
            .Setup(c => c.ObterAdmPorId(It.Is<int>(id => id == 1)))
            .ReturnsAsync(administrador);
        
        
        _administradorRepositoryMock
            .Setup(c => c.ObterAdmPorEmail(It.Is<string>(email => email == "email@teste.com")))
            .ReturnsAsync(administrador);
        
        _administradorRepositoryMock
            .Setup(c => c.ObterTodosAdm())!
            .ReturnsAsync(possuiAdministrador ? new List<Administrador> { administrador } : null);
    
        _passwordHasherMock
            .Setup(c => c.HashPassword(It.IsAny<Administrador>(), It.IsAny<string>()))
            .Returns("senha-hasheada");
    }

    #endregion
}