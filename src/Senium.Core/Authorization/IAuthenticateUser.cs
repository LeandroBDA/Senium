using Microsoft.AspNetCore.Http;
using Senium.Core.Enums;
using Senium.Core.Extensions;

namespace Senium.Core.Authorization;

public interface IAuthenticatedUser
{
    public int Id { get; }
    public ETipoUsuario? TipoUsuario { get; }    
    public string Nome { get; }
    public string Email { get; }
    
    
    public bool UsuarioLogado { get; }
    public bool UsuarioComum { get; }
    public bool UsuarioAdministradorGeral { get; }
    public bool UsuarioAdministradorComum { get; }

    int ObterIdentificador();
}

public class AuthenticatedUser : IAuthenticatedUser
{
    public int Id { get; } = -1;
    public ETipoUsuario? TipoUsuario { get; }    
    public string Nome { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;

    public bool UsuarioLogado => Id > 0;
    public bool UsuarioComum => TipoUsuario is ETipoUsuario.Comum;
    public bool UsuarioAdministradorComum => TipoUsuario is ETipoUsuario.AdministradorComum;
    public bool UsuarioAdministradorGeral => TipoUsuario is ETipoUsuario.AdministradorGeral;
    
    public AuthenticatedUser()
    { }
    
    public AuthenticatedUser(IHttpContextAccessor httpContextAccessor)
    {
        Id = httpContextAccessor.ObterUsuarioId()!.Value;
        TipoUsuario = httpContextAccessor.ObterTipoUsuario()!.Value;

        Nome = httpContextAccessor.ObterNome();
        Email = httpContextAccessor.ObterEmail();
    }

    public int ObterIdentificador() => Id;
}