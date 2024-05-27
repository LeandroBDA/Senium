using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Senium.Domain.Entities;

namespace Senium.Infra.Data.Mappings;

public class CurriculoMapping : IEntityTypeConfiguration<Curriculo>
{
    public void Configure(EntityTypeBuilder<Curriculo> builder)
    {
        // Dados Pessoais
        builder.Property(x => x.Telefone)
            .IsRequired()
            .HasMaxLength(15);
        
        builder.Property(x => x.EstadoCivil)
            .IsRequired()
            .HasMaxLength(13);
        
        builder.Property(x => x.Genero)
            .IsRequired()
            .HasMaxLength(9);
        
        builder.Property(x => x.RacaEtnia)
            .IsRequired()
            .HasMaxLength(21);
        
        builder.Property(x => x.GrauDeFormacao)
            .IsRequired()
            .HasMaxLength(30);

        builder.Property(x => x.Cep)
            .IsRequired()
            .HasDefaultValue(9);

        builder.Property(x => x.Endereco)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(x => x.Cidade)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(x => x.Estado)
            .IsRequired()
            .HasMaxLength(50);
        
        //Dados Profissionais

        builder.Property(x => x.Titulo)
            .IsRequired();

        builder.Property(x => x.AreaDeAtuacao)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(x => x.ResumoProfissional)
            .IsRequired()
            .HasMaxLength(300);
        
        builder.HasMany(c => c.Experiencias)
            .WithOne(e => e.Curriculo)
            .HasForeignKey(e => e.CurriculoId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}