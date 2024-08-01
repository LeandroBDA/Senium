using System.Text.RegularExpressions;
using FluentValidation;
using Senium.Domain.Entities;

namespace Senium.Domain.Validations;

public class CurriculoValidation : AbstractValidator<Curriculo>
{
    public CurriculoValidation()
    {
       // Dados pessoais
        RuleFor(x => x.Telefone)
            .NotEmpty()
            .WithMessage("O campo telefone não pode ser vazio.")

            .MinimumLength(11)
            .WithMessage("O número deve ter no mínimo 8 caracteres.")

            .MaximumLength(15)
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
            
            .MinimumLength(2)
            .WithMessage("O campo estado deve ter no mínimo 3 caracteres")
           
            .MaximumLength(50)
            .WithMessage("O campo estado deve ter no máximo 50 caracteres");


        RuleFor(x => x.EstadoCivil).NotEmpty().WithMessage("Estado Civil é necessário");
        
      
        RuleFor(x => x.Genero).NotEmpty().WithMessage("Genero é necessário");
    
        RuleFor(x => x.RacaEtnia).NotEmpty().WithMessage("RacaEtnia é necessário");
            
      
        RuleFor(x => x.GrauDeFormacao)
            .NotEmpty().WithMessage("Grau de formação necessário.")
            .Must(SerUmGrauDeFormacaoValido).WithMessage("Grau de formação inválido.");


        RuleFor(x => x.Cep)
            .NotEmpty().WithMessage("O CEP não pode estar vazio.")
            .Length(8).WithMessage("O CEP deve conter exatamente 8 caracteres.")
            .Matches("^[0-9]{8}$").WithMessage("O CEP deve conter apenas números.");

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
        
        // Dados Profissionais

        RuleFor(c => c.Titulo)
            .NotEmpty().WithMessage("O campo Título é obrigatório.");

        RuleFor(c => c.AreaDeAtuacao)
            .NotEmpty().WithMessage("O campo Área de atuação é obrigatório.");

        RuleFor(c => c.ResumoProfissional)
            .NotEmpty().WithMessage("O campo Resumo profissional é obrigatório.")
            .MaximumLength(300).WithMessage("O campo Resumo profissional deve conter 300 caracteres.");
        
        RuleFor(c => c.Linkedin)
            .NotEmpty().When(c => !string.IsNullOrEmpty(c.Linkedin)).WithMessage("Por favor, insira uma URL válida.")
            .Must(SerUmaUrlValida).When(c => !string.IsNullOrEmpty(c.Linkedin)).WithMessage("Por favor, insira uma URL válida.")
            .Must(SerUmaUrlValidaDoLinkedIn).When(c => !string.IsNullOrEmpty(c.Linkedin)).WithMessage("Por favor, insira um link válido do LinkedIn.");

        RuleFor(c => c.Portfolio)
            .NotEmpty().When(c => !string.IsNullOrEmpty(c.Portfolio)).WithMessage("Por favor, insira uma URL válida.")
            .Must(SerUmaUrlValida).When(c => !string.IsNullOrEmpty(c.Portfolio)).WithMessage("Por favor, insira uma URL válida.");

        
        RuleFor(curriculo => curriculo)
            .Must(c => c.Clt || c.Pj || c.Temporario)
            .WithMessage("Selecione pelo menos um tipo de contrato pretendido (CLT, PJ, Temporário).");
        
        RuleFor(curriculo => curriculo)
            .Must(c => c.Presencial || c.Remoto || c.Hibrido)
            .WithMessage("Selecione pelo menos uma modalidade de trabalho pretendida (Presencial, Remoto, Híbrido).");
    }
    
    private bool SerUmaUrlValida(string? url)
{
    if (string.IsNullOrEmpty(url))
    {
        return false;
    }

    if (Uri.TryCreate(url, UriKind.Absolute, out Uri? resultado))
    {
        return resultado.Scheme == Uri.UriSchemeHttp || resultado.Scheme == Uri.UriSchemeHttps;
    }

    return false;
}

private bool SerUmaUrlValidaDoLinkedIn(string? url)
{
    if (string.IsNullOrEmpty(url))
    {
        return false;
    }

    var regex = new Regex(@"^https:\/\/(www\.)?linkedin\.com\/in\/[a-zA-Z0-9-]+\/?$");
    return regex.IsMatch(url);
}
    
    private bool SerUmGrauDeFormacaoValido(string grauDeFormacao)
    {
        var validOptions = new[]
        {
            "Ensino Fundamental", "Ensino Fundamental Incompleto", "Ensino Médio",
            "Ensino Médio Incompleto", "Superior incompleto", "Superior completo",
            "Pós graduação", "Mestrado", "Doutorado"
        };

        return validOptions.Contains(grauDeFormacao);
    }
}