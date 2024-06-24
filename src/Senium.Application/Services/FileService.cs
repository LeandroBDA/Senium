using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Senium.Application.Contracts.Services;
using Senium.Application.Notifications;
using Senium.Core.Enums;
using Senium.Core.Extensions;
using Senium.Core.Settings;

namespace Senium.Application.Services;

public class FileService : BaseService, IFileService
{
    private readonly AppSettings _appSettings;
    private readonly UploadSettings _uploadSettings;
    
    private const long MaxFileSizeInBytes = 5 * 1024 * 1024; // 5 MB
    
    public FileService(INotificator notificator, IMapper mapper, IOptions<AppSettings> appSettings, IOptions<UploadSettings> uploadSettings) : base(notificator, mapper)
    {
        _appSettings = appSettings.Value;
        _uploadSettings = uploadSettings.Value;
    }
    
    public async Task<string?> UploadPhoto(IFormFile arquivo, EUploadPath uploadPath,
        EPathAccess pathAccess = EPathAccess.Private, int urlLimitLength = 255)
    {
        const long maxFileSizeInBytes = 2 * 1024 * 1024;
       
        var permittedExtensions = new[] { ".jpg", ".jpeg", ".png" };
        var ext = Path.GetExtension(arquivo.FileName).ToLowerInvariant();

        if (string.IsNullOrEmpty(ext) || !permittedExtensions.Contains(ext))
        {
            Notificator.Handle("Apenas arquivos JPEG e PNG são permitidos");
            return null;
        }
        
        if (arquivo.Length > maxFileSizeInBytes)
        {
            Notificator.Handle($"O arquivo deve ter no máximo {maxFileSizeInBytes / (1024 * 1024)} MB (2 MB).");
            return null; 
        }
        
        var fileName = GenerateNewFileName(arquivo.FileName, pathAccess, EUploadPath.FotoUsuarios, urlLimitLength);
        var filePath = MountFilePath(fileName, pathAccess, EUploadPath.FotoUsuarios);

        try
        {
            await File.WriteAllBytesAsync(filePath, ConvertFileInByteArray(arquivo));
        }
        catch (DirectoryNotFoundException)
        {
            var file = new FileInfo(filePath);
            file.Directory?.Create();
            await File.WriteAllBytesAsync(filePath, ConvertFileInByteArray(arquivo));
        }

        return GetFileUrl(fileName, pathAccess, EUploadPath.FotoUsuarios);
    }

    public async Task<string?> UploadPdf(IFormFile arquivo, EUploadPath uploadPath, EPathAccess pathAccess = EPathAccess.Private, int urlLimitLength = 255)
    {
        
        if (arquivo.Length > MaxFileSizeInBytes)
        {
            Notificator.Handle($"O arquivo deve ter no máximo {MaxFileSizeInBytes / (1024 * 1024)} MB.");
            return null; // Retorna null para indicar que o upload não foi realizado
        }

        var fileName = GenerateNewFileName(arquivo.FileName, pathAccess, EUploadPath.PdfUsuarios, urlLimitLength);
        var filePath = MountFilePath(fileName, pathAccess, EUploadPath.PdfUsuarios);

        try
        {
            await File.WriteAllBytesAsync(filePath, ConvertFileInByteArray(arquivo));
        }
        catch (DirectoryNotFoundException)
        {
            var file = new FileInfo(filePath);
            file.Directory?.Create();
            await File.WriteAllBytesAsync(filePath, ConvertFileInByteArray(arquivo));
        }

        return GetFileUrl(fileName, pathAccess, EUploadPath.PdfUsuarios);
    }


    public string ObterPath(Uri uri)
    {
        return GetFilePath(uri);
    }

    public bool Apagar(Uri uri)
    {
        try
        {
            var filePath = GetFilePath(uri);

            new FileInfo(filePath).Delete();
            return true;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return false;
        }
    }

    private string GenerateNewFileName(string fileName, EPathAccess pathAccess, EUploadPath uploadPath, int limit = 255)
    {
        var guid = Guid.NewGuid().ToString("N");
        var newFileName = fileName.Replace("-", "");
        var url = GetFileUrl($"{guid}_{newFileName}", pathAccess, uploadPath);

        if (url.Length <= limit)
        {
            return newFileName;
        }

        var remove = url.Length - limit;
        newFileName = newFileName.Remove(newFileName.Length - remove - Path.GetExtension(newFileName).Length, remove);

        return $"{guid}_{newFileName}";
    }

    private string MountFilePath(string fileName, EPathAccess pathAccess, EUploadPath uploadPath)
    {
        var path = pathAccess == EPathAccess.Private ? _uploadSettings.PrivateBasePath : _uploadSettings.PublicBasePath;
        return Path.Combine(path, uploadPath.ToDescriptionString(), fileName);
    }

    private string GetFileUrl(string fileName, EPathAccess pathAccess, EUploadPath uploadPath)
    {
        return Path.Combine(_appSettings.UrlApi, pathAccess.ToDescriptionString(), uploadPath.ToDescriptionString(),
            fileName);
    }

    private string GetBaseFileUrl(EPathAccess pathAccess, EUploadPath uploadPath)
    {
        return Path.Combine(_appSettings.UrlApi, pathAccess.ToDescriptionString(), uploadPath.ToDescriptionString());
    }

    private string GetFilePath(Uri uri)
    {
        var filePath = uri.AbsolutePath;
        if (filePath.StartsWith("/"))
        {
            filePath = filePath.Remove(0, 1);
        }

        var pathAccessStr = filePath.Split("/")[1];
        var pathAccess = Enum.Parse<EPathAccess>(pathAccessStr, true);
        filePath = filePath.Remove(0, pathAccess.ToDescriptionString().Length);
        if (filePath.StartsWith("/"))
        {
            filePath = filePath.Remove(0, 1);
        }

        var basePath = pathAccess == EPathAccess.Private
            ? _uploadSettings.PrivateBasePath
            : _uploadSettings.PublicBasePath;

        return Path.Combine(basePath, filePath);
    }

    private static byte[] ConvertFileInByteArray(IFormFile file)
    {
        using var memoryStream = new MemoryStream();
        file.CopyTo(memoryStream);
        return memoryStream.ToArray();
    }


}