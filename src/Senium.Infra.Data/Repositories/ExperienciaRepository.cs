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

    public void Adicionar(Experiencia experiencia)
    {
        Context.Experiencias.Add(experiencia);
    }

    public void Atualizar(Experiencia experiencia)
    {
        Context.Experiencias.Update(experiencia);
    }

    public void Remover(Experiencia experiencia)
    {
        Context.Experiencias.Remove(experiencia);
    }

    public async Task<Experiencia?> ObterExperienciaPorId(int id, int curriculoId)
    {
        return await Context.Experiencias
            .Include(x => x.Curriculo)
            .FirstOrDefaultAsync(x => x.Id == id && x.CurriculoId == curriculoId);
    }
}