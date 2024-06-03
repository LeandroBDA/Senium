using System.Linq.Expressions;
using FluentAssertions;
using FluentAssertions.Execution;
using FluentValidation.Results;
using Microsoft.AspNetCore.Http;
using Moq;
using Senium.Application.Contracts.Services;
using Senium.Application.Dto.V1.Curriculo;
using Senium.Application.Services;
using Senium.Application.Tests.Fixtures;
using Senium.Core.Enums;
using Senium.Domain.Contracts.Repositories;
using Senium.Domain.Entities;

namespace Senium.Application.Tests.Service;

public class CurriculoServiceTest : BaseServiceTest, IClassFixture<ServicesFixture>
{
    private readonly CurriculoService _curriculoService;
    private readonly Mock<ICurriculoRepository> _curriculoRepositoryMock = new();
    private readonly Mock<IHttpContextAccessor> _httpContextAccessorMock = new();
    private readonly Mock<IFileService> _fileServiceMock = new();

    public CurriculoServiceTest(ServicesFixture fixture)
    {
        _curriculoService = new CurriculoService(NotificatorMock.Object, fixture.Mapper,_httpContextAccessorMock.Object,
            _curriculoRepositoryMock.Object, _fileServiceMock.Object);
    }

    #region ObterCurriculoPorId

    [Fact]
    public async Task ObterPorId_CurriculoInexistente_HandleNotFoundResource()
    {
        // Arrange
        SetupMocks();

        const int id = 2;
    
        // Act
        var curriculoDto = await _curriculoService.ObterCurriculoPorId(id);

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
        var curriculoDto = await _curriculoService.ObterCurriculoPorId(id);

        // Assert
        using (new AssertionScope())
        {
            curriculoDto.Should().NotBeNull();
            curriculoDto.Should().BeOfType<CurriculoDto>();
            NotificatorMock.Verify(c => c.HandleNotFoundResource(), Times.Never);
        }
    }


    #endregion
    
    #region AdicionarCurriculo

    [Fact]
    public async Task Adicionar_CurriculoInvalida_HandleErros()
    {
        // Arrange
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
            EPessoaComDeficiencia = false,
            EDeficienciaAuditiva = false,
            EDeficienciaFisica = false,
            EDeficienciaIntelectual = false,
            EDeficienciaMotora = false,
            EDeficienciaVisual = false,
            ELgbtqia = false,
            EBaixaRenda = false,
            Titulo = "",
            AreaDeAtuacao = "",
            ResumoProfissional = "",
            Linkedin = "invalid_url",
            Portfolio = "invalid_url",
            Clt = false,
            Pj = false,
            Temporario = false,
            Presencial = false,
            Remoto = false,
            Hibrido = false
        };


        // Act
        var curriculo = await _curriculoService.AdicionarCurriculo(dto);

        // Assert
        using (new AssertionScope())
        {
            curriculo.Should().BeNull();

            NotificatorMock
                .Verify(c => c.Handle(It.IsAny<List<ValidationFailure>>()), Times.Once);

            _curriculoRepositoryMock
                .Verify(c => c.AdicionarCurriculo(It.IsAny<Curriculo>()), Times.Never);
        
            _curriculoRepositoryMock
                .Verify(c => c.UnitOfWork.Commit(), Times.Never);
        }
    }

    [Fact]
    public async Task Adicionar_CurriculoExistente_Handle()
    {
        // Arrange
        SetupMocks(curriculoExistente: true);

        var dto = new AdicionarCurriculoDto
        {
            Telefone = "85 988888888",
            EstadoCivil = "Solteiro",
            Genero = "Masculino",
            RacaEtnia = "Branco",
            GrauDeFormacao = "Graduação",
            Cep = "12345678",
            Endereco = "Rua Teste, 123",
            Cidade = "Cidade Teste",
            Estado = "CE",
            EPessoaComDeficiencia = false,
            EDeficienciaAuditiva = false,
            EDeficienciaFisica = false,
            EDeficienciaIntelectual = false,
            EDeficienciaMotora = false,
            EDeficienciaVisual = false,
            ELgbtqia = false,
            EBaixaRenda = false,
            Titulo = "Desenvolvedor Full Stack",
            AreaDeAtuacao = "Tecnologia da Informação",
            ResumoProfissional = "Profissional com experiência em desenvolvimento web.",
            Linkedin = "https://www.linkedin.com/in/seuperfil",
            Portfolio = "https://www.portfolio.com",
            Clt = true,
            Pj = false,
            Temporario = false,
            Presencial = true,
            Remoto = false,
            Hibrido = false
        };

        // Act
        var curriculo = await _curriculoService.AdicionarCurriculo(dto);

        // Assert
        using (new AssertionScope())
        {
            curriculo.Should().BeNull();

            NotificatorMock
                .Verify(c => c.Handle(It.IsAny<string>()), Times.Once);

            _curriculoRepositoryMock
                .Verify(c => c.AdicionarCurriculo(It.IsAny<Curriculo>()), Times.Never);
    
            _curriculoRepositoryMock
                .Verify(c => c.UnitOfWork.Commit(), Times.Never);
        }
    }

    [Fact]
    public async Task Adicionar_ErroAoSalvar_Handle()
    {
        // Arrange
        SetupMocks();

        var dto = new AdicionarCurriculoDto
        {
            Telefone = "85 988888888",
            EstadoCivil = "Solteiro",
            Genero = "Masculino",
            RacaEtnia = "Branco",
            GrauDeFormacao = "Graduação",
            Cep = "12345678",
            Endereco = "Rua Teste, 123",
            Cidade = "Cidade Teste",
            Estado = "CE",
            EPessoaComDeficiencia = false,
            EDeficienciaAuditiva = false,
            EDeficienciaFisica = false,
            EDeficienciaIntelectual = false,
            EDeficienciaMotora = false,
            EDeficienciaVisual = false,
            ELgbtqia = false,
            EBaixaRenda = false,
            Titulo = "Desenvolvedor Full Stack",
            AreaDeAtuacao = "Tecnologia da Informação",
            ResumoProfissional = "Profissional com experiência em desenvolvimento web.",
            Linkedin = "https://www.linkedin.com/in/teste/",
            Portfolio = "https://www.portfolio.com",
            Clt = true,
            Pj = false,
            Temporario = false,
            Presencial = true,
            Remoto = false,
            Hibrido = false
        };

        // Act
        var curriculo = await _curriculoService.AdicionarCurriculo(dto);

        // Assert
        using (new AssertionScope())
        {
            curriculo.Should().BeNull();

            NotificatorMock
                .Verify(c => c.Handle(It.IsAny<string>()), Times.Once);

            _curriculoRepositoryMock
                .Verify(c => c.AdicionarCurriculo(It.IsAny<Curriculo>()), Times.Once);

            _curriculoRepositoryMock
                .Verify(c => c.UnitOfWork.Commit(), Times.Once);
        }
    }

    [Fact]
    public async Task Adicionar_SucessoAoSalvar_CurriculoDto()
    {
        // Arrange
        SetupMocks(commit: true);

        var dto = new AdicionarCurriculoDto
        {
            Telefone = "85 988888888",
            EstadoCivil = "Solteiro",
            Genero = "Masculino",
            RacaEtnia = "Branco",
            GrauDeFormacao = "Graduação",
            Cep = "12345678",
            Endereco = "Rua Teste, 123",
            Cidade = "Cidade Teste",
            Estado = "CE",
            EPessoaComDeficiencia = false,
            EDeficienciaAuditiva = false,
            EDeficienciaFisica = false,
            EDeficienciaIntelectual = false,
            EDeficienciaMotora = false,
            EDeficienciaVisual = false,
            ELgbtqia = false,
            EBaixaRenda = false,
            Titulo = "Desenvolvedor Full Stack",
            AreaDeAtuacao = "Tecnologia da Informação",
            ResumoProfissional = "Profissional com experiência em desenvolvimento web.",
            Linkedin = "https://www.linkedin.com/in/teste/",
            Portfolio = "https://www.portfolio.com",
            Clt = true,
            Pj = false,
            Temporario = false,
            Presencial = true,
            Remoto = false,
            Hibrido = false
        };

        // Act
        var curriculo = await _curriculoService.AdicionarCurriculo(dto);

        // Assert
        using (new AssertionScope())
        { 
            curriculo.Should().NotBeNull();

            NotificatorMock
                .Verify(c => c.Handle(It.IsAny<string>()), Times.Never);

            _curriculoRepositoryMock
                .Verify(c => c.AdicionarCurriculo(It.IsAny<Curriculo>()), Times.Once);

            _curriculoRepositoryMock
                .Verify(c => c.UnitOfWork.Commit(), Times.Once);
        }
    }
    
    [Fact]
    public async Task Adicionar_SucessoComPdf_CurriculoDto()
    {
        SetupMocks(false, true);

        var file = new Mock<IFormFile>();
        file.Setup(c => c.Length).Returns(56);
        var dto = new AdicionarCurriculoDto
        {
            Telefone = "85 988888888",
            EstadoCivil = "Solteiro",
            Genero = "Masculino",
            RacaEtnia = "Branco",
            GrauDeFormacao = "Graduação",
            Cep = "12345678",
            Endereco = "Rua Teste, 123",
            Cidade = "Cidade Teste",
            Estado = "CE",
            EPessoaComDeficiencia = false,
            EDeficienciaAuditiva = false,
            EDeficienciaFisica = false,
            EDeficienciaIntelectual = false,
            EDeficienciaMotora = false,
            EDeficienciaVisual = false,
            ELgbtqia = false,
            EBaixaRenda = false,
            Titulo = "Desenvolvedor Full Stack",
            AreaDeAtuacao = "Tecnologia da Informação",
            ResumoProfissional = "Profissional com experiência em desenvolvimento web.",
            Linkedin = "https://www.linkedin.com/in/teste/",
            Portfolio = "https://www.portfolio.com",
            Pdf = file.Object,
            Clt = true,
            Pj = false,
            Temporario = false,
            Presencial = true,
            Remoto = false,
            Hibrido = false
        };

        var curriculo = await _curriculoService.AdicionarCurriculo(dto);

        using (new AssertionScope())
        {
            curriculo.Should().NotBeNull();
            
            NotificatorMock
                .Verify(c => c.Handle(It.IsAny<List<ValidationFailure>>()), Times.Never);
            NotificatorMock
                .Verify(c => c.Handle(It.IsAny<string>()), Times.Never);
            
            _curriculoRepositoryMock.Verify(c => c.AdicionarCurriculo(It.IsAny<Curriculo>()), Times.Once);
            _curriculoRepositoryMock.Verify(c => c.UnitOfWork.Commit(), Times.Once);
        }
    }
    
    [Fact]
    public async Task Adicionar_SucessoSemPdf_CurriculoDto()
    {
        SetupMocks(false, true);

        var dto = new AdicionarCurriculoDto
        {
            Telefone = "85 988888888",
            EstadoCivil = "Solteiro",
            Genero = "Masculino",
            RacaEtnia = "Branco",
            GrauDeFormacao = "Graduação",
            Cep = "12345678",
            Endereco = "Rua Teste, 123",
            Cidade = "Cidade Teste",
            Estado = "CE",
            EPessoaComDeficiencia = false,
            EDeficienciaAuditiva = false,
            EDeficienciaFisica = false,
            EDeficienciaIntelectual = false,
            EDeficienciaMotora = false,
            EDeficienciaVisual = false,
            ELgbtqia = false,
            EBaixaRenda = false,
            Titulo = "Desenvolvedor Full Stack",
            AreaDeAtuacao = "Tecnologia da Informação",
            ResumoProfissional = "Profissional com experiência em desenvolvimento web.",
            Linkedin = "https://www.linkedin.com/in/teste/",
            Portfolio = "https://www.portfolio.com",
            Clt = true,
            Pj = false,
            Temporario = false,
            Presencial = true,
            Remoto = false,
            Hibrido = false
        };

        var curriculo = await _curriculoService.AdicionarCurriculo(dto);

        using (new AssertionScope())
        {
            curriculo.Should().NotBeNull();
            
            NotificatorMock
                .Verify(c => c.Handle(It.IsAny<List<ValidationFailure>>()), Times.Never);
            NotificatorMock
                .Verify(c => c.Handle(It.IsAny<string>()), Times.Never);
            
            _curriculoRepositoryMock.Verify(c => c.AdicionarCurriculo(It.IsAny<Curriculo>()), Times.Once);
            _curriculoRepositoryMock.Verify(c => c.UnitOfWork.Commit(), Times.Once);
        }
    }
    
    [Fact]
    public async Task Adicionar_PdfMaiorQue5MB_DeveRetornarErro()
    {
        SetupMocks(false, true);

        var file = new Mock<IFormFile>();
        file.Setup(c => c.Length).Returns(6 * 1024 * 1024); // 6 MB
        var dto = new AdicionarCurriculoDto
        {
            Telefone = "85 988888888",
            EstadoCivil = "Solteiro",
            Genero = "Masculino",
            RacaEtnia = "Branco",
            GrauDeFormacao = "Graduação",
            Cep = "12345678",
            Endereco = "Rua Teste, 123",
            Cidade = "Cidade Teste",
            Estado = "CE",
            EPessoaComDeficiencia = false,
            EDeficienciaAuditiva = false,
            EDeficienciaFisica = false,
            EDeficienciaIntelectual = false,
            EDeficienciaMotora = false,
            EDeficienciaVisual = false,
            ELgbtqia = false,
            EBaixaRenda = false,
            Titulo = "Desenvolvedor Full Stack",
            AreaDeAtuacao = "Tecnologia da Informação",
            ResumoProfissional = "Profissional com experiência em desenvolvimento web.",
            Linkedin = "https://www.linkedin.com/in/teste/",
            Portfolio = "https://www.portfolio.com",
            Pdf = file.Object,
            Clt = true,
            Pj = false,
            Temporario = false,
            Presencial = true,
            Remoto = false,
            Hibrido = false
        };

        var curriculo = await _curriculoService.AdicionarCurriculo(dto);

        using (new AssertionScope())
        {
            curriculo.Should().BeNull();
        
            NotificatorMock
                .Verify(c => c.Handle(It.Is<string>(msg => msg == "O arquivo deve ter no máximo 5 MB.")), Times.Once);
        
            _curriculoRepositoryMock.Verify(c => c.AdicionarCurriculo(It.IsAny<Curriculo>()), Times.Never);
            _curriculoRepositoryMock.Verify(c => c.UnitOfWork.Commit(), Times.Never);
        }
    }

    [Fact]
    public async Task Adicionar_ErroAoFazerUploadPdf_DeveRetornarErro()
    {
        SetupMocks(false, true);

        var file = new Mock<IFormFile>();
        file.Setup(c => c.Length).Returns(56);
        _fileServiceMock.Setup(f => f.UploadPdf(It.IsAny<IFormFile>(), It.IsAny<EUploadPath>(), It.IsAny<EPathAccess>(), It.IsAny<int>()))
            .ThrowsAsync(new Exception("Erro simulado ao fazer upload"));

        var dto = new AdicionarCurriculoDto
        {
            Telefone = "85 988888888",
            EstadoCivil = "Solteiro",
            Genero = "Masculino",
            RacaEtnia = "Branco",
            GrauDeFormacao = "Graduação",
            Cep = "12345678",
            Endereco = "Rua Teste, 123",
            Cidade = "Cidade Teste",
            Estado = "CE",
            EPessoaComDeficiencia = false,
            EDeficienciaAuditiva = false,
            EDeficienciaFisica = false,
            EDeficienciaIntelectual = false,
            EDeficienciaMotora = false,
            EDeficienciaVisual = false,
            ELgbtqia = false,
            EBaixaRenda = false,
            Titulo = "Desenvolvedor Full Stack",
            AreaDeAtuacao = "Tecnologia da Informação",
            ResumoProfissional = "Profissional com experiência em desenvolvimento web.",
            Linkedin = "https://www.linkedin.com/in/teste/",
            Portfolio = "https://www.portfolio.com",
            Pdf = file.Object,
            Clt = true,
            Pj = false,
            Temporario = false,
            Presencial = true,
            Remoto = false,
            Hibrido = false
        };

        var curriculo = await _curriculoService.AdicionarCurriculo(dto);

        using (new AssertionScope())
        {
            curriculo.Should().BeNull();
        
            NotificatorMock
                .Verify(c => c.Handle(It.Is<string>(msg => msg.Contains("Erro ao fazer upload do PDF: Erro simulado ao fazer upload"))), Times.Once);
        
            _curriculoRepositoryMock.Verify(c => c.AdicionarCurriculo(It.IsAny<Curriculo>()), Times.Never);
            _curriculoRepositoryMock.Verify(c => c.UnitOfWork.Commit(), Times.Never);
        }
    }


    #endregion

    #region AtualizarCurriculo

    [Fact]
    public async Task Atualizar_IdsNaoConferem_Handle()
    {
        //Arrange
        SetupMocks();
        
        const int id = 2;
        var dto = new AtualizarCurriculoDto
        {
            Id = 1
        };
        
        //Act
        var curriculo = await _curriculoService.AtualizarCurriculo(id, dto);

        //Assert
        using (new AssertionScope())
        {
            curriculo.Should().BeNull();
            
            NotificatorMock
                .Verify(c => c.Handle(It.IsAny<string>()), Times.Once);
            NotificatorMock
                .Verify(c => c.Handle(It.IsAny<List<ValidationFailure>>()), Times.Never);
            
            _curriculoRepositoryMock
                .Verify(c => c.AtualizarCurriculo(It.IsAny<Curriculo>()), Times.Never);
            _curriculoRepositoryMock
                .Verify(c => c.UnitOfWork.Commit(), Times.Never);
        }
    }

    [Fact]
    public async Task Atualizar_CurriculoInexistente_HandleNotFoundResourse()
    {
        //Arrange
        SetupMocks();
        
        const int id = 2;
        var dto = new AtualizarCurriculoDto
        {
            Id = 2
        };
        
        //Act
        var curriculo = await _curriculoService.AtualizarCurriculo(id, dto);

        //Assert
        using (new AssertionScope())
        {
            curriculo.Should().BeNull();
            
            NotificatorMock
                .Verify(c => c.Handle(It.IsAny<string>()), Times.Never);
            NotificatorMock
                .Verify(c => c.Handle(It.IsAny<List<ValidationFailure>>()), Times.Never);
            NotificatorMock.
                Verify(c => c.HandleNotFoundResource(), Times.Once);
            
            _curriculoRepositoryMock
                .Verify(c => c.AtualizarCurriculo(It.IsAny<Curriculo>()), Times.Never);
            _curriculoRepositoryMock
                .Verify(c => c.UnitOfWork.Commit(), Times.Never);
        }
    }
    
    [Fact]
    public async Task Atualizar_ErroAoComitar_Handle()
    {
        //Arrange
        SetupMocks();
        
        const int id = 1;
        var dto = new AtualizarCurriculoDto
        {
            Id = 1,
            Telefone = "85 988888888",
            EstadoCivil = "Solteiro",
            Genero = "Masculino",
            RacaEtnia = "Branco",
            GrauDeFormacao = "Graduação",
            Cep = "12345678",
            Endereco = "Rua Teste, 123",
            Cidade = "Cidade Teste",
            Estado = "CE",
            EPessoaComDeficiencia = false,
            EDeficienciaAuditiva = false,
            EDeficienciaFisica = false,
            EDeficienciaIntelectual = false,
            EDeficienciaMotora = false,
            EDeficienciaVisual = false,
            ELgbtqia = false,
            EBaixaRenda = false,
            Titulo = "Desenvolvedor Full Stack",
            AreaDeAtuacao = "Tecnologia da Informação",
            ResumoProfissional = "Profissional com experiência em desenvolvimento web.",
            Linkedin = "https://www.linkedin.com/in/seuperfil/",
            Portfolio = "https://www.portfolio.com",
            Clt = true,
            Pj = false,
            Temporario = false,
            Presencial = true,
            Remoto = false,
            Hibrido = false
        };
        
        //Act
        var curriculo = await _curriculoService.AtualizarCurriculo(id, dto);
        
        //Assert
        using (new AssertionScope())
        {
            curriculo.Should().BeNull();
            
            NotificatorMock
                .Verify(c => c.Handle(It.IsAny<string>()), Times.Once);
            NotificatorMock
                .Verify(c => c.Handle(It.IsAny<List<ValidationFailure>>()), Times.Never);
            
            _curriculoRepositoryMock
                .Verify(c => c.AtualizarCurriculo(It.IsAny<Curriculo>()), Times.Once);
            _curriculoRepositoryMock
                .Verify(c => c.UnitOfWork.Commit(), Times.Once);
        }
    }
    
    [Fact]
    public async Task Atualizar_CurriculoInvalida_HandleErros()
    {
        // Arrange
        SetupMocks(true);
        
        const int id = 1;
        var dto = new AtualizarCurriculoDto
        {
            Id = 1,
            Telefone = "85 988888888",
            EstadoCivil = "Solteiro",
            Genero = "Masculino",
            RacaEtnia = "Branco",
            GrauDeFormacao = "Graduação",
            Cep = "12345678",
            Endereco = "Rua Teste, 123",
            Cidade = "Cidade Teste",
            Estado = "CE",
            EPessoaComDeficiencia = false,
            EDeficienciaAuditiva = false,
            EDeficienciaFisica = false,
            EDeficienciaIntelectual = false,
            EDeficienciaMotora = false,
            EDeficienciaVisual = false,
            ELgbtqia = false,
            EBaixaRenda = false,
            Titulo = "Desenvolvedor Full Stack",
            AreaDeAtuacao = "Tecnologia da Informação",
            ResumoProfissional = "Profissional com experiência em desenvolvimento web.",
            Linkedin = "invalid-url", 
            Portfolio = "https://www.portfolio.com",
            Clt = true,
            Pj = false,
            Temporario = false,
            Presencial = true,
            Remoto = false,
            Hibrido = false
        };
        
        // Act
        var curriculo = await _curriculoService.AtualizarCurriculo( id, dto);

        // Assert
        using (new AssertionScope())
        {
            curriculo.Should().BeNull();

            NotificatorMock
                .Verify(c => c.Handle(It.IsAny<List<ValidationFailure>>()), Times.Once);

            _curriculoRepositoryMock
                .Verify(c => c.AdicionarCurriculo(It.IsAny<Curriculo>()), Times.Never);
        
            _curriculoRepositoryMock
                .Verify(c => c.UnitOfWork.Commit(), Times.Never);
        }
    }
    
    [Fact]
    public async Task Atualizar_SucessoAoComitar_CurriculoDto()
    {
        //Arrange
        SetupMocks(commit: true);

        const int id = 1;
        var dto = new AtualizarCurriculoDto
        {
            Id = 1,
            Telefone = "85 988888888",
            EstadoCivil = "Solteiro",
            Genero = "Masculino",
            RacaEtnia = "Branco",
            GrauDeFormacao = "Graduação",
            Cep = "12345678",
            Endereco = "Rua Teste, 123",
            Cidade = "Cidade Teste",
            Estado = "CE",
            EPessoaComDeficiencia = false,
            EDeficienciaAuditiva = false,
            EDeficienciaFisica = false,
            EDeficienciaIntelectual = false,
            EDeficienciaMotora = false,
            EDeficienciaVisual = false,
            ELgbtqia = false,
            EBaixaRenda = false,
            Titulo = "Desenvolvedor Full Stack",
            AreaDeAtuacao = "Tecnologia da Informação",
            ResumoProfissional = "Profissional com experiência em desenvolvimento web.",
            Linkedin = "https://www.linkedin.com/in/seuperfil",
            Portfolio = "https://www.portfolio.com",
            Clt = true,
            Pj = false,
            Temporario = false,
            Presencial = true,
            Remoto = false,
            Hibrido = false
        };
        
        //Act
        var curriculo = await _curriculoService.AtualizarCurriculo(id, dto);
        
        //Assert
        using (new AssertionScope())
        {
            curriculo.Should().NotBeNull();

            NotificatorMock
                .Verify(c => c.Handle(It.IsAny<string>()), Times.Never);
            NotificatorMock
                .Verify(c => c.Handle(It.IsAny<List<ValidationFailure>>()), Times.Never);

            _curriculoRepositoryMock
                .Verify(c => c.AtualizarCurriculo(It.IsAny<Curriculo>()), Times.Once);
            _curriculoRepositoryMock
                .Verify(c => c.UnitOfWork.Commit(), Times.Once);
        }
    }

    [Fact]
    public async Task Atualizar_CurriculoComPdf_Sucesso()
    {
        SetupMocks(commit: true);
        
        var file = new Mock<IFormFile>();
        file.Setup(c => c.Length).Returns(56);
        const int id = 1;
        var dto = new AtualizarCurriculoDto
        {
            Id = 1,
            Telefone = "85 988888888",
            EstadoCivil = "Solteiro",
            Genero = "Masculino",
            RacaEtnia = "Branco",
            GrauDeFormacao = "Graduação",
            Cep = "12345678",
            Endereco = "Rua Teste, 123",
            Cidade = "Cidade Teste",
            Estado = "CE",
            EPessoaComDeficiencia = false,
            EDeficienciaAuditiva = false,
            EDeficienciaFisica = false,
            EDeficienciaIntelectual = false,
            EDeficienciaMotora = false,
            EDeficienciaVisual = false,
            ELgbtqia = false,
            EBaixaRenda = false,
            Titulo = "Desenvolvedor Full Stack",
            AreaDeAtuacao = "Tecnologia da Informação",
            ResumoProfissional = "Profissional com experiência em desenvolvimento web.",
            Linkedin = "https://www.linkedin.com/in/seuperfil",
            Portfolio = "https://www.portfolio.com",
            Pdf = file.Object,
            Clt = true,
            Pj = false,
            Temporario = false,
            Presencial = true,
            Remoto = false,
            Hibrido = false
        };
        
        var curriculo = await _curriculoService.AtualizarCurriculo(id, dto);

        using (new AssertionScope())
        {
            curriculo.Should().NotBeNull();
            
            NotificatorMock.Verify(c => c.Handle(It.IsAny<string>()), Times.Never);
            NotificatorMock.Verify(c => c.Handle(It.IsAny<List<ValidationFailure>>()), Times.Never);
            _curriculoRepositoryMock.Verify(c => c.AtualizarCurriculo(It.IsAny<Curriculo>()), Times.Once);
            _curriculoRepositoryMock.Verify(c => c.UnitOfWork.Commit(), Times.Once);
        }
    }
    
    [Fact]
    public async Task Atualizar_CurriculoSemPdf_Sucesso()
    {
        SetupMocks(commit: true);
        
        const int id = 1;
        var dto = new AtualizarCurriculoDto
        {
            Id = 1,
            Telefone = "85 988888888",
            EstadoCivil = "Solteiro",
            Genero = "Masculino",
            RacaEtnia = "Branco",
            GrauDeFormacao = "Graduação",
            Cep = "12345678",
            Endereco = "Rua Teste, 123",
            Cidade = "Cidade Teste",
            Estado = "CE",
            EPessoaComDeficiencia = false,
            EDeficienciaAuditiva = false,
            EDeficienciaFisica = false,
            EDeficienciaIntelectual = false,
            EDeficienciaMotora = false,
            EDeficienciaVisual = false,
            ELgbtqia = false,
            EBaixaRenda = false,
            Titulo = "Desenvolvedor Full Stack",
            AreaDeAtuacao = "Tecnologia da Informação",
            ResumoProfissional = "Profissional com experiência em desenvolvimento web.",
            Linkedin = "https://www.linkedin.com/in/seuperfil",
            Portfolio = "https://www.portfolio.com",
            Clt = true,
            Pj = false,
            Temporario = false,
            Presencial = true,
            Remoto = false,
            Hibrido = false
        };
        
        var curriculo = await _curriculoService.AtualizarCurriculo(id, dto);

        using (new AssertionScope())
        {
            curriculo.Should().NotBeNull();
            
            NotificatorMock.Verify(c => c.Handle(It.IsAny<string>()), Times.Never);
            NotificatorMock.Verify(c => c.Handle(It.IsAny<List<ValidationFailure>>()), Times.Never);
            _curriculoRepositoryMock.Verify(c => c.AtualizarCurriculo(It.IsAny<Curriculo>()), Times.Once);
            _curriculoRepositoryMock.Verify(c => c.UnitOfWork.Commit(), Times.Once);
        }
    }
    
    [Fact]
public async Task Atualizar_CurriculoComPdf_MaiorQueLimite_NotificacaoErro()
{
    // Arrange
    SetupMocks(commit: false);
    var file = new Mock<IFormFile>();
    file.Setup(c => c.Length).Returns(6 * 1024 * 1024 + 1); // Define um tamanho maior que 5MB
    const int id = 1;
    var dto = new AtualizarCurriculoDto
    {
        Id = id,
        Telefone = "85 988888888",
        EstadoCivil = "Solteiro",
        Genero = "Masculino",
        RacaEtnia = "Branco",
        GrauDeFormacao = "Graduação",
        Cep = "12345678",
        Endereco = "Rua Teste, 123",
        Cidade = "Cidade Teste",
        Estado = "CE",
        EPessoaComDeficiencia = false,
        EDeficienciaAuditiva = false,
        EDeficienciaFisica = false,
        EDeficienciaIntelectual = false,
        EDeficienciaMotora = false,
        EDeficienciaVisual = false,
        ELgbtqia = false,
        EBaixaRenda = false,
        Titulo = "Desenvolvedor Full Stack",
        AreaDeAtuacao = "Tecnologia da Informação",
        ResumoProfissional = "Profissional com experiência em desenvolvimento web.",
        Linkedin = "https://www.linkedin.com/in/seuperfil",
        Portfolio = "https://www.portfolio.com",
        Pdf = file.Object,
        Clt = true,
        Pj = false,
        Temporario = false,
        Presencial = true,
        Remoto = false,
        Hibrido = false
    };

    // Act
    var curriculo = await _curriculoService.AtualizarCurriculo(id, dto);

    // Assert
    using (new AssertionScope())
    {
        curriculo.Should().BeNull(); 
        NotificatorMock.Verify(c => c.Handle(It.Is<string>(msg => msg.Contains("O arquivo deve ter no máximo"))), Times.Once); 
        _curriculoRepositoryMock.Verify(c => c.AtualizarCurriculo(It.IsAny<Curriculo>()), Times.Never); 
        _curriculoRepositoryMock.Verify(c => c.UnitOfWork.Commit(), Times.Never); 
    }
}

    
    [Fact]
    public async Task Atualizar_Curriculo_TamanhoPdfExcedido_NotificacaoErro()
    {
        // Arrange
        SetupMocks(commit: false);
        var maxSizeInBytes = 5 * 1024 * 1024; // 5 MB
        var file = new Mock<IFormFile>();
        file.Setup(c => c.Length).Returns(maxSizeInBytes + 1); // Excede o limite de tamanho
        const int id = 1;
        var dto = new AtualizarCurriculoDto
        {
            Id = id,
            Telefone = "85 988888888",
            EstadoCivil = "Solteiro",
            Genero = "Masculino",
            RacaEtnia = "Branco",
            GrauDeFormacao = "Graduação",
            Cep = "12345678",
            Endereco = "Rua Teste, 123",
            Cidade = "Cidade Teste",
            Estado = "CE",
            EPessoaComDeficiencia = false,
            EDeficienciaAuditiva = false,
            EDeficienciaFisica = false,
            EDeficienciaIntelectual = false,
            EDeficienciaMotora = false,
            EDeficienciaVisual = false,
            ELgbtqia = false,
            EBaixaRenda = false,
            Titulo = "Desenvolvedor Full Stack",
            AreaDeAtuacao = "Tecnologia da Informação",
            ResumoProfissional = "Profissional com experiência em desenvolvimento web.",
            Linkedin = "https://www.linkedin.com/in/seuperfil",
            Portfolio = "https://www.portfolio.com",
            Pdf = file.Object,
            Clt = true,
            Pj = false,
            Temporario = false,
            Presencial = true,
            Remoto = false,
            Hibrido = false
        };

        // Act
        var curriculo = await _curriculoService.AtualizarCurriculo(id, dto);

        // Assert
        using (new AssertionScope())
        {
            curriculo.Should().BeNull();
            NotificatorMock.Verify(c => c.Handle($"O arquivo deve ter no máximo {maxSizeInBytes / (1024 * 1024)} MB."), Times.Once);
            _curriculoRepositoryMock.Verify(c => c.AtualizarCurriculo(It.IsAny<Curriculo>()), Times.Never);
            _curriculoRepositoryMock.Verify(c => c.UnitOfWork.Commit(), Times.Never);
        }
    }
    
    [Fact]
    public async Task AtualizarCurriculo_PdfExcedeTamanhoMaximo_NotificacaoErro()
    {
        // Arrange
        SetupMocks(commit: false);
        var file = new Mock<IFormFile>();
        file.Setup(c => c.Length).Returns(6 * 1024 * 1024); // Define um tamanho maior que o permitido
        const int id = 1;
        var dto = new AtualizarCurriculoDto
        {
            Id = id,
            Pdf = file.Object // Este é o objeto do arquivo PDF, fornecido pelo mock
        };

        // Act
        var curriculo = await _curriculoService.AtualizarCurriculo(id, dto);

        // Assert
        using (new AssertionScope())
        {
            curriculo.Should().BeNull();
            NotificatorMock.Verify(c => c.Handle(It.Is<string>(msg => msg.Contains("O arquivo deve ter no máximo 5 MB."))), Times.Once);
            _curriculoRepositoryMock.Verify(c => c.AtualizarCurriculo(It.IsAny<Curriculo>()), Times.Never);
            _curriculoRepositoryMock.Verify(c => c.UnitOfWork.Commit(), Times.Never);
        }
    }
    
    
    #endregion
    
    #region SetupMocks
    
    private void SetupMocks(bool curriculoExistente = false, bool commit = false, bool retornaComPdf = false)
    {
        var curriculo = new Curriculo
        {
            UsuarioId = 1,
            Telefone = "85 988888888",
            EstadoCivil = "Solteiro",
            Genero = "Masculino",
            RacaEtnia = "Branco",
            GrauDeFormacao = "Graduação",
            Cep = "12345678",
            Endereco = "Rua Teste, 123",
            Cidade = "Cidade Teste",
            Estado = "CE",
            EPessoaComDeficiencia = false,
            EDeficienciaAuditiva = false,
            EDeficienciaFisica = false,
            EDeficienciaIntelectual = false,
            EDeficienciaMotora = false,
            EDeficienciaVisual = false,
            ELgbtqia = false,
            EBaixaRenda = false,
            Titulo = "Desenvolvedor Full Stack",
            AreaDeAtuacao = "Tecnologia da Informação",
            ResumoProfissional = "Profissional com experiência em desenvolvimento web.",
            Linkedin = "https://www.linkedin.com/in/seuperfil",
            Portfolio = "https://www.portfolio.com",
            Pdf = retornaComPdf ? "https://pdf.com/pdfteste.pdf" : null,
            Clt = true,
            Pj = false,
            Temporario = false,
            Presencial = true,
            Remoto = false,
            Hibrido = false
        };

        _curriculoRepositoryMock
            .Setup(c => c.FirstOrDefault(It.IsAny<Expression<Func<Curriculo, bool>>>()))
            .ReturnsAsync(curriculoExistente ? curriculo : null);

        _curriculoRepositoryMock
            .Setup(c => c.UnitOfWork.Commit())
            .ReturnsAsync(commit);
    
        _curriculoRepositoryMock
            .Setup(c => c.ObterCurriculoPorId(It.Is<int>(id => id == 1)))
            .ReturnsAsync(curriculo);
        
        _fileServiceMock
            .Setup(c => c.UploadPdf(It.IsAny<IFormFile>(), It.IsAny<EUploadPath>(), It.IsAny<EPathAccess>(),
                It.IsAny<int>()))
            .ReturnsAsync("path");
    }


    #endregion
}