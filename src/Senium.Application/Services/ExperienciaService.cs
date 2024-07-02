using AutoMapper;
using Microsoft.AspNetCore.Http;
using Senium.Application.Contracts.Services;
using Senium.Application.Dto.V1.Experiencia;
using Senium.Application.Notifications;
using Senium.Core.Extensions;
using Senium.Domain.Contracts.Repositories;
using Senium.Domain.Entities;

namespace Senium.Application.Services;

public class ExperienciaService : BaseService, IExperienciaService
{
    private readonly IExperienciaRepository _experienciaRepository;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public ExperienciaService(INotificator notificator,
        IMapper mapper,
        IExperienciaRepository experienciaRepository,
        IHttpContextAccessor httpContextAccessor) 
        : base(notificator, mapper)
    {
        _experienciaRepository = experienciaRepository;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<ExperienciaDto?> AdicionarExperiencia(AdicionarExperienciaDto dto)
    {
        var experiencia = Mapper.Map<Experiencia>(dto);
        experiencia.UsuarioId = _httpContextAccessor.ObterUsuarioId() ?? 0;
        
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

    public async Task<List<ExperienciaDto>?> ObterExperienciaPorUsuarioId(int usuarioId)
    {
        var experiencias = await _experienciaRepository.ObterExperienciaDoUsuario(usuarioId);

        if (experiencias.Any()) 
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
            && a.UsuarioId == experiencia.UsuarioId
            && a.DataDeInicio == experiencia.DataDeInicio
            && a.DataDeTermino == experiencia.DataDeTermino
            && a.TrabalhoAtual == experiencia.TrabalhoAtual);

        if (experienciaExistente != null)
        {
            Notificator.Handle($"Já existe uma experiência como esta cadastrada!");
        }

        return !Notificator.HasNotification;
    }
}