using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Senium.Domain.Entities;

namespace Senium.Infra.Data.Mappings;

public class CurriculoMapping : IEntityTypeConfiguration<Curriculo>
{
    public void Configure(EntityTypeBuilder<Curriculo> builder)
    {
        builder.Property(x => x.Telefone)
            .IsRequired();
        
        builder.Property(x => x.EstadoCivil)
            .IsRequired()
            .HasMaxLength(10);
        
        builder.Property(x => x.Genero)
            .IsRequired()
            .HasMaxLength(9);
        
        builder.Property(x => x.RacaEtnia)
            .IsRequired()
            .HasMaxLength(7);
        
        builder.Property(x => x.GrauDeFormacao)
            .IsRequired()
            .HasMaxLength(30);

        builder.Property(x => x.Cep)
            .IsRequired()
            .HasDefaultValue("00000-00");

        builder.Property(x => x.Endereco)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(x => x.Cidade)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(x => x.Estado)
            .HasMaxLength(50);
    }
}