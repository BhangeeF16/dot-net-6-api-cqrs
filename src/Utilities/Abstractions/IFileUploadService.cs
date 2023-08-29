using Microsoft.AspNetCore.Http;

namespace Utilities.Abstractions;

public interface IFileUploadService
{
    string GetMimeType(string fileName);
    string GetFileCompleteUrl(string File);
    string UploadFile(IFormFile file, string DirectoryName = "DEFAULT");
    bool DeleteFile(string File);
}