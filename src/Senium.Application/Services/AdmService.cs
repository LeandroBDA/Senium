// using AutoMapper;
// using Microsoft.AspNetCore.Identity;
// using Senium.Application.Contracts.Services;
// using Senium.Application.Dto.V1.Adm;
// using Senium.Application.Notifications;
// using Senium.Domain.Contracts.Repositories;
// using Senium.Domain.Entities;
//
// namespace Senium.Application.Services;
//
// public class AdmService : BaseService, IAdmService
// {
//     private readonly IAdmRepository _admRepository;
//     private readonly IPasswordHasher<Administrador> _passwordHasher;
//     private IAdmService _admServiceImplementation;
//
//     public AdmService(INotificator notificator,
//         IMapper mapper,
//         IAdmRepository admRepository,
//         IPasswordHasher<Administrador> passwordHasher) : base(notificator, mapper)
//     {
//         _admRepository = admRepository;
//         _passwordHasher = passwordHasher;
//     }
// }