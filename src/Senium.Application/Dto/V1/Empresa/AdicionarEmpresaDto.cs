﻿namespace Senium.Application.Dto.V1.Empresa;

public class AdicionarEmpresaDto
{
    public string Nome { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string Telefone { get; set; } = null!;
    public string NomeDaEmpresa { get; set; } = null!;
}