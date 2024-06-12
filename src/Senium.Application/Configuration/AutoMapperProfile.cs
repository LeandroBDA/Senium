using AutoMapper;
using Senium.Application.Dto.V1.Administrador;
using Senium.Application.Dto.V1.Curriculo;
using Senium.Application.Dto.V1.Empresa;
using Senium.Application.Dto.V1.Experiencia;
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
        CreateMap<AdicionarEmpresaDto, Empresa>();

        #endregion
        
        #region Curriculo

        CreateMap<CurriculoDto, Curriculo>().ReverseMap();
        CreateMap<AdicionarCurriculoDto, Curriculo>().ReverseMap();
        CreateMap<AtualizarCurriculoDto, Curriculo>();
        
        #endregion
        
        #region Experiencia

        CreateMap<ExperienciaDto, Experiencia>().ReverseMap();
        CreateMap<AdicionarExperienciaDto, Experiencia>().ReverseMap();
        CreateMap<AtualizarExperienciaDto, Experiencia>().ReverseMap();

        #endregion

        #region Administrador


        CreateMap<AdministradorDto, Administrador>().ReverseMap();
        CreateMap<AdicionarAdministradorDto, Administrador>().ReverseMap();
        CreateMap<AtualizarAdministradorDto, Administrador>().ReverseMap();

        #endregion

    }
}