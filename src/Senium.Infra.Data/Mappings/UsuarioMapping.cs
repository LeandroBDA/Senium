using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Senium.Domain.Entities;

namespace Senium.Infra.Data.Mappings;

public class UsuarioMapping : IEntityTypeConfiguration<Usuario>
{
    public void Configure(EntityTypeBuilder<Usuario> builder)
    {
        builder
            .Property(u => u.Nome)
            .HasMaxLength(100)
            .IsRequired();

        builder
            .Property(u => u.Email)
            .HasMaxLength(256)
            .IsRequired();

        builder
            .Property(u => u.Senha)
            .HasMaxLength(255)
            .IsRequired();
    }
}