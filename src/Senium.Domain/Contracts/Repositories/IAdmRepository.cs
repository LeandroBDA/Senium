using Senium.Domain.Entities;

namespace Senium.Domain.Contracts.Repositories;

public interface IAdmRepository : IRepository<Administrador>
{
    Task<Administrador?> VerificarAdmPorId(int id);
    Task<Administrador> VerificarAdmPorEmail(string email);
    
}