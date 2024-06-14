using AutoMapper;
using Senium.Application.Contracts.Services;
using Senium.Application.Dto.V1.Experiencia;
using Senium.Application.Notifications;
using Senium.Domain.Contracts.Repositories;
using Senium.Domain.Entities;

namespace Senium.Application.Services;

public class ExperienciaService : BaseService, IExperienciaService
{
    private readonly IExperienciaRepository _experienciaRepository;
    private readonly ICurriculoRepository _curriculoRepository;

    public ExperienciaService(INotificator notificator, IMapper mapper, IExperienciaRepository experienciaRepository, ICurriculoRepository curriculoRepository) 
        : base(notificator, mapper)
    {
        _experienciaRepository = experienciaRepository;
        _curriculoRepository = curriculoRepository;
    }

    public async Task<ExperienciaDto?> AdicionarExperiencia(AdicionarExperienciaDto dto)
    {
        var curriculo = await _curriculoRepository.ObterCurriculoPorId(dto.CurriculoId);
        if (curriculo == null)
        {
            Notificator.Handle("Currículo não encontrado.");
            return null;
        }

        var experiencia = Mapper.Map<Experiencia>(dto);
        experiencia.CurriculoId = curriculo.Id;

        if (!await Validar(experiencia))
        {
            return null;
        }

        _experienciaRepository.AdicionarExperiencia(experiencia);
        if (await _experienciaRepository.UnitOfWork.Commit())
        {
            return Mapper.Map<ExperienciaDto>(experiencia);
        }

        Notificator.Handle("Não foi possível cadastrar experiência");
        return null;
    }

    public async Task<ExperienciaDto?> AtualizarExperiencia(int id, AtualizarExperienciaDto dto)
    {
        if (id != dto.Id)
        {
            Notificator.Handle("Os Ids não conferem");
            return null;
        }

        var experiencia = await _experienciaRepository.ObterExperienciaPorId(id);
        if (experiencia == null)
        {
            Notificator.HandleNotFoundResource();
            return null;
        }
        
        Mapper.Map(dto, experiencia);
        
        if (!await Validar(experiencia))
        {
            return null;
        }
        
        _experienciaRepository.AtualizarExperiencia(experiencia);
        
        if (await _experienciaRepository.UnitOfWork.Commit())
        {
            return Mapper.Map<ExperienciaDto>(experiencia);
        }

        Notificator.Handle("Não foi possível atualizar experiência");
        return null;
    }

    public async Task DeletarExperiencia(int id)
    {
        var experiencia = await _experienciaRepository.ObterExperienciaPorId(id);

        if (experiencia == null)
        {
            Notificator.HandleNotFoundResource();
            return;
        }
        
        _experienciaRepository.RemoverExperiencia(experiencia);

        if (!await _experienciaRepository.UnitOfWork.Commit())
        {
            Notificator.Handle("Não foi possível remover experiência.");
        }
    }

    public async Task<List<ExperienciaDto>?> ObterExperienciaPorCurriculoId(int curriculoId)
    {
        var experiencias = await _experienciaRepository.ObterExperienciaDoCurriculo(curriculoId);

        if (experiencias != null && experiencias.Any()) 
            return Mapper.Map<List<ExperienciaDto>>(experiencias);
    
        Notificator.HandleNotFoundResource(); 
        return null;

    }

    private async Task<bool> Validar(Experiencia experiencia)
    {
        if (!experiencia.Validar(out var validationResult))
        {
            Notificator.Handle(validationResult.Errors);
        }

        var experienciaExistente = await _experienciaRepository.FirstOrDefault(a => 
            a.Cargo == experiencia.Cargo
            && a.Empresa == experiencia.Empresa
            && a.Descricao == experiencia.Descricao
            && a.CurriculoId == experiencia.CurriculoId);

        if (experienciaExistente != null)
        {
            Notificator.Handle($"Já existe uma experiência como esta cadastrada!");
        }

        return !Notificator.HasNotification;
    }
}