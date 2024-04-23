using Microsoft.EntityFrameworkCore;

namespace Senium.Infra.Data.Context;

public class ApplicationDbContext : BaseApplicationDbContext
{
    public ApplicationDbContext(DbContextOptions options) : base(options)
    {
    }
}