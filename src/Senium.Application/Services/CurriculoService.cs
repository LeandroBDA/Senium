using AutoMapper;
using Senium.Application.Contracts.Services;
using Senium.Application.Dto.V1.Curriculo;
using Senium.Application.Notifications;
using Senium.Domain.Entities;
using Senium.Infra.Data.Repositories;

namespace Senium.Application.Services;

public class CurriculoService : BaseService, ICurriculoService
{
    private readonly CurriculoRepository _curriculoRepository;
    
    protected CurriculoService(INotificator notificator, IMapper mapper, CurriculoRepository curriculoRepository) : base(notificator, mapper)
    {
        _curriculoRepository = curriculoRepository;
    }
    
    public async Task<CurriculoDto?> ObterPorId(int id)
    {
        var curriculo = await _curriculoRepository.ObterPorId(id);
        return curriculo != null ? Mapper.Map<CurriculoDto>(curriculo) : null;
    }

    public async Task<CurriculoDto?> Adicionar(AdicionarCurriculoDto curriculodto)
    {
        var curriculo = Mapper.Map<Curriculo>(curriculodto);
        _curriculoRepository.Adicionar(curriculo);

        if (await _curriculoRepository.UnitOfWork.Commit())
        {
            return Mapper.Map<CurriculoDto>(curriculo);
        }
        
        Notificator.Handle("Não foi possível dicionar um curriculo.");
        return null;
    }

    public async Task<CurriculoDto?> Editar(int id, CurriculoDto curriculoDto)
    {
        if (id != curriculoDto.Id)
        {
            Notificator.Handle("Os id não conferem");
            return null;
        }

        var curriculo = await _curriculoRepository.ObterPorId(id);
        if (curriculo == null)
        {
            Notificator.HandleNotFoundResource();
            return null;
        }

        Mapper.Map(curriculoDto, curriculo); 
        _curriculoRepository.Editar(curriculo);

        if (await _curriculoRepository.UnitOfWork.Commit())
        {
            return Mapper.Map<CurriculoDto>(curriculo);
        }
       
        Notificator.Handle("Não foi possivel auterar o curriculo");
        return null;
    }

    public Task<CurriculoDto?> Remove(int id)
    {
        throw new NotImplementedException();
    }
  
    private async Task<bool> Validar(Curriculo curriculo)
    {
        if (!curriculo.Validar(out var validationResult))
        {
            Notificator.Handle(validationResult.Errors);
        }
        
        var curriculoExistente = await _curriculoRepository.FirstOrDefault(a => 
            a.Id != curriculo.Id);
        if (curriculoExistente != null)
        {
            Notificator.Handle($"Já existe um usuario cadastrado com esse e-mail!");
        }

        return !Notificator.HasNotification;
    }
}