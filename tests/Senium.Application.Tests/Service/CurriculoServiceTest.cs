using System.Linq.Expressions;
using FluentAssertions;
using FluentAssertions.Execution;
using FluentValidation.Results;
using Microsoft.AspNetCore.Identity;
using Moq;
using Senium.Application.Dto.V1.Curriculo;
using Senium.Application.Services;
using Senium.Application.Tests.Fixtures;
using Senium.Domain.Contracts.Repositories;
using Senium.Domain.Entities;

namespace Senium.Application.Tests.Service;

public class CurriculoServiceTest : BaseServiceTest, IClassFixture<ServicesFixture>
{
    private readonly CurriculoService _curriculoService;

    private readonly Mock<ICurriculoRepository> _curriculoRepositoryMock = new();
    private readonly Mock<IPasswordHasher<Curriculo>> _passwordHasherMock = new();

    public CurriculoServiceTest(ServicesFixture fixture)
    {
        _curriculoService = new CurriculoService(NotificatorMock.Object,fixture.Mapper,
            _curriculoRepositoryMock.Object, _passwordHasherMock.Object);
    }

    #region Cadastrar

    [Fact]
    public async Task Adicionar_CurriculoInvalido_HandleErros()
    {
        //Arrange
        SetupMocks();
        
        var dto = new AdicionarCurriculoDto
        {
            Telefone = "",
            EstadoCivil = "",
            Genero = "",
            RacaEtnia = "",
            GrauDeFormacao = "",
            Cep = "",
            Endereco = "",
            Cidade = "",
            Estado = "",
            EPessoaComDeficiencia = "",
        };

        //Act
        var curriculo = await _curriculoService.Adicionar(dto);

        //Assert
        using (new AssertionScope())
        {
            curriculo.Should().BeNull();
            
            NotificatorMock
                .Verify(c => c.Handle(It.IsAny<List<ValidationFailure>>()), Times.Once);
            NotificatorMock
                .Verify(c => c.Handle(It.IsAny<string>()), Times.Never);
            
            _curriculoRepositoryMock.Verify(c => c.Adicionar(It.IsAny<Curriculo>()), Times.Never);
            _curriculoRepositoryMock.Verify(c => c.UnitOfWork.Commit(), Times.Never);
        }
    }

    [Fact]
    public async Task Adicionar_CurriculoExistente_Handle()
    {
        //Arrange
        SetupMocks(curriculoExistente:true);
        
        var dto = new AdicionarCurriculoDto
        {
            Telefone = "(85) 9 99999999",
            EstadoCivil = "CasadoTeste",
            Genero = "MasculinoTeste",
            RacaEtnia = "BrancoTeste",
            GrauDeFormacao = "EnsinoMédioCpmpletoTeste",
            Cep = "60805-152",
            Endereco = "RuaTeste",
            Cidade = "MaranguapeTeste",
            Estado = "CearaTeste",
            EPessoaComDeficiencia = "NãoTeste",
            
        };

        //Act
        var curriculo = await _curriculoService.Adicionar(dto);

        //Assert
        using (new AssertionScope())
        {
            curriculo.Should().BeNull();
            
            NotificatorMock
                .Verify(c => c.Handle(It.IsAny<List<ValidationFailure>>()), Times.Never);
            NotificatorMock
                .Verify(c => c.Handle(It.IsAny<string>()), Times.Once);
            
            _curriculoRepositoryMock
                .Verify(c => c.Adicionar(It.IsAny<Curriculo>()), Times.Never);
            _curriculoRepositoryMock
                .Verify(c => c.UnitOfWork.Commit(), Times.Never);
        }
    }
    
    [Fact]
    public async Task Adicionar_ErroAoSalvar_Handle()
    {
        //Arrange
        SetupMocks();
        
        var dto = new AdicionarCurriculoDto
        {
            Telefone = "(85) 9 99999999",
            EstadoCivil = "CasadoTeste",
            Genero = "MasculinoTeste",
            RacaEtnia = "BrancoTeste",
            GrauDeFormacao = "EnsinoMédioCpmpletoTeste",
            Cep = "60805-152",
            Endereco = "RuaTeste",
            Cidade = "MaranguapeTeste",
            Estado = "CearaTeste",
            EPessoaComDeficiencia = "NãoTeste",
        };

        //Act
        var curriculo = await _curriculoService.Adicionar(dto);

        //Assert
        using (new AssertionScope())
        {
            curriculo.Should().BeNull();
            
            NotificatorMock
                .Verify(c => c.Handle(It.IsAny<List<ValidationFailure>>()), Times.Never);
            NotificatorMock
                .Verify(c => c.Handle(It.IsAny<string>()), Times.Once);
            
            _curriculoRepositoryMock
                .Verify(c => c.Adicionar(It.IsAny<Curriculo>()), Times.Never);
            _curriculoRepositoryMock
                .Verify(c => c.UnitOfWork.Commit(), Times.Never);
        }
    }
    
    [Fact]
    public async Task Adicionar_SucessoAoSalvar_CurriculoDto()
    {
        //Arrange
        SetupMocks(commit: true);
        
        var dto = new AdicionarCurriculoDto
        {
            Telefone = "(85) 9 99999999",
            EstadoCivil = "CasadoTeste",
            Genero = "MasculinoTeste",
            RacaEtnia = "BrancoTeste",
            GrauDeFormacao = "EnsinoMédioCpmpletoTeste",
            Cep = "60805-152",
            Endereco = "RuaTeste",
            Cidade = "MaranguapeTeste",
            Estado = "CearaTeste",
            EPessoaComDeficiencia = "NãoTeste",
        };

        //Act
        var curriculo = await _curriculoService.Adicionar(dto);

        //Assert
        using (new AssertionScope())
        {
            curriculo.Should().NotBeNull();
            
            
            NotificatorMock
                .Verify(c => c.Handle(It.IsAny<List<ValidationFailure>>()), Times.Never);
            NotificatorMock
                .Verify(c => c.Handle(It.IsAny<string>()), Times.Once);
            
            _curriculoRepositoryMock
                .Verify(c => c.Adicionar(It.IsAny<Curriculo>()), Times.Never);
            _curriculoRepositoryMock
                .Verify(c => c.UnitOfWork.Commit(), Times.Never);
        }
    }
    
    #endregion

    #region ObterPorId

    [Fact]
    public async Task ObterPorId_CurriculoInexistente_HandleNotFoundResource()
    {
        // Arrange
        SetupMocks();

        const int id = 2;
    
        // Act
        var curriculoDto = await _curriculoService.ObterPorId(id);

        // Assert
        using (new AssertionScope())
        {
            curriculoDto.Should().BeNull();
            NotificatorMock.Verify(c => c.HandleNotFoundResource(), Times.Once);
        }
    }

    [Fact]
    public async Task ObterPorId_CurriculoExistente_Sucesso()
    {
        // Arrange
        SetupMocks(curriculoExistente: true);

        const int id = 1;
    
        // Act
        var curriculoDto = await _curriculoService.ObterPorId(id);

        // Assert
        using (new AssertionScope())
        {
            curriculoDto.Should().NotBeNull();
            curriculoDto.Should().BeOfType<CurriculoDto>();
            NotificatorMock.Verify(c => c.HandleNotFoundResource(), Times.Never);
        }
    }
    
    #endregion
    
    #region SetupMocks

    private void SetupMocks(bool curriculoExistente = false, bool commit = false)
    {
        var curriculo = new Curriculo
        {
            Telefone = "(85) 9 99999999",
            EstadoCivil = "CasadoTeste",
            Genero = "MasculinoTeste",
            RacaEtnia = "BrancoTeste",
            GrauDeFormacao = "EnsinoMédioCpmpletoTeste",
            Cep = "60805-152",
            Endereco = "RuaTeste",
            Cidade = "MaranguapeTeste",
            Estado = "CearaTeste",
            EPessoaComDeficiencia = false,
        };
        
        _curriculoRepositoryMock
            .Setup(c => c.FirstOrDefault(It.IsAny<Expression<Func<Curriculo, bool>>>()))
            .ReturnsAsync(curriculoExistente ? curriculo : null);

        _curriculoRepositoryMock
            .Setup(c => c.UnitOfWork.Commit())
            .ReturnsAsync(commit);
        
        _curriculoRepositoryMock
            .Setup(c => c.ObterPorId(It.Is<int>(id => id == 1)))
            .ReturnsAsync(curriculo);
    }   
    
    #endregion
}