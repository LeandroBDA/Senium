using System.Reflection;
using FluentValidation.Results;
using Microsoft.EntityFrameworkCore;
using Senium.Domain.Contracts.Interfaces;
using Senium.Domain.Entities;
using Senium.Infra.Data.Extensions;

namespace Senium.Infra.Data.Context;

public class BaseApplicationDbContext : DbContext, IUnitOfWork
{
    public BaseApplicationDbContext(DbContextOptions options) : base(options) { }

    public DbSet<Administrador?> Administradores { get; set; } = null!;
    public DbSet<Usuario> Usuarios { get; set; } = null!;
    public DbSet<Curriculo> Curriculos { get; set; } = null!;
    public DbSet<Experiencia> Experiencias { get; set; } = null!;
    public DbSet<Empresa> Empresas { get; set; } = null!;
    
    public async Task<bool> Commit() => await SaveChangesAsync() > 0;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        
        ApplyConfigurations(modelBuilder);
        
        base.OnModelCreating(modelBuilder);
    }
    
    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken())
    {
        ApplyTrackingChanges();
        return base.SaveChangesAsync(cancellationToken);
    }

    private void ApplyTrackingChanges()
    {
        var entries = ChangeTracker
            .Entries()
            .Where(e => e.Entity is ITracking && e.State is EntityState.Added or EntityState.Modified);

        foreach (var entityEntry in entries)
        {
            ((ITracking)entityEntry.Entity).AtualizadoEm = DateTime.Now;

            if (entityEntry.State != EntityState.Added)
                continue;
            
            ((ITracking)entityEntry.Entity).CriadoEm = DateTime.Now;
        }
    }

    private static void ApplyConfigurations(ModelBuilder modelBuilder)
    {
        modelBuilder.Ignore<ValidationResult>();

        modelBuilder.ApplyEntityConfiguration();
        modelBuilder.ApplyTrackingConfiguration();
    }
}