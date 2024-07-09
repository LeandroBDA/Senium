using AutoMapper;
using Microsoft.AspNetCore.Http;
using Senium.Application.Contracts.Services;
using Senium.Application.Dto.V1.Curriculo;
using Senium.Application.Notifications;
using Senium.Core.Enums;
using Senium.Core.Extensions;
using Senium.Domain.Contracts.Repositories;
using Senium.Domain.Entities;

namespace Senium.Application.Services;

public class CurriculoService : BaseService, ICurriculoService
{
    private readonly ICurriculoRepository _curriculoRepository;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IFileService _fileService;
    
    public CurriculoService(INotificator notificator, IMapper mapper, IHttpContextAccessor httpContextAccessor,ICurriculoRepository curriculoRepository, IFileService fileService) : base(notificator, mapper) 
    {
        _httpContextAccessor = httpContextAccessor;
        _curriculoRepository = curriculoRepository;
        _fileService = fileService;
    }
    
    public async Task<CurriculoDto?> ObterCurriculoPorUsuarioId(int id)
    {
        var curriculo = await _curriculoRepository.ObterCurriculoPorUsuarioId(id);
        if (curriculo != null)
        {
            return Mapper.Map<CurriculoDto>(curriculo);
        }
        
        Notificator.HandleNotFoundResource();
        return null;
    }

    public async Task<CurriculoDto?> AdicionarCurriculo(AdicionarCurriculoDto curriculodto)
    {
        var curriculo = Mapper.Map<Curriculo>(curriculodto);
        curriculo.UsuarioId = _httpContextAccessor.ObterUsuarioId() ?? 0;

        if (!await Validar(curriculo))
        {
            return null;
        }

        if (curriculodto.Pdf is not null)
        {
            const long maxSizeInBytes = 5 * 1024 * 1024;
            if (curriculodto.Pdf.Length > maxSizeInBytes)
            {
                Notificator.Handle($"O arquivo deve ter no máximo {maxSizeInBytes / (1024 * 1024)} MB.");
                return null;
            }

            try
            {
                curriculo.Pdf = await _fileService.UploadPdf(curriculodto.Pdf, EUploadPath.PdfUsuarios);
            }
            catch (Exception ex)
            {
                Notificator.Handle($"Erro ao fazer upload do PDF: {ex.Message}");
                return null;
            }
        }
        
        if (curriculodto.Photo is not null)
        {
            const long maxFileSizeInBytes = 2 * 1024 * 1024;
            if (curriculodto.Photo.Length > maxFileSizeInBytes)
            {
                Notificator.Handle($"O arquivo deve ter no máximo {maxFileSizeInBytes / (1024 * 1024)} MB.");
                return null;
            }
            
            try
            {
                curriculo.Photo = await _fileService.UploadPhoto(curriculodto.Photo, EUploadPath.FotoUsuarios);
            }
            catch (Exception ex)
            {
                Notificator.Handle($"Erro ao fazer upload da Foto: {ex.Message}");
                return null;
            }
        }

        _curriculoRepository.AdicionarCurriculo(curriculo);

        if (await _curriculoRepository.UnitOfWork.Commit())
        {
            return Mapper.Map<CurriculoDto>(curriculo);
        }

        Notificator.Handle("Não foi possível adicionar um curriculo.");
        return null;
    }


    public async Task<CurriculoDto?> AtualizarCurriculo(int id, AtualizarCurriculoDto curriculoDto)
    {
        if (id != curriculoDto.UsuarioId)
        {
            Notificator.Handle("Os IDs não conferem");
            return null;
        }

        var curriculo = await _curriculoRepository.ObterCurriculoPorUsuarioId(id);
        if (curriculo == null)
        {
            Notificator.HandleNotFoundResource();
            return null;
        }
        
        Mapper.Map(curriculoDto, curriculo);
        
        if (curriculoDto.Pdf is not null)
        {
            const long maxSizeInBytes = 5 * 1024 * 1024;
            if (curriculoDto.Pdf.Length > maxSizeInBytes)
            {
                Notificator.Handle($"O arquivo deve ter no máximo {maxSizeInBytes / (1024 * 1024)} MB.");
                return null;
            }
            
            if (!await ManterPdf(curriculoDto.Pdf, curriculo))
            {
                return null;
            }
        }
        
        if (curriculoDto.Photo is not null)
        {
            const long maxSizeInBytes = 2 * 1024 * 1024;
            if (curriculoDto.Photo.Length > maxSizeInBytes)
            {
                Notificator.Handle($"O arquivo deve ter no máximo {maxSizeInBytes / (1024 * 1024)} MB.");
                return null;
            }
            
            if (!await ManterPhoto(curriculoDto.Photo, curriculo))
            {
                return null;
            }
        }

        if (!await ValidarAtualizacao(curriculo))
        {
            return null;
        }

        _curriculoRepository.AtualizarCurriculo(curriculo);

        if (await _curriculoRepository.UnitOfWork.Commit())
        {
            return Mapper.Map<CurriculoDto>(curriculo);
        }

        Notificator.Handle("Não foi possível alterar o currículo");
        return null;
    }

    public async Task<List<CurriculoDto>?> ObterTodosCurriculo()
    {
        var curriculos = await _curriculoRepository.ObterTodosCurriculo();
        if (curriculos == null || !curriculos.Any())
        {
            Notificator.HandleNotFoundResource();
            return null;
        }

        return Mapper.Map<List<CurriculoDto>>(curriculos);
    }


    public async Task<bool> Validar(Curriculo curriculo)
    {
        if (!curriculo.Validar(out var validationResult))
        {
            Notificator.Handle(validationResult.Errors);
            return false; 
        }
        
        var curriculoExistente = await _curriculoRepository.FirstOrDefault(a => a.UsuarioId == curriculo.UsuarioId);
        if (curriculoExistente != null)
        {
            Notificator.Handle("Já existe um currículo para este usuário!");
            return false; 
        }


        return !Notificator.HasNotification;
    }
    
    public Task<bool> ValidarAtualizacao(Curriculo curriculo)
    {
        if (!curriculo.Validar(out var validationResult))
        {
            Notificator.Handle(validationResult.Errors);
            return Task.FromResult(false);
        }

        return Task.FromResult(!Notificator.HasNotification);
    }
    
    private async Task<bool> ManterPdf(IFormFile pdf, Curriculo curriculo)
    {
        if (!string.IsNullOrWhiteSpace(curriculo.Pdf) && Uri.TryCreate(curriculo.Pdf, UriKind.Absolute, out Uri? pdfUri) && !_fileService.Apagar(pdfUri))
        {
            Notificator.Handle("Não foi possível remover o PDF anterior.");
            return false;
        }

        curriculo.Pdf = await _fileService.UploadPdf(pdf, EUploadPath.PdfUsuarios);
        return true;
    }
    
    private async Task<bool> ManterPhoto(IFormFile photo, Curriculo curriculo)
    {
        if (!string.IsNullOrWhiteSpace(curriculo.Photo) && Uri.TryCreate(curriculo.Photo, UriKind.Absolute, out Uri? photoUri) && !_fileService.Apagar(photoUri))
        {
            Notificator.Handle("Não foi possível remover a Foto anterior.");
            return false;
        }

        curriculo.Photo = await _fileService.UploadPhoto(photo, EUploadPath.FotoUsuarios);
        return true;
    }
}