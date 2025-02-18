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
    
    public async Task<Curriculo?> ObterCurriculoPorUsuarioId(int id)
    {
        return await Context.Curriculos.AsNoTrackingWithIdentityResolution()
            .FirstOrDefaultAsync(c => c.UsuarioId == id);
    }

    public void AdicionarCurriculo(Curriculo curriculo)
    { 
        Context.Curriculos.Add(curriculo);
    }

    public void AtualizarCurriculo(Curriculo curriculo)
    {
        Context.Curriculos.Update(curriculo);
    }

    public void RemoverCurriculo(Curriculo curriculo)
    {
        Context.Curriculos.Remove(curriculo);
    }
    
    public async Task<List<Curriculo>> ObterTodosCurriculo()
    {
        return await Context.Curriculos.AsNoTrackingWithIdentityResolution().ToListAsync();
    }
}