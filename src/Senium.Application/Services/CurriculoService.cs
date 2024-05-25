using AutoMapper;
using Microsoft.AspNetCore.Http;
using Senium.Application.Contracts.Services;
using Senium.Application.Dto.V1.Curriculo;
using Senium.Application.Notifications;
using Senium.Core.Extensions;
using Senium.Domain.Contracts.Repositories;
using Senium.Domain.Entities;

namespace Senium.Application.Services;

public class CurriculoService : BaseService, ICurriculoService
{
    private readonly ICurriculoRepository _curriculoRepository;
    private readonly IHttpContextAccessor _httpContextAccessor;
    
    public CurriculoService(INotificator notificator, IMapper mapper, IHttpContextAccessor httpContextAccessor,ICurriculoRepository curriculoRepository) : base(notificator, mapper) 
    {
        _httpContextAccessor = httpContextAccessor;
        _curriculoRepository = curriculoRepository;
    }
    
    public async Task<CurriculoDto?> ObterCurriculoPorId(int id)
    {
        var curriculo = await _curriculoRepository.ObterCurriculoPorId(id);
        if (curriculo != null)
        {
            return Mapper.Map<CurriculoDto>(curriculo);
        }
        
        Notificator.HandleNotFoundResource();
        return null;
    }

    public async Task<CurriculoDto?> AdicionarCurriculo(AdicionarCurriculoDto curriculodto)
    {
        var curriculo = Mapper.Map<Curriculo>(curriculodto);
        curriculo.UsuarioId = _httpContextAccessor.ObterUsuarioId() ?? 0;
        
        if (!await Validar(curriculo))
        {
            return null;
        }
        
        _curriculoRepository.AdicionarCurriculo(curriculo);
        
        if (await _curriculoRepository.UnitOfWork.Commit())
        {
            return Mapper.Map<CurriculoDto>(curriculo);
        }
        Notificator.Handle("Não foi possível adicionar um curriculo.");
        return null;
    }

    public async Task<CurriculoDto?> AtualizarCurriculo(int id, AtualizarCurriculoDto curriculoDto)
    {
        if (id != curriculoDto.Id)
        {
            Notificator.Handle("Os IDs não conferem");
            return null;
        }
        
        var curriculo = await _curriculoRepository.ObterCurriculoPorId(id);
        if (curriculo == null)
        {
            Notificator.HandleNotFoundResource();
            return null;
        }
      
        Mapper.Map(curriculoDto, curriculo);

        if (!await ValidarAtualizacao(curriculo))
        {
            return null;
        }

        _curriculoRepository.AtualizarCurriculo(curriculo);

        if (await _curriculoRepository.UnitOfWork.Commit())
        {
          
            return Mapper.Map<CurriculoDto>(curriculo);
        }
        
        Notificator.Handle("Não foi possível alterar o currículo");
        return null;
    }
    public async Task<bool> Validar(Curriculo curriculo)
    {
        if (!curriculo.Validar(out var validationResult))
        {
            Notificator.Handle(validationResult.Errors);
            return false; 
        }
        
        var curriculoExistente = await _curriculoRepository.FirstOrDefault(a => a.UsuarioId == curriculo.UsuarioId);
        if (curriculoExistente != null)
        {
            Notificator.Handle("Já existe um currículo para este usuário!");
            return false; 
        }


        return !Notificator.HasNotification;
    }
    
    public async Task<bool> ValidarAtualizacao(Curriculo curriculo)
    {
        if (!curriculo.Validar(out var validationResult))
        {
            Notificator.Handle(validationResult.Errors);
            return false;
        }

        return !Notificator.HasNotification;
    }
}