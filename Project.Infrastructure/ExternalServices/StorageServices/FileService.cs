using Project.Application.Interfaces.IExternalServices;
using Microsoft.AspNetCore.Http;

namespace Project.Infrastructure.ExternalServices.StorageServices
{
    public class FileService : IFileService
    {
        private readonly string _baseDirectory;
        private readonly string[] _allowedExtensions = { ".jpg", ".jpeg", ".png", ".gif", ".bmp" };
        private readonly long _maxFileSize = 5 * 1024 * 1024;
        public FileService()
        {
            _baseDirectory = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
            Directory.CreateDirectory(_baseDirectory);
        }
        public async Task<string> SaveImageAsync(IFormFile formFile, string folder, CancellationToken cancellationToken = default)
        {
            if (!IsValidImage(formFile))
                throw new Exception("File is invalid or empty.");
            string folderPath = Path.Combine(_baseDirectory, folder);
            string fileName = $"{Guid.NewGuid()}{Path.GetExtension(formFile.FileName)}";
            string filePath = Path.Combine(folderPath, fileName);
            Directory.CreateDirectory(folderPath);
            using (FileStream fileStream = new FileStream(filePath, FileMode.Create))
            {
                await formFile.CopyToAsync(fileStream, cancellationToken);
            }
            return Path.Combine("/uploads", folder, fileName).Replace("\\", "/");
        }
        public async Task<List<string>> SaveImagesAsync(IList<IFormFile> formFiles, string folder, CancellationToken cancellationToken = default)
        {
            List<string> filePaths = new List<string>();
            for (int i = 0; i < formFiles.Count; i++)
            {
                if (IsValidImage(formFiles[i]))
                {
                    filePaths.Add(await SaveImageAsync(formFiles[i], folder, cancellationToken));
                }
            }
            return filePaths;
        }
        public bool IsValidImage(IFormFile formFile)
        {
            if (formFile == null || formFile.Length == 0)
                return false;
            if (formFile.Length > _maxFileSize)
                return false;
            string extension = Path.GetExtension(formFile.FileName).ToLowerInvariant();
            return _allowedExtensions.Contains(extension);
        }
        public bool DeleteFile(string filePath)
        {
            string relativePath = filePath.TrimStart('/', '\\');
            if (relativePath.StartsWith("uploads", StringComparison.OrdinalIgnoreCase))
            {
                relativePath = relativePath.Substring("uploads".Length).TrimStart('/', '\\');
            }
            string fullPath = Path.Combine(_baseDirectory, relativePath);
            if (File.Exists(fullPath))
            {
                try
                {
                    File.Delete(fullPath);
                    return true;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }
            }
            return false;
        }
        public bool FileExists(string filePath)
        {
            return File.Exists(Path.Combine(_baseDirectory, filePath).Replace("/", "\\"));
        }
        public bool DeleteFiles(IList<string> filePaths)
        {
            bool allDeleted = true;
            for (int i = 0; i < filePaths.Count; i++)
            {
                if (!DeleteFile(filePaths[i]))
                    allDeleted = false;
            }
            return allDeleted;
        }
        public string GetFileUrl(string relativePath)
        {
            return $"/uploads/{relativePath}";
        }
    }
}