using Senium.Application.Notifications;

namespace Senium.API.Controllers.V1;

public abstract class MainController : BaseController
{
    protected MainController(INotificator notificator) : base(notificator)
    {
    }
}