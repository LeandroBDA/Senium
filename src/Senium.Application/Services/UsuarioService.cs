using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Senium.Application.Contracts.Services;
using Senium.Application.Dto.V1.Usuario;
using Senium.Application.Notifications;
using Senium.Domain.Contracts.Repositories;
using Senium.Domain.Entities;

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

    public async Task<UsuarioDto?> Adicionar(AdicionarUsuarioDto dto)
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
        _usuarioRepository.Cadastrar(usuario);
        
        if (await _usuarioRepository.UnitOfWork.Commit())
        {
            return Mapper.Map<UsuarioDto>(usuario);
        }
        Notificator.Handle("Não foi possível cadastrar o usuário");
        return null;
    }

    public async Task<UsuarioDto?> ObterPorId(int id)
    {
        var usuario = await _usuarioRepository.ObterPorId(id);
        if (usuario != null)
        {
            return Mapper.Map<UsuarioDto>(usuario);
        }
        
        Notificator.HandleNotFoundResource();
        return null;
    }

    public async Task<UsuarioDto?> ObterPorEmail(string email)
    {
        var usuario = await _usuarioRepository.ObterPorEmail(email);
        if (usuario != null)
        {
            return Mapper.Map<UsuarioDto>(usuario);
        }
        
        Notificator.HandleNotFoundResource();
        return null;
    }

    public async Task<List<UsuarioDto>> ObterTodos()
    {
        var usuarios = await _usuarioRepository.ObterTodos();
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
}