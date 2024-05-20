using Microsoft.EntityFrameworkCore;
using Senium.Domain.Contracts.Repositories;
using Senium.Domain.Entities;
using Senium.Infra.Data.Abstractions;
using Senium.Infra.Data.Context;

namespace Senium.Infra.Data.Repositories;

public class CurriculoRepository : Repository<Curriculo>, ICurriculoRepository
{
    public CurriculoRepository(BaseApplicationDbContext context) : base(context)
    {
    }
    
    public async Task<Curriculo?> ObterPorId(int id)
    {
        return await Context.Curriculos
            .FirstOrDefaultAsync(c => c.Id == id);
    }

    public void Adicionar(Curriculo curriculo)
    { 
        Context.Curriculos.Add(curriculo);
    }

    public void Editar(Curriculo curriculo)
    {
        Context.Curriculos.Update(curriculo);
    }

    public void Remover(Curriculo curriculo)
    {
        Context.Curriculos.Remove(curriculo);
    }
}