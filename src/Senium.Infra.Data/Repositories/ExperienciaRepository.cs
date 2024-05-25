using Microsoft.EntityFrameworkCore;
using Senium.Domain.Contracts.Repositories;
using Senium.Domain.Entities;
using Senium.Infra.Data.Abstractions;
using Senium.Infra.Data.Context;

namespace Senium.Infra.Data.Repositories;

public class ExperienciaRepository : Repository<Experiencia>, IExperienciaRepository
{
    public ExperienciaRepository(BaseApplicationDbContext context) : base(context)
    {
    }

    public void AdicionarExperiencia(Experiencia experiencia)
    {
        Context.Experiencias.Add(experiencia);
    }

    public void AtualizarExperiencia(Experiencia experiencia)
    {
        Context.Experiencias.Update(experiencia);
    }

    public void RemoverExperiencia(Experiencia experiencia)
    {
        Context.Experiencias.Remove(experiencia);
    }

    public async Task<Experiencia?> ObterExperienciaPorId(int id)
    {
        return await Context.Experiencias.FirstOrDefaultAsync(c => c.Id == id);
    }

    public async Task<List<Experiencia>> ObterExperienciaDoCurriculo(int curriculoId)
    {
        return await Context.Experiencias
            .Include(x => x.Curriculo)
            .Where(x => x.CurriculoId == curriculoId)
            .ToListAsync();
    }
}