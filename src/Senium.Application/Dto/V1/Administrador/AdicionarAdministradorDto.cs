﻿namespace Senium.Application.Dto.V1.Administrador;

public class AdicionarAdministradorDto
{
    public string Nome { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string Senha { get; set; } = null!;
}