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
    
    public ExperienciaService(INotificator notificator, IMapper mapper, IExperienciaRepository experienciaRepository) : base(notificator, mapper)
    {
        _experienciaRepository = experienciaRepository;
    }

    public async Task<ExperienciaDto?> Adicionar(AdicionarExperienciaDto dto)
    {
        var experiencia = Mapper.Map<Experiencia>(dto);
        
        if (!await Validar(experiencia))
        {
            return null;
        }
        
        _experienciaRepository.Adicionar(experiencia);
        if (await _experienciaRepository.UnitOfWork.Commit())
        {
            return Mapper.Map<ExperienciaDto>(experiencia);
        }
        
        Notificator.Handle("Não foi possível cadastrar experiência");
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
            && a.Descricao == experiencia.Descricao);
        
        if (experienciaExistente != null)
        {
            Notificator.Handle($"Já existe uma experiência como esta cadastrada!");
        }

        return !Notificator.HasNotification;
    }
}