using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Senium.Domain.Entities;

namespace Senium.Infra.Data.Mappings;

public class ExperienciaMapping : IEntityTypeConfiguration<Experiencia>
{
    public void Configure(EntityTypeBuilder<Experiencia> builder)
    {
        builder.Property(e => e.Cargo)
            .IsRequired();

        builder.Property(e => e.Empresa)
            .IsRequired();

        builder.Property(e => e.Descricao)
            .IsRequired()
            .HasMaxLength(300);

        builder.Property(e => e.DataDeInicio)
            .IsRequired();
    }
}