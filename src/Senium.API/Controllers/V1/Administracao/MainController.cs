using Senium.Application.Notifications;
using Senium.Core.Authorization;
using Senium.Core.Enums;

namespace Senium.API.Controllers.V1.Administracao;


public abstract class MainController : BaseController
{
    protected MainController(INotificator notificator) : base(notificator)
    {
    }
}
