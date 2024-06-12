using System.ComponentModel;

namespace Senium.Core.Enums;

public enum ETipoUsuario
{
    [Description("Administrador Geral")]
    AdministradorGeral = 1,
    [Description("Administrador Comum")]
    AdministradorComum = 2,
    [Description("Comum")]
    Comum = 3
}