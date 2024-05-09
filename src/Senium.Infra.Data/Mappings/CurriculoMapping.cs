using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Senium.Domain.Entities;

namespace Senium.Infra.Data.Mappings;

public class CurriculoMapping : IEntityTypeConfiguration<Curriculo>
{
    public void Configure(EntityTypeBuilder<Curriculo> builder)
    {
        builder.ToTable("User");
           
        builder.HasKey(x => x.Id);
        
        builder.Property(x => x.Id)
            .UseMySqlIdentityColumn()
            .HasColumnType("BIGINT");

        builder.Property(x => x.Telefone)
            .IsRequired()
            .HasDefaultValue("00 000000000");
        
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
          
        builder.Property(x => x.EPessoaComDeficiencia)
            .IsRequired()
            .HasMaxLength(3);
        
        builder.Property(x => x.EDeficienciaAuditiva)
            .IsRequired()
            .HasMaxLength(3);
        
        builder.Property(x => x.EDeficienciaFisica)
            .IsRequired()
            .HasMaxLength(3);
        
        builder.Property(x => x.EDeficienciaIntelectual)
            .IsRequired()
            .HasMaxLength(3);
        
        builder.Property(x => x.EDeficienciaMotora)
            .IsRequired()
            .HasMaxLength(3);
        
        builder.Property(x => x.EDeficienciaVisual)
            .IsRequired()
            .HasMaxLength(3);
    }
}