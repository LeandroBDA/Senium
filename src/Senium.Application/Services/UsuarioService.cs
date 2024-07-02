using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Senium.Application.Contracts.Services;
using Senium.Application.Dto.V1.Usuario;
using Senium.Application.Notifications;
using Senium.Domain.Contracts.Repositories;
using Senium.Domain.Entities;
using Senium.Domain.Validations;

namespace Senium.Application.Services;

public class UsuarioService : BaseService, IUsuarioService
{
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly IPasswordHasher<Usuario> _passwordHasher;
    
    public UsuarioService(INotificator notificator,
        IMapper mapper, 
        IUsuarioRepository usuarioRepository, 
        IPasswordHasher<Usuario> passwordHasher) : base(notificator, mapper)
    {
        _usuarioRepository = usuarioRepository;
        _passwordHasher = passwordHasher;
    }

    public async Task<UsuarioDto?> AdicionarUsuario(AdicionarUsuarioDto dto)
    {
        var usuario = Mapper.Map<Usuario>(dto);
        
        if (dto.Senha != dto.ConfirmarSenha)
        {
            Notificator.Handle("As senhas informadas não coincidem");
            return null;
        }
        
        if (!await Validar(usuario))
        {
            return null;
        }

        usuario.Senha = _passwordHasher.HashPassword(usuario, usuario.Senha);
        _usuarioRepository.CadastrarUsuario(usuario);
        
        if (await _usuarioRepository.UnitOfWork.Commit())
        {
            return Mapper.Map<UsuarioDto>(usuario);
        }
        Notificator.Handle("Não foi possível cadastrar o usuário");
        return null;
    }

    public async Task<UsuarioDto?> AtualizarUsuario(int id, AtualizarUsuarioDto dto)
    {
        if (id != dto.Id)
        {
            Notificator.Handle("Ids não conferem");
            return null;
        }

        var usuario = await _usuarioRepository.ObterUsuarioPorId(id);
        if (usuario == null)
        {
           Notificator.HandleNotFoundResource();
           return null;
        }

        Mapper.Map(dto, usuario);
        if (!await ValidarAtualizacao(usuario))
        {
            return null;
        }
        
        _usuarioRepository.AtualizarUsuario(usuario);
        if (await _usuarioRepository.UnitOfWork.Commit())
        {
            return Mapper.Map<UsuarioDto>(usuario);
        }
        
        Notificator.Handle("Não foi possivel atualizar o usuário");
        return null;
    }

    public async Task<UsuarioDto?> ObterUsuarioPorId(int id)
    {
        var usuario = await _usuarioRepository.ObterUsuarioPorId(id);
        if (usuario != null)
        {
            return Mapper.Map<UsuarioDto>(usuario);
        }
        
        Notificator.HandleNotFoundResource();
        return null;
    }

    public async Task<UsuarioDto?> ObterUsuarioPorEmail(string email)
    {
        var usuario = await _usuarioRepository.ObterUsuarioPorEmail(email);
        if (usuario != null)
        {
            return Mapper.Map<UsuarioDto>(usuario);
        }
        
        Notificator.HandleNotFoundResource();
        return null;
    }

    public async Task<List<UsuarioDto>> ObterTodosUsuarios()
    {
        var usuarios = await _usuarioRepository.ObterTodosUsuarios();
        return Mapper.Map<List<UsuarioDto>>(usuarios);
    }
    
    private async Task<bool> Validar(Usuario usuario)
    {
        if (!usuario.Validar(out var validationResult))
        {
            Notificator.Handle(validationResult.Errors);
        }
        
        var usuarioExistente = await _usuarioRepository.FirstOrDefault(a => 
            a.Email == usuario.Email  && a.Id != usuario.Id);
        if (usuarioExistente != null)
        {
            Notificator.Handle($"Já existe um usuario cadastrado com esse e-mail!");
        }

        return !Notificator.HasNotification;
    }

    private async Task<bool> ValidarAtualizacao(Usuario usuario)
    {
        var validator = new AtualizarUsuarioValidation();
        var validationResult = validator.Validate(usuario);

        if (!validationResult.IsValid)
        {
            Notificator.Handle(validationResult.Errors);
        }
        
        var usuarioExistente = await _usuarioRepository.FirstOrDefault(a => a.Email == usuario.Email && a.Id != usuario.Id);

        if (usuarioExistente != null)
        {
            Notificator.Handle("Já existe um usuario cadastrado com esse e-mail!");
        }
        
        return !Notificator.HasNotification;
    }
}