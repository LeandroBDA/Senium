using AutoMapper;
using Senium.Application.Notifications;

namespace Senium.Application.Services;

public class BaseService
{
    protected readonly INotificator Notificator;
    protected readonly IMapper Mapper;

    protected BaseService(INotificator notificator, IMapper mapper)
    {
        Notificator = notificator;
        Mapper = mapper;
    }
}