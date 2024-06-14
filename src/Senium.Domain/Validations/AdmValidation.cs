using FluentValidation;
using Senium.Domain.Contracts.Repositories;
using Senium.Domain.Entities;

namespace Senium.Domain.Validations;

public class AdmValidation : AbstractValidator<Administrador>
{
    private readonly IAdmRepository _admRepository;
    private static readonly Dictionary<string, (int Attempts, DateTime? LockoutEnd)> _loginAttempts = new Dictionary<string, (int, DateTime?)>();
    private IAdmRepository admRepository;
    public AdmValidation()
    {
        _admRepository = admRepository;
            
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("O campo E-mail é obrigatório.")
            .Matches(@"^[^@\s]+@[^@\s]+\.[^@\s]+$").WithMessage("O campo E-mail deve ser um endereço de e-mail válido.");

      
        RuleFor(x => x.Senha)
            .NotEmpty().WithMessage("O campo Senha é obrigatório.")
            .Length(8, 20).WithMessage("A senha deve ter entre 8 e 20 caracteres.")
            .Matches(@"^(?=.*[a-zA-Z])(?=.*\d)(?=.*[^a-zA-Z0-9]).{8,20}$")
            .WithMessage("A senha deve conter letras, números, símbolos e ter entre 8 e 20 caracteres.");
        
        RuleFor(x => x)
            .Must(ValidarAdministrador)
            .WithMessage("E-mail ou senha incorretos.");
    }
    
    private bool ValidarAdministrador(Administrador administrador)
    {
        if (_loginAttempts.TryGetValue(administrador.Email, out var attemptInfo))
        {
            if (attemptInfo.LockoutEnd.HasValue && DateTime.UtcNow < attemptInfo.LockoutEnd.Value)
            {
                Console.WriteLine("Conta bloqueada temporariamente devido a múltiplas tentativas malsucedidas.");
                return false;
            }
        }
        
        var admin = _admRepository.VerificarAdmPorEmail(administrador.Email);
        if (admin == null || !BCrypt.Net.BCrypt.Verify(administrador.Senha, administrador.HashedPassword))
        {
            if (_loginAttempts.ContainsKey(administrador.Email))
            {
                _loginAttempts[administrador.Email] = (attemptInfo.Attempts + 1, attemptInfo.LockoutEnd);
            }
            else
            {
                _loginAttempts[administrador.Email] = (1, null);
            }
            
            if (_loginAttempts[administrador.Email].Attempts >= 3)
            {
                _loginAttempts[administrador.Email] = (0, DateTime.UtcNow.AddMinutes(10)); // Bloqueado por 10 min
                Console.WriteLine("Conta bloqueada temporariamente devido a múltiplas tentativas malsucedidas.");
                Task.Delay(5000);
            }
            
            return false;
        }
    }
}