using Microsoft.AspNetCore.Http;
using Senium.Core.Enums;

namespace Senium.Application.Contracts.Services;

public interface IFileService
{
    Task<string?> UploadPhoto(IFormFile arquivo, EUploadPath uploadPath, EPathAccess pathAccess = EPathAccess.Public, int urlLimitLength = 255);
    Task<string?> UploadPdf(IFormFile arquivo, EUploadPath uploadPath, EPathAccess pathAccess = EPathAccess.Public, int urlLimitLength = 255);
    string ObterPath(Uri uri);
    bool Apagar(Uri uri);
}