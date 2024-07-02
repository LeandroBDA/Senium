using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Senium.Application.Contracts.Services;
using Senium.Application.Dto.V1.Administrador;
using Senium.Application.Notifications;
using Senium.Domain.Contracts.Repositories;
using Senium.Domain.Entities;
using Senium.Domain.Validations;

namespace Senium.Application.Services;

public class AdministradorService : BaseService, IAdministradorService
{
    private readonly IAdministradorRepository _administradorRepository;
    private readonly IPasswordHasher<Administrador> _passwordHasher;
    
    public AdministradorService(INotificator notificator,
        IMapper mapper,
        IAdministradorRepository administradorRepository,
        IPasswordHasher<Administrador> passwordHasher) : base(notificator, mapper)
    {
        _administradorRepository = administradorRepository;
        _passwordHasher = passwordHasher;
    }
    
    public async Task<AdministradorDto?> AdicionarAdm(AdicionarAdministradorDto dto)
    {
        var administrador = Mapper.Map<Administrador>(dto);

        if (!await Validar(administrador))
        {
            return null;
        }

        administrador.Senha = _passwordHasher.HashPassword(administrador, administrador.Senha);
        _administradorRepository.AdicionarAdm(administrador);

        if (await _administradorRepository.UnitOfWork.Commit())
        {
            return Mapper.Map<AdministradorDto>(administrador);
        }
        
        Notificator.Handle("Não foi possível cadastrar o administrador");
        return null;
    }

    public async Task<AdministradorDto?> AtualizarAdm(int id, AtualizarAdministradorDto dto)
    {
        if (id != dto.Id)
        {
            Notificator.Handle("Os ids não conferem");
            return null;
        }

        if (string.IsNullOrEmpty(dto.Nome) & string.IsNullOrEmpty(dto.Email) & string.IsNullOrEmpty(dto.Senha))
        {
            Notificator.Handle("Nenhuma alteração a ser feita");
            return null;
        }

        var administrador = await _administradorRepository.ObterAdmPorId(id);
        if (administrador == null)
        {
            Notificator.HandleNotFoundResource();
            return null;
        }
        
        var nomeAtual = administrador.Nome;
        var emailAtual = administrador.Email;
        var senhaAtual = administrador.Senha;
        
        dto.Nome = !string.IsNullOrEmpty(dto.Nome) ? dto.Nome : nomeAtual;
        dto.Email = !string.IsNullOrEmpty(dto.Email) ? dto.Email : emailAtual;
        
        Mapper.Map(dto, administrador);
        
        if (!await ValidarAtualizacao(administrador))
        {
            return null;
        }
        
        administrador.Senha = !string.IsNullOrEmpty(dto.Senha) ? _passwordHasher.HashPassword(administrador, dto.Senha) : senhaAtual;
        _administradorRepository.AtualizarAdm(administrador);

        if (await _administradorRepository.UnitOfWork.Commit())
        {
            return Mapper.Map<AdministradorDto>(administrador);
        }

        Notificator.Handle("Não foi possível atualizar o administrador");
        return null;
    }


    public async Task RemoverAdm(int id)
    {
        if (id == 1)
        {
            Notificator.Handle("Administrador Geral não pode ser excluido.");
            return;
        }
        
        var administrador = await _administradorRepository.ObterAdmPorId(id);

        if (administrador == null)
        {
            Notificator.HandleNotFoundResource();
            return;
        }
        
        _administradorRepository.RemoverAdm(administrador);

        if (!await _administradorRepository.UnitOfWork.Commit())
        {
            Notificator.Handle("Não foi possível remover administrador.");
        }
    }

    public async Task<List<AdministradorDto>> ObterTodosAdm()
    {
        var administradores = await _administradorRepository.ObterTodosAdm();
        return Mapper.Map<List<AdministradorDto>>(administradores);

    }
    
    private async Task<bool> Validar(Administrador administrador)
    {
        if (!administrador.Validar(out var validationResult))
        {
            Notificator.Handle(validationResult.Errors);
        }
        
        var administradorExistente = await _administradorRepository.FirstOrDefault(a => 
            a.Email == administrador.Email  && a.Id != administrador.Id);
        if (administradorExistente != null)
        {
            Notificator.Handle($"Já existe um administrador cadastrado cadastrado com essas identificações!");
        }

        return !Notificator.HasNotification;
    }
    
    private async Task<bool> ValidarAtualizacao(Administrador dto)
    {
        var validator = new AtualizarAdministradorValidator();
        var validationResult = validator.Validate(dto);

        if (!validationResult.IsValid)
        {
            Notificator.Handle(validationResult.Errors);
        }

        var administradorExistente = await _administradorRepository.FirstOrDefault(a => 
            a.Email == dto.Email && a.Id != dto.Id);
    
        if (administradorExistente != null)
        {
            Notificator.Handle("Já existe um administrador cadastrado com esse e-mail!");
        }

        return !Notificator.HasNotification;
    }
    
}