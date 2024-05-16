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
        var empresa = await _empresaService.Adicionar(dto);

        // Assert
        using (new AssertionScope())
        {
            empresa.Should().BeNull();

            NotificatorMock
                .Verify(c => c.Handle(It.IsAny<List<ValidationFailure>>()), Times.Once);

            _empresaRepositoryMock
                .Verify(c => c.Cadastrar(It.IsAny<Empresa>()), Times.Never);
        
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
        var empresa = await _empresaService.Adicionar(dto);

        // Assert
        using (new AssertionScope())
        {
            empresa.Should().BeNull();

            NotificatorMock
                .Verify(c => c.Handle(It.IsAny<string>()), Times.Once);

            _empresaRepositoryMock
                .Verify(c => c.Cadastrar(It.IsAny<Empresa>()), Times.Never);
        
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
        var empresa = await _empresaService.Adicionar(dto);

        // Assert
        using (new AssertionScope())
        {
            empresa.Should().BeNull();

            NotificatorMock
                .Verify(c => c.Handle(It.IsAny<string>()), Times.Once);

            _empresaRepositoryMock
                .Verify(c => c.Cadastrar(It.IsAny<Empresa>()), Times.Once);

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
        var empresa = await _empresaService.Adicionar(dto);

        // Assert
        using (new AssertionScope())
        { 
            empresa.Should().NotBeNull();

            NotificatorMock
                .Verify(c => c.Handle(It.IsAny<string>()), Times.Never);

            _empresaRepositoryMock
                .Verify(c => c.Cadastrar(It.IsAny<Empresa>()), Times.Once);

            _empresaRepositoryMock
                .Verify(c => c.UnitOfWork.Commit(), Times.Once);
        }
    }

    #endregion

    
    #region SetupMocks

    
    private void SetupMocks(bool empresaExistente = false, bool commit = false)
    {
        var empresa = new Empresa
        {
            Nome = "UsuarioTeste",
            Email = "emailempresa@teste.com",
            Telefone = "(21)912345678",
            NomeDaEmpresa = "EmpresaTeste"
        };
    

        _empresaRepositoryMock
            .Setup(c => c.FirstOrDefault(It.IsAny<Expression<Func<Empresa, bool>>>()))
            .ReturnsAsync(empresaExistente ? empresa : null);

        _empresaRepositoryMock
            .Setup(c => c.UnitOfWork.Commit())
            .ReturnsAsync(commit);
    }

    #endregion
}