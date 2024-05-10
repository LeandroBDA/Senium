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


        RuleFor(x => x.EstadoCivil).NotEmpty().WithMessage("Estado Civil é necessário");
        
      
        RuleFor(x => x.Genero).NotEmpty().WithMessage("Genero é necessário");
    
        RuleFor(x => x.RacaEtnia).NotEmpty().WithMessage("RacaEtnia é necessário");
            
      
        RuleFor(x => x.GrauDeFormacao).NotEmpty().WithMessage("Grau de formação necessário");


        RuleFor(x => x.Cep).NotEmpty();

        RuleFor(x => x.EPessoaComDeficiencia).Empty();

        RuleFor(x => x)
            .Custom((obj, context) =>
            {
                if (obj.EPessoaComDeficiencia)
                {
                    if (!obj.EDeficienciaAuditiva &&
                        !obj.EDeficienciaFisica &&
                        !obj.EDeficienciaIntelectual &&
                        !obj.EDeficienciaMotora &&
                        !obj.EDeficienciaVisual)
                    {
                        context.AddFailure("Pelo menos uma deficiência deve ser selecionada.");
                    }
                }
            });

        RuleFor(x => x.ELgbtqia).Empty();

        RuleFor(x => x.EBaixaRenda).Empty();
    }
}