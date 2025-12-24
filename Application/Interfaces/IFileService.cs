using Domain.Entities;
using Domain.Wrappers;
using Microsoft.AspNetCore.Http;

namespace Application.Interfaces
{
    public interface IFileService
    {
        // ===================== UPLOAD =====================

        Task<ServiceResult<string>> UploadFileAsync(
            IFormFile file,
            string controllerName,
            string userId
        );

        Task<ServiceResult<List<string>>> UploadFilesAsync(
            IEnumerable<IFormFile> files,
            string controllerName,
            string userId
        );

        // ===================== DOWNLOAD =====================

        Task<
            ServiceResult<(byte[] FileBytes, string FileName, string ContentType)>
        > DownloadFileAsync(string filePath, string userId);

        // ===================== DELETE =====================

        Task<ServiceResult<bool>> DeleteFileAsync(string filePath, string userId);

        // ===================== READ =====================

        Task<ServiceResult<FileRecord?>> GetFileByIdAsync(string fileId, string userId);

        Task<ServiceResult<FileRecord?>> GetFileByPathAsync(string relativePath, string userId);

        Task<ServiceResult<IEnumerable<FileRecord>>> GetFilesByUserAsync(string userId);

        // ===================== STORAGE =====================

        Task<ServiceResult<long>> GetTotalStorageUsedAsync(string userId);
        Task<ServiceResult<IEnumerable<FileRecord>>> GetPDFFilesByUserAsync(string userId);
    }
}
