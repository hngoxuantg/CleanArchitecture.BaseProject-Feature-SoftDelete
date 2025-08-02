using Microsoft.AspNetCore.Http;

namespace Project.Application.Interfaces.IExternalServices
{
    public interface IFileService
    {
        Task<string> SaveImage(IFormFile formFile, string folder, CancellationToken cancellationToken = default);
        Task<List<string>> SaveImages(IList<IFormFile> formFiles, string folder, CancellationToken cancellationToken = default);
        bool DeleteFile(string filePath);
        bool DeleteFiles(IList<string> filePaths);
        bool FileExits(string filePath);
        string GetFileUrl(string relativePath);
        bool IsValidImage(IFormFile formFile);
    }
}