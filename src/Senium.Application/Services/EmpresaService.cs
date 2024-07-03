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

    public async Task<EmpresaDto?> AdicionarEmpresa(AdicionarEmpresaDto empresaDto)
    {
        var empresa = Mapper.Map<Empresa>(empresaDto);

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
    
    public async Task<EmpresaDto?> AtualizarEmpresa(int id, AtualizarEmpresaDto empresaDto)
    {
        if (id != empresaDto.Id)
        {
            Notificator.Handle("Os Ids não conferem");
            return null;
        }
        
        var empresa = await _empresaRepository.ObterEmpresaPorId(id);
        if (empresa == null)
        {
            Notificator.HandleNotFoundResource();
            return null;
        }
        
        Mapper.Map(empresaDto, empresa);
        
        if (!await ValidarAtualizacao(empresa))
        {
            return null;
        }
        
        _empresaRepository.AtualizarEmpresa(empresa);
      
        if (await _empresaRepository.UnitOfWork.Commit())
        {
            return Mapper.Map<EmpresaDto>(empresa);
        }
      
        return null;
    }
    public Task<bool> ValidarAtualizacao(Empresa empresa)
    {
        if (!empresa.Validar(out var validationResult))
        {
            Notificator.Handle(validationResult.Errors);
            return Task.FromResult(false);
        }

        return Task.FromResult(!Notificator.HasNotification);
    }
    
    public async Task<List<EmpresaDto>?> ObterTodasEmpresas()
    {
        var empresa = await _empresaRepository.ObterTodasEmpresas();
       
        if (empresa != null)
        {
            return Mapper.Map<List<EmpresaDto>>(empresa);
        }
        
        Notificator.HandleNotFoundResource();
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
            Notificator.Handle($"Já existe uma empresa cadastrada com essas indentificações!");
        }

        return !Notificator.HasNotification;
    }
}