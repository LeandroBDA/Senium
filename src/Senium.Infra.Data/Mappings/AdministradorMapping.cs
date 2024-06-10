using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Senium.Domain.Entities;

namespace Senium.Infra.Data.Mappings;

public class AdministradorMapping : IEntityTypeConfiguration<Administrador>
{
    public void Configure(EntityTypeBuilder<Administrador> builder)
    {
        builder.Property(a => a.Nome)
            .IsRequired();
        
        builder.Property(a => a.Email)
            .IsRequired();
        
        builder.Property(a => a.Senha)
            .IsRequired();
    }
}