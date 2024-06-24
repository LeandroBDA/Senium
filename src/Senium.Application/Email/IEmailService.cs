using Senium.Domain.Entities;

namespace Senium.Application.Email;

public interface IEmailService
{
    Task EnviarEmailRecuperarSenhaUsuario(Usuario usuario);
    Task EnviarEmailRecuperarSenhaAdministrador(Administrador administrador);
}