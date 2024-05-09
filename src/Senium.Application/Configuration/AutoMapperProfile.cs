using AutoMapper;
using Senium.Application.Dto.V1.Empresa;
using Senium.Application.Dto.V1.Usuario;
using Senium.Domain.Entities;

namespace Senium.Application.Configuration;

public class AutoMapperProfile : Profile
{
    public AutoMapperProfile()
    {
        #region Usuario

        CreateMap<UsuarioDto, Usuario>().ReverseMap();
        CreateMap<AdicionarUsuarioDto, Usuario>().ReverseMap();

        #endregion

        #region Empresa

        CreateMap<EmpresaDto, Empresa>().ReverseMap();
        CreateMap<AdicionarEmpresaDto, Empresa>().ReverseMap();

        #endregion
    }
}