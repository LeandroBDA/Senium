using FluentValidation;
using Senium.Domain.Entities;

namespace Senium.Domain.Validations;

public class CurriculoValidation : AbstractValidator<Curriculo>
{
    public CurriculoValidation()
    {
       
        RuleFor(x => x.Telefone)
            .NotEmpty()
            .WithMessage("O campo telefone não pode ser vazio.")

            .MinimumLength(8)
            .WithMessage("O número deve ter no mínimo 8 caracteres.")

            .MaximumLength(11)
            .WithMessage("O número deve ter no máximo 11 caracteres.");

        RuleFor(x => x.Cidade)
            .NotEmpty()
            .WithMessage("O campo cidade é necessário.")
            
            .MinimumLength(3)
            .WithMessage("O campo cidade deve ter no mínimo 3 caracteres")
           
            .MaximumLength(30)
            .WithMessage("O campo cidade deve ter no máximo 30 caracteres");
        
        RuleFor(x => x.Endereco)
            .NotEmpty()
            .WithMessage("O campo Endereço é necessário.")
            
            .MinimumLength(5)
            .WithMessage("O campo endereço deve ter no mínimo 5 caracteres")
           
            .MaximumLength(50)
            .WithMessage("O campo endereço deve ter no máximo 50 caracteres");
        
        RuleFor(x => x.Estado)
            .NotEmpty()
            .WithMessage("O campo estado é necessário.")
            
            .MinimumLength(3)
            .WithMessage("O campo estado deve ter no mínimo 3 caracteres")
           
            .MaximumLength(50)
            .WithMessage("O campo estado deve ter no máximo 50 caracteres");

      
      
        // _____________________________________________________________________________________________________

        RuleFor(x => x.EstadoCivil).NotEmpty().WithMessage("Estado Civil é necessário");
        
        // _____________________________________________________________________________________________________

        RuleFor(x => x.Genero).NotEmpty().WithMessage("Genero é necessário");
       
        // _____________________________________________________________________________________________________

        RuleFor(x => x.RacaEtnia).NotEmpty().WithMessage("RacaEtnia é necessário");
            
        // _____________________________________________________________________________________________________

        RuleFor(x => x.GrauDeFormacao).NotEmpty().WithMessage("Grau de formação necessário");
           
        // _____________________________________________________________________________________________________

        RuleFor(x => x.Cep).Empty().When(x => x.Cep != null)
            .When(x => !string.IsNullOrEmpty(x.Cep));
       
        // _____________________________________________________________________________________________________

        RuleFor(x => x.EPessoaComDeficiencia).Empty().When(x => x.EPessoaComDeficiencia != null || x.EPessoaComDeficiencia == null);
       
        var Epcd =  new Curriculo();

        if (Epcd.EPessoaComDeficiencia != null)
        {
            RuleFor(x => x.EDeficienciaAuditiva).NotEmpty();
            RuleFor(x => x.EDeficienciaFisica).NotEmpty();
            RuleFor(x => x.EDeficienciaIntelectual).NotEmpty();
            RuleFor(x => x.EDeficienciaMotora).NotEmpty();
            RuleFor(x => x.EDeficienciaVisual).NotEmpty();
        } 

        RuleFor(x => x.ELgbtqia).Empty().When(x => x.ELgbtqia != null || x.ELgbtqia == null);
       
        RuleFor(x => x.EBaixaRenda).Empty().When(x => x.EBaixaRenda != null || x.EBaixaRenda == null);
        
        
        // _____________________________________________________________________________________________________
    }  
}