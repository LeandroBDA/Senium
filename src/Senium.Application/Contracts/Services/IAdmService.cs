using Senium.Application.Dto.V1.Adm;
using Senium.Domain.Entities;

namespace Senium.Application.Contracts.Services;

public interface IAdmService
{
    Task<Administrador> VerificarAdmPorId(VerificarAdmDto dto);
    Task<Administrador> VerificarAdmPorEmail(string dto);
    Task<Administrador> VerificarAdmHashedPassword(int dto);
}