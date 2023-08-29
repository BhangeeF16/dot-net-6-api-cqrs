using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.StaticFiles;
using System.Net.Http.Headers;
using Utilities.Abstractions;

namespace Utilities.Services;

public class FileUploadService : IFileUploadService
{
    #region Constructors And Locals

    protected readonly string BaseUrl;
    protected readonly string PhysicalPath;
    public const string FolderName = "wwwroot//Uploads";
    //private readonly IWebHostEnvironment _webHostEnvironment;
    private readonly IHttpContextAccessor _httpContext;
    //public FileUploadService(IWebHostEnvironment webHostEnvironment, IHttpContextAccessor httpContext)
    //{
    //    _webHostEnvironment = webHostEnvironment;
    //    _httpContext = httpContext;

    //    BaseUrl = $"{_httpContext.HttpContext.Request.Scheme}://{_httpContext.HttpContext.Request.Host}";
    //    PhysicalPath = _webHostEnvironment.ContentRootPath;

    //    if (!Directory.Exists(FolderName))
    //    {
    //        Directory.CreateDirectory(FolderName);
    //    }
    //}

    #endregion

    public string GetMimeType(string fileName)
    {
        var provider = new FileExtensionContentTypeProvider();
        string contentType;
        if (!provider.TryGetContentType(fileName, out contentType))
        {
            contentType = "application/octet-stream";
        }
        return contentType;
    }
    public bool DeleteFile(string Image)
    {
        var pathToDelete = Path.Combine(BaseUrl, "wwwroot");
        var fullPathUrl = Path.Combine(pathToDelete, Image);
        var file = new FileInfo(fullPathUrl);
        if (file.Exists)
        {
            file.Delete();
            return true;
        }
        else
        {
            return false;
        }
    }
    public string GetFileCompleteUrl(string Image)
    {
        return $"{BaseUrl}//{Image}".Replace("\\", "//");
    }
    public string UploadFile(IFormFile file, string DirectoryName = "DEFAULT")
    {
        var pathToSave = $"{PhysicalPath}//{FolderName}//{DirectoryName}";
        var responsePath = $"{BaseUrl}//{FolderName}//{DirectoryName}";

        if (file.Length > 0)
        {
            var existingFileName = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.ToString().Replace("\"", "");
            var fileExtension = Path.GetExtension(existingFileName);

            var fileName = existingFileName?.Replace(existingFileName, Guid.NewGuid().ToString());
            fileName = fileName.Insert(fileName.Length, fileExtension);

            var fullPath = $"{pathToSave}//{fileName}";
            UploadFile(fullPath, file);

            var filePathUsedToDisplay = $"{responsePath}//{fileName}";
            var filePathToSaveInDB = $"Uploads//{fileName}";
            return filePathToSaveInDB;
        }
        return string.Empty;
    }
    private static bool UploadFile(string savingPath, IFormFile file)
    {
        var check = false;
        if (!Directory.Exists(savingPath))
        {
            Directory.CreateDirectory(savingPath);
        }
        using (var stream = new FileStream(savingPath.Replace("//", "\\"), FileMode.Create))
        {
            file.CopyTo(stream);
            check = true;
        }
        return check;
    }
}
