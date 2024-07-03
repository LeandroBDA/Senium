using System.Linq.Expressions;
using FluentAssertions;
using FluentAssertions.Execution;
using FluentValidation.Results;
using Moq;
using Senium.Application.Dto.V1.Empresa;
using Senium.Application.Services;
using Senium.Application.Tests.Fixtures;
using Senium.Domain.Contracts.Repositories;
using Senium.Domain.Entities;

namespace Senium.Application.Tests.Service;

public class EmpresaServiceTest : BaseServiceTest, IClassFixture<ServicesFixture>
{
    private readonly EmpresaService _empresaService;
    private readonly Mock<IEmpresaRepository> _empresaRepositoryMock = new();

    public EmpresaServiceTest(ServicesFixture fixture)
    {
        _empresaService = new EmpresaService(NotificatorMock.Object, fixture.Mapper,
            _empresaRepositoryMock.Object);
    }

    #region Cadastrar

    [Fact]
    public async Task Adicionar_EmpresaInvalida_HandleErros()
    {
        // Arrange
        SetupMocks();

        var dto = new AdicionarEmpresaDto
        {
            Nome = "",
            Email = "",
            Telefone = "",
            NomeDaEmpresa = ""
        };

        // Act
        var empresa = await _empresaService.AdicionarEmpresa(dto);

        // Assert
        using (new AssertionScope())
        {
            empresa.Should().BeNull();

            NotificatorMock
                .Verify(c => c.Handle(It.IsAny<List<ValidationFailure>>()), Times.Once);

            _empresaRepositoryMock
                .Verify(c => c.CadastrarEmpresa(It.IsAny<Empresa>()), Times.Never);
        
            _empresaRepositoryMock
                .Verify(c => c.UnitOfWork.Commit(), Times.Never);
        }
    }

    [Fact]
    public async Task Adicionar_EmpresaExistente_Handle()
    {
        // Arrange
        SetupMocks(empresaExistente: true);

        var dto = new AdicionarEmpresaDto
        {
            Nome = "EmpresaTeste", 
            Email = "emailempresa@teste.com",
            Telefone = "(21)912345678",
            NomeDaEmpresa = "EmpresaTeste"
        };

        // Act
        var empresa = await _empresaService.AdicionarEmpresa(dto);

        // Assert
        using (new AssertionScope())
        {
            empresa.Should().BeNull();

            NotificatorMock
                .Verify(c => c.Handle(It.IsAny<string>()), Times.Once);

            _empresaRepositoryMock
                .Verify(c => c.CadastrarEmpresa(It.IsAny<Empresa>()), Times.Never);
        
            _empresaRepositoryMock
                .Verify(c => c.UnitOfWork.Commit(), Times.Never);
        }
    }

    [Fact]
    public async Task Adicionar_ErroAoSalvar_Handle()
    {
        // Arrange
        SetupMocks();

        var dto = new AdicionarEmpresaDto
        {
            Nome = "EmpresaTeste",
            Email = "emailempresa@teste.com",
            Telefone = "(21)912345678",
            NomeDaEmpresa = "EmpresaTeste"
        };

        // Act
        var empresa = await _empresaService.AdicionarEmpresa(dto);

        // Assert
        using (new AssertionScope())
        {
            empresa.Should().BeNull();

            NotificatorMock
                .Verify(c => c.Handle(It.IsAny<string>()), Times.Once);

            _empresaRepositoryMock
                .Verify(c => c.CadastrarEmpresa(It.IsAny<Empresa>()), Times.Once);

            _empresaRepositoryMock
                .Verify(c => c.UnitOfWork.Commit(), Times.Once);
        }
    }

    [Fact]
    public async Task Adicionar_SucessoAoSalvar_EmpresaDto()
    {
        // Arrange
        SetupMocks(commit: true);

        var dto = new AdicionarEmpresaDto
        {
            Nome = "EmpresaTeste",
            Email = "emailempresa@teste.com",
            Telefone = "(21)912345678",
            NomeDaEmpresa = "EmpresaTeste"
        };

        // Act
        var empresa = await _empresaService.AdicionarEmpresa(dto);

        // Assert
        using (new AssertionScope())
        { 
            empresa.Should().NotBeNull();

            NotificatorMock
                .Verify(c => c.Handle(It.IsAny<string>()), Times.Never);

            _empresaRepositoryMock
                .Verify(c => c.CadastrarEmpresa(It.IsAny<Empresa>()), Times.Once);

            _empresaRepositoryMock
                .Verify(c => c.UnitOfWork.Commit(), Times.Once);
        }
    }

    #endregion

    #region ObterTodos

    [Fact]
    public async Task ObterTodasEmpresas_ExistemEmpresas_Sucesso()
    {
        // Arrange
        SetupMocks(empresaExistente: true);

        // Act
        var empresas = await _empresaService.ObterTodasEmpresas();

        // Assert
        using (new AssertionScope())
        {
            empresas.Should().NotBeNull();
            empresas.Should().BeOfType<List<EmpresaDto>>();
            NotificatorMock.Verify(c => c.HandleNotFoundResource(), Times.Never);

            // Verifica se o método do repositório foi chamado uma vez
            _empresaRepositoryMock.Verify(c => c.ObterTodasEmpresas(), Times.Once);
        }
    }

    [Fact]
    public async Task ObterTodasEmpresas_NaoExistemEmpresas_HandleNotFound()
    {
        // Arrange
        SetupMocks(empresaExistente: false);

        // Act
        var empresas = await _empresaService.ObterTodasEmpresas();

        // Assert
        using (new AssertionScope())
        {
            empresas.Should().BeNull();
            NotificatorMock.Verify(c => c.HandleNotFoundResource(), Times.Once);

            // Verifica se o método do repositório foi chamado uma vez
            _empresaRepositoryMock.Verify(c => c.ObterTodasEmpresas(), Times.Once);
        }
    }
    
    #endregion
    
    #region SetupMocks

    private void SetupMocks(bool empresaExistente = false, bool commit = false)
    {
        List<Empresa>? empresas = empresaExistente
            ? new List<Empresa>
            {
                new Empresa
                {
                    Id = 1,
                    Nome = "Empresa A",
                    Email = "email.empresa@teste.com",
                    Telefone = "(11) 98765-4321",
                    NomeDaEmpresa = "Empresa A Ltda"
                },
                new Empresa
                {
                    Id = 2,
                    Nome = "Empresa B",
                    Email = "email.empresa.b@teste.com",
                    Telefone = "(22) 98765-4321",
                    NomeDaEmpresa = "Empresa B S.A."
                }
            }
            : null;


        if (empresas != null)
        {
            _empresaRepositoryMock
                .Setup(c => c.FirstOrDefault(It.IsAny<Expression<Func<Empresa, bool>>>()))
                .ReturnsAsync(empresaExistente ? empresas.FirstOrDefault() : null);

            _empresaRepositoryMock
                .Setup(c => c.ObterTodasEmpresas())
                .ReturnsAsync(empresas);
        }

        _empresaRepositoryMock
            .Setup(c => c.UnitOfWork.Commit())
            .ReturnsAsync(commit);
    }


    #endregion

    #region AtualizarEmpresa

    [Fact]
    public async Task Atualizar_NomesNaoConferem_Handle()
    {
        //Arrange
        SetupMocks();
        const int id = 1;
        var dto = new AtualizarEmpresaDto
        {
            Id = 2
        };
        
        //Act
        var empresa = await _empresaService.AtualizarEmpresa(id, dto);

        //Assert
        using (new AssertionScope())
        {
            empresa.Should().BeNull();
            
          
            NotificatorMock
                .Verify(c => c.Handle(It.IsAny<List<ValidationFailure>>()), Times.Never);
            
            _empresaRepositoryMock
                .Verify(c => c.AtualizarEmpresa(It.IsAny<Empresa>()), Times.Never);
            _empresaRepositoryMock
                .Verify(c => c.UnitOfWork.Commit(), Times.Never);
        }
    }

    [Fact]
    public async Task Atualizar_EmpresaInexistente_HandleNotFoundResourse()
    {
        //Arrange
        SetupMocks();
        
        const int id = 2;
        var dto = new AtualizarEmpresaDto
        {
            Id = 2
        };
        
        //Act
        var empresa = await _empresaService.AtualizarEmpresa(id, dto);


        //Assert
        using (new AssertionScope())
        {
            empresa.Should().BeNull();
            
            NotificatorMock
                .Verify(c => c.Handle(It.IsAny<string>()), Times.Never);
            NotificatorMock
                .Verify(c => c.Handle(It.IsAny<List<ValidationFailure>>()), Times.Never);
            NotificatorMock.
                Verify(c => c.HandleNotFoundResource(), Times.Once);
            
             
            _empresaRepositoryMock
                .Verify(c => c.AtualizarEmpresa(It.IsAny<Empresa>()), Times.Never);
            _empresaRepositoryMock
                .Verify(c => c.UnitOfWork.Commit(), Times.Never);
        }
    }
    
    [Fact]
    public async Task Atualizar_ErroAoComitar_Handle()
    {
        //Arrange
        SetupMocks();
        
        const int id = 1;
        var dto = new AtualizarEmpresaDto
        {
            Id = 1,
            Nome = "Embraer",
            Email = "Embraer@gmail.com",
            NomeDaEmpresa = "Embraer",
            Telefone = "85 988888888",
        };
        
        //Act
        var empresa = await _empresaService.AtualizarEmpresa(id, dto);
        
        //Assert
        using (new AssertionScope())
        {
            empresa.Should().BeNull();
            
           
            NotificatorMock
                .Verify(c => c.Handle(It.IsAny<List<ValidationFailure>>()), Times.Never);
            
            _empresaRepositoryMock
                .Verify(c => c.AtualizarEmpresa(It.IsAny<Empresa>()), Times.Never);
            _empresaRepositoryMock
                .Verify(c => c.UnitOfWork.Commit(), Times.Never);
        }
    }
    
    [Fact]
    public async Task Atualizar_EmpresaInvalida_HandleErros()
    {
        // Arrange
        SetupMocks(true);
        
        const int id = 1;
        var dto = new AtualizarEmpresaDto
        {
            Id = 1,
            Nome = "Embraer",
            Email = "Embraer@gmail.com",
            NomeDaEmpresa = "Embraer",
            Telefone = "85 988888888",
        };
        
        // Act
        var empresa = await _empresaService.AtualizarEmpresa( id, dto);

        // Assert
        using (new AssertionScope())
        {
            empresa.Should().BeNull();

          
            _empresaRepositoryMock
                .Verify(c => c.AtualizarEmpresa(It.IsAny<Empresa>()), Times.Never);
            _empresaRepositoryMock
                .Verify(c => c.UnitOfWork.Commit(), Times.Never);
        }
    }
    
    [Fact]
    public async Task Atualizar_SucessoAoComitar_EmpresaDto()
    {
        //Arrange
        SetupMocks(commit: true);

        const int id = 1;
        var dto = new AtualizarEmpresaDto
        {
            Id = 1,
            Nome = "Embraer",
            Email = "Embraer@gmail.com",
            NomeDaEmpresa = "Embraer",
            Telefone = "85 988888888",
        };
        
        //Act
        var empresa = await _empresaService.AtualizarEmpresa(id, dto);
        
        //Assert
        using (new AssertionScope())
        {
            empresa.Should().BeNull();

            NotificatorMock
                .Verify(c => c.Handle(It.IsAny<string>()), Times.Never);
            _empresaRepositoryMock
                .Verify(c => c.AtualizarEmpresa(It.IsAny<Empresa>()), Times.Never);
            _empresaRepositoryMock
                .Verify(c => c.UnitOfWork.Commit(), Times.Never);
        }
    }

    #endregion
    
}