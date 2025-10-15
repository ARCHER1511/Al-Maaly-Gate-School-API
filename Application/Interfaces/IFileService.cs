using Domain.Entities;
using Domain.Wrappers;
using Microsoft.AspNetCore.Http;

namespace Application.Interfaces
{
    public interface IFileService
    {
        Task<ServiceResult<string>> UploadFileAsync(IFormFile file,string controllerName);
        Task<ServiceResult<List<string>>> UploadFilesAsync(IEnumerable<IFormFile> files,string controllerName);
        Task<ServiceResult<string>> ReplaceFileAsync(IFormFile newFile,string existingFilePath,string controllerName);
        Task<ServiceResult<bool>> DeleteFileAsync(string filePath);


        Task<ServiceResult<FileRecord?>> GetFileByIdAsync(string id);
        Task<ServiceResult<FileRecord?>> GetFileByPathAsync(string relativePath);
        Task<ServiceResult<IEnumerable<FileRecord>>> GetAllFilesAsync();
        Task<ServiceResult<IEnumerable<FileRecord>>> GetFilesByControllerAsync(string controllerName);
        Task<ServiceResult<IEnumerable<FileRecord>>> GetFilesByTypeAsync(string fileType);
        Task<ServiceResult<IEnumerable<FileRecord>>> GetRecentFilesAsync(int days = 7);
        Task<ServiceResult<long>> GetTotalStorageUsedAsync();
        Task<ServiceResult<int>> DeleteFilesByControllerAsync(string controllerName);
        Task<ServiceResult<(byte[] FileBytes, string FileName, string ContentType)>> DownloadFileAsync(string filePath);
    }
}
