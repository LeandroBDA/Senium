using System.Linq.Expressions;
using FluentAssertions;
using FluentAssertions.Execution;
using FluentValidation.Results;
using Moq;
using Senium.Application.Dto.V1.Experiencia;
using Senium.Application.Services;
using Senium.Application.Tests.Fixtures;
using Senium.Domain.Contracts.Repositories;
using Senium.Domain.Entities;

namespace Senium.Application.Tests.Service;

public class ExperienciaServiceTest : BaseServiceTest, IClassFixture<ServicesFixture>
{
    private readonly ExperienciaService _experienciaService;
    private readonly Mock<IExperienciaRepository> _experienciaRepositoryMock = new();
    private readonly Mock<ICurriculoRepository> _curriculoRepositoryMock = new();
    
    public ExperienciaServiceTest(ServicesFixture fixture)
    {
        _experienciaService = new ExperienciaService(NotificatorMock.Object, fixture.Mapper,
            _experienciaRepositoryMock.Object, _curriculoRepositoryMock.Object);
    }

    #region AdicionarExperiencia

    [Fact]
    public async Task Adicionar_ExperienciaInvalida_HandleErros()
    {
        // Arrange
        SetupMocks();

        var dto = new AdicionarExperienciaDto
        {
            CurriculoId = 1,
            Cargo = "",
            Empresa = " ",
            Descricao = " ",
            DataDeInicio = new DateTime(2023, 3, 6),
            DataDeTermino = null,
            TrabalhoAtual = true
        };
        
        // Act
        var experiencia = await _experienciaService.AdicionarExperiencia(dto);

        // Assert
        using (new AssertionScope())
        {
            experiencia.Should().BeNull();

            NotificatorMock
                .Verify(c => c.Handle(It.IsAny<List<ValidationFailure>>()), Times.Once);

            _experienciaRepositoryMock
                .Verify(c => c.AdicionarExperiencia(It.IsAny<Experiencia>()), Times.Never);
        
            _experienciaRepositoryMock
                .Verify(c => c.UnitOfWork.Commit(), Times.Never);
        }
    }

    [Fact]
    public async Task Adicionar_ExperienciaExistente_Handle()
    {
        // Arrange
        SetupMocks(experienciaExistente: true);

        var dto = new AdicionarExperienciaDto
        {
            CurriculoId = 1,
            Cargo = "Desenvolvedor de Software",
            Empresa = "Tech Solutions Ltda",
            DataDeInicio = new DateTime(2023, 3, 6),
            DataDeTermino = null,
            TrabalhoAtual = true
        };

        // Act
        var experiencia = await _experienciaService.AdicionarExperiencia(dto);

        // Assert
        using (new AssertionScope())
        {
            experiencia.Should().BeNull();

            NotificatorMock
                .Verify(c => c.Handle(It.IsAny<string>()), Times.Once);

            _experienciaRepositoryMock
                .Verify(c => c.AdicionarExperiencia(It.IsAny<Experiencia>()), Times.Never);
    
            _experienciaRepositoryMock
                .Verify(c => c.UnitOfWork.Commit(), Times.Never);
        }
    }

    [Fact]
    public async Task Adicionar_ErroAoSalvar_Handle()
    {
        // Arrange
        SetupMocks();

        var dto = new AdicionarExperienciaDto
        {
            CurriculoId = 1,
            Cargo = "Desenvolvedor de Software",
            Empresa = "Tech Solutions Ltda",
            Descricao = "Desenvolver aplicativos",
            DataDeInicio = new DateTime(2023, 3, 6),
            DataDeTermino = null,
            TrabalhoAtual = true
        };

        // Act
        var experiencia = await _experienciaService.AdicionarExperiencia(dto);

        // Assert
        using (new AssertionScope())
        {
            experiencia.Should().BeNull();

            NotificatorMock
                .Verify(c => c.Handle(It.IsAny<string>()), Times.Once);

            _experienciaRepositoryMock
                .Verify(c => c.AdicionarExperiencia(It.IsAny<Experiencia>()), Times.Once);

            _experienciaRepositoryMock
                .Verify(c => c.UnitOfWork.Commit(), Times.Once);
        }
    }

    [Fact]
    public async Task Adicionar_SucessoAoSalvar_ExperienciaDto()
    {
        // Arrange
        SetupMocks(commit: true);

        var dto = new AdicionarExperienciaDto
        {
            CurriculoId = 1,
            Cargo = "Desenvolvedor de Software",
            Empresa = "Tech Solutions Ltda",
            Descricao = "Desenvolver aplicativos",
            DataDeInicio = new DateTime(2023, 3, 6),
            DataDeTermino = null,
            TrabalhoAtual = true
        };

        // Act
        var experiencia = await _experienciaService.AdicionarExperiencia(dto);

        // Assert
        using (new AssertionScope())
        { 
            experiencia.Should().NotBeNull();

            NotificatorMock
                .Verify(c => c.Handle(It.IsAny<string>()), Times.Never);

            _experienciaRepositoryMock
                .Verify(c => c.AdicionarExperiencia(It.IsAny<Experiencia>()), Times.Once);

            _experienciaRepositoryMock
                .Verify(c => c.UnitOfWork.Commit(), Times.Once);
        }
    }
    
    [Fact]
    public async Task Adicionar_CurriculoNaoEncontrado_Handle()
    {
        // Arrange
        SetupMocks(experienciaExistente: false);

        var dto = new AdicionarExperienciaDto
        {
            CurriculoId = 2, 
            Cargo = "Desenvolvedor de Software",
            Empresa = "Tech Solutions Ltda",
            Descricao = "Desenvolver aplicativos",
            DataDeInicio = new DateTime(2023, 3, 6),
            DataDeTermino = null,
            TrabalhoAtual = true
        };

        // Act
        var experiencia = await _experienciaService.AdicionarExperiencia(dto);

        // Assert
        using (new AssertionScope())
        {
            experiencia.Should().BeNull();

            NotificatorMock
                .Verify(c => c.Handle("Currículo não encontrado."), Times.Once);

            _experienciaRepositoryMock
                .Verify(c => c.AdicionarExperiencia(It.IsAny<Experiencia>()), Times.Never);

            _experienciaRepositoryMock
                .Verify(c => c.UnitOfWork.Commit(), Times.Never);
        }
    }

    #endregion

    #region AtualizarExperiencia

    [Fact]
    public async Task Atualizar_IdsNaoConferem_Handle()
    {
        //Arrange
        SetupMocks();
        
        const int id = 2;
        var dto = new AtualizarExperienciaDto
        {
            Id = 1
        };
        
        //Act
        var experiencia = await _experienciaService.AtualizarExperiencia(id, dto);

        //Assert
        using (new AssertionScope())
        {
            experiencia.Should().BeNull();
            
            NotificatorMock
                .Verify(c => c.Handle(It.IsAny<string>()), Times.Once);
            NotificatorMock
                .Verify(c => c.Handle(It.IsAny<List<ValidationFailure>>()), Times.Never);
            
            _experienciaRepositoryMock
                .Verify(c => c.AtualizarExperiencia(It.IsAny<Experiencia>()), Times.Never);
            _experienciaRepositoryMock
                .Verify(c => c.UnitOfWork.Commit(), Times.Never);
        }
    }

    [Fact]
    public async Task Atualizar_ExperienciaInexistente_HandleNotFoundResourse()
    {
        //Arrange
        SetupMocks();
        
        const int id = 2;
        var dto = new AtualizarExperienciaDto
        {
            Id = 2
        };
        
        //Act
        var experiencia = await _experienciaService.AtualizarExperiencia(id, dto);

        //Assert
        using (new AssertionScope())
        {
            experiencia.Should().BeNull();
            
            NotificatorMock
                .Verify(c => c.Handle(It.IsAny<string>()), Times.Never);
            NotificatorMock
                .Verify(c => c.Handle(It.IsAny<List<ValidationFailure>>()), Times.Never);
            NotificatorMock.
                Verify(c => c.HandleNotFoundResource(), Times.Once);
            
            _experienciaRepositoryMock
                .Verify(c => c.AtualizarExperiencia(It.IsAny<Experiencia>()), Times.Never);
            _experienciaRepositoryMock
                .Verify(c => c.UnitOfWork.Commit(), Times.Never);
        }
    }
    
    [Fact]
    public async Task Atualizar_ErroAoComitar_Handle()
    {
        //Arrange
        SetupMocks();
        
        const int id = 1;
        var dto = new AtualizarExperienciaDto
        {
            Id = 1,
            Cargo = "Desenvolvedor de Software",
            Empresa = "Tech Solutions Ltda",
            Descricao = "Desenvolver aplicativos",
            DataDeInicio = new DateTime(2023, 3, 6),
            DataDeTermino = null,
            TrabalhoAtual = true
        };
        
        //Act
        var experiencia = await _experienciaService.AtualizarExperiencia(id, dto);
        
        //Assert
        using (new AssertionScope())
        {
            experiencia.Should().BeNull();
            
            NotificatorMock
                .Verify(c => c.Handle(It.IsAny<string>()), Times.Once);
            NotificatorMock
                .Verify(c => c.Handle(It.IsAny<List<ValidationFailure>>()), Times.Never);
            
            _experienciaRepositoryMock
                .Verify(c => c.AtualizarExperiencia(It.IsAny<Experiencia>()), Times.Once);
            _experienciaRepositoryMock
                .Verify(c => c.UnitOfWork.Commit(), Times.Once);
        }
    }
    
    [Fact]
    public async Task Atualizar_ExperienciaInvalida_HandleErros()
    {
        // Arrange
        SetupMocks(true);
        
        const int id = 1;
        var dto = new AtualizarExperienciaDto
        {
            Id = 1,
            Cargo = " ",
            Empresa = " ",
            Descricao = " ",
            DataDeInicio = new DateTime(2023, 3, 6),
            DataDeTermino = null,
            TrabalhoAtual = true
        };
        
        // Act
        var curriculo = await _experienciaService.AtualizarExperiencia( id, dto);

        // Assert
        using (new AssertionScope())
        {
            curriculo.Should().BeNull();

            NotificatorMock
                .Verify(c => c.Handle(It.IsAny<List<ValidationFailure>>()), Times.Once);

            _experienciaRepositoryMock
                .Verify(c => c.AdicionarExperiencia(It.IsAny<Experiencia>()), Times.Never);
        
            _experienciaRepositoryMock
                .Verify(c => c.UnitOfWork.Commit(), Times.Never);
        }
    }
    
    [Fact]
    public async Task Atualizar_SucessoAoComitar_ExperienciaDto()
    {
        //Arrange
        SetupMocks(commit: true);

        const int id = 1;
        var dto = new AtualizarExperienciaDto
        {
            Id = 1,
            Cargo = "Desenvolvedor de Software",
            Empresa = "Tech Solutions Ltda",
            Descricao = "Desenvolver aplicativos",
            DataDeInicio = new DateTime(2023, 3, 6),
            DataDeTermino = null,
            TrabalhoAtual = true
        };
        
        //Act
        var experiencia = await _experienciaService.AtualizarExperiencia(id, dto);
        
        //Assert
        using (new AssertionScope())
        {
            experiencia.Should().NotBeNull();

            NotificatorMock
                .Verify(c => c.Handle(It.IsAny<string>()), Times.Never);
            NotificatorMock
                .Verify(c => c.Handle(It.IsAny<List<ValidationFailure>>()), Times.Never);

            _experienciaRepositoryMock
                .Verify(c => c.AtualizarExperiencia(It.IsAny<Experiencia>()), Times.Once);
            _experienciaRepositoryMock
                .Verify(c => c.UnitOfWork.Commit(), Times.Once);
        }
    }

    #endregion

    #region DeletarExperiencia

    [Fact]
    public async Task DeletarExperiencia_ExperienciaInexistente_HandleNotFoundResource()
    {
        // Arrange
        SetupMocks(experienciaExistente: false);

        const int id = 2;

        // Act
        await _experienciaService.DeletarExperiencia(id);

        // Assert
        using (new AssertionScope())
        {
            NotificatorMock.Verify(c => c.HandleNotFoundResource(), Times.Once);
        
            _experienciaRepositoryMock.Verify(c => c.RemoverExperiencia(It.IsAny<Experiencia>()), Times.Never);
            _experienciaRepositoryMock.Verify(c => c.UnitOfWork.Commit(), Times.Never);
        }
    }
    
    [Fact]
    public async Task Deletar_ErroAoSalvar_Handle()
    {
        // Arrange
        SetupMocks();

        const int id = 1;

        // Act
        await _experienciaService.DeletarExperiencia(id);

        // Assert
        using (new AssertionScope())
        {
            NotificatorMock
                .Verify(c => c.Handle(It.IsAny<string>()), Times.Once);

            _experienciaRepositoryMock
                .Verify(c => c.RemoverExperiencia(It.IsAny<Experiencia>()), Times.Once);

            _experienciaRepositoryMock
                .Verify(c => c.UnitOfWork.Commit(), Times.Once);
        }
    }

    [Fact]
    public async Task DeletarExperiencia_ExperienciaExistente_Sucesso()
    {
        // Arrange
        SetupMocks(experienciaExistente: true, commit: true);

        const int id = 1;

        // Act
        await _experienciaService.DeletarExperiencia(id);

        // Assert
        using (new AssertionScope())
        {
            NotificatorMock.Verify(c => c.HandleNotFoundResource(), Times.Never);
            NotificatorMock.Verify(c => c.Handle(It.IsAny<string>()), Times.Never);
        
            _experienciaRepositoryMock.Verify(c => c.RemoverExperiencia(It.IsAny<Experiencia>()), Times.Once);
            _experienciaRepositoryMock.Verify(c => c.UnitOfWork.Commit(), Times.Once);
        }
    }


    #endregion

    #region ObterExperiencia
    
    [Fact]
    public async Task ObterExperienciaPorCurriculoId_CurriculoSemExperiencias_RetornaNull()
    {
        // Arrange
        SetupMocks(experienciaExistente: false);

        const int id = 1;

        // Act
        var experiencia = await _experienciaService.ObterExperienciaPorCurriculoId(id);

        // Assert
        using (new AssertionScope())
        {
            experiencia.Should().BeNull();
            NotificatorMock.Verify(c => c.HandleNotFoundResource(), Times.Once);
        }
    }
    
    [Fact]
    public async Task ObterExperienciaPorCurriculoId_CurriculoExistente_Sucesso()
    {
        // Arrange
        SetupMocks(experienciaExistente: true);

        const int id = 1;

        // Act
        var experiencia = await _experienciaService.ObterExperienciaPorCurriculoId(id);

        //Assert
        using (new AssertionScope())
        {
            experiencia.Should().NotBeNull();
            experiencia.Should().BeOfType<List<ExperienciaDto>>();
            NotificatorMock.Verify(c => c.HandleNotFoundResource(), Times.Never);
        }
    }

    #endregion

    #region SetupMocks

    private void SetupMocks(bool experienciaExistente = false, bool commit = false)
    {
        var curriculo = new Curriculo
        {
            Id = 1
        };
        
        var experiencia = new Experiencia
        {
            CurriculoId = 1,
            Cargo = "Desenvolvedor de Software",
            Empresa = "Tech Solutions Ltda",
            Descricao = "Desenvolver aplicativos",
            DataDeInicio = new DateTime(2023, 3, 6),
            DataDeTermino = null,
            TrabalhoAtual = true
        };
        
        _experienciaRepositoryMock
            .Setup(c => c.FirstOrDefault(It.IsAny<Expression<Func<Experiencia, bool>>>()))
            .ReturnsAsync(experienciaExistente ? experiencia : null);

        _experienciaRepositoryMock
            .Setup(c => c.UnitOfWork.Commit())
            .ReturnsAsync(commit);
    
        _experienciaRepositoryMock
            .Setup(c => c.ObterExperienciaPorId(It.Is<int>(id => id == 1)))
            .ReturnsAsync(experiencia);

        var experiencias = experienciaExistente ? new List<Experiencia> { experiencia } : new List<Experiencia>();

        _experienciaRepositoryMock
            .Setup(c => c.ObterExperienciaDoCurriculo(It.IsAny<int>()))
            .ReturnsAsync((int curriculoId) => experiencias.Where(e => e.CurriculoId == curriculoId).ToList());
        
        _curriculoRepositoryMock
            .Setup(c => c.ObterCurriculoPorId(It.Is<int>(id => id == 1)))
            .ReturnsAsync(curriculo);
    }

    #endregion
}