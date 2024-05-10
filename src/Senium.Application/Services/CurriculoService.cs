using AutoMapper;
using Senium.Application.Contracts.Services;
using Senium.Application.Dto.V1.Curriculo;
using Senium.Application.Notifications;
using Senium.Domain.Contracts.Repositories;
using Senium.Domain.Entities;

namespace Senium.Application.Services;

public class CurriculoService : BaseService, ICurriculoService
{
    private readonly ICurriculoRepository _curriculoRepository; //Após a mudança o erro continua, possivél erro na máquina. Migrations para fazer.
    
    public CurriculoService(INotificator notificator, IMapper mapper, ICurriculoRepository curriculoRepository) : base(notificator, mapper) 
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
            Notificator.Handle("Os IDs não conferem");
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
        
        Notificator.Handle("Não foi possível alterar o currículo");
        return null;
    }
    
    public Task<CurriculoDto?> Remove(int id)
    {
        throw new NotImplementedException();
    }
  
    public async Task<bool?> Validar(Curriculo curriculo)
    {
        // Valida o currículo
        if (!curriculo.Validar(out var validationResult))
        {
            Notificator.Handle(validationResult.Errors);
            return false; 
        }
        
        var curriculoExistente = await _curriculoRepository.FirstOrDefault(a => a.Id != curriculo.Id);
        if (curriculoExistente != null)
        {
            Notificator.Handle("Já existe um usuário cadastrado com esse e-mail!");
            return false; 
        }
        
        return !Notificator.HasNotification;
    }
}