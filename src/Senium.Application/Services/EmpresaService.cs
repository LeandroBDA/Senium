using AutoMapper;
using Senium.Application.Contracts.Services;
using Senium.Application.Dto.V1.Empresa;
using Senium.Application.Notifications;
using Senium.Domain.Contracts.Repositories;
using Senium.Domain.Entities;

namespace Senium.Application.Services;

public class EmpresaService : BaseService, IEmpresaService
{
    private readonly IEmpresaRepository _empresaRepository;
    
    public EmpresaService(INotificator notificator,
        IMapper mapper,
        IEmpresaRepository empresaRepository)
        : base(notificator, mapper)
    {
        _empresaRepository = empresaRepository;
    }

    public async Task<EmpresaDto?> AdicionarEmpresa(AdicionarEmpresaDto dto)
    {
        var empresa = Mapper.Map<Empresa>(dto);

        if (!await Validar(empresa))
        {
            return null;
        }
        
        _empresaRepository.CadastrarEmpresa(empresa);

        if (await _empresaRepository.UnitOfWork.Commit())
        {
            return Mapper.Map<EmpresaDto>(empresa);
        }
        
        Notificator.Handle("Não foi possível cadastrar a empresa");
        return null;
    }

    private async Task<bool> Validar(Empresa empresa)
    {
        if (!empresa.Validar(out var validationResult))
        {
            Notificator.Handle(validationResult.Errors);
        }
        
        var usuarioExistente = await _empresaRepository.FirstOrDefault(a => 
            a.Email == empresa.Email 
            && a.Telefone == empresa.Telefone
            && a.NomeDaEmpresa == empresa.NomeDaEmpresa
            && a.Id != empresa.Id);
        
        if (usuarioExistente != null)
        {
            Notificator.Handle($"Já existe uma emperesa cadastrada com essas indentificações!");
        }

        return !Notificator.HasNotification;
    }
}