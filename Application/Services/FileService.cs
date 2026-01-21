using Application.Interfaces;
using Domain.Entities;
using Domain.Wrappers;
using Infrastructure.Interfaces;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;

namespace Application.Services
{
    public class FileService : IFileService
    {
        private readonly string _rootPath;
        private readonly IUnitOfWork _unitOfWork;

        private static readonly string[] _allowedExtensions =
        {
            ".jpg",
            ".jpeg",
            ".png",
            ".gif",
            ".pdf",
            ".doc",
            ".docx",
        };

        public FileService(IWebHostEnvironment env, IUnitOfWork unitOfWork)
        {
            _rootPath = Path.Combine(env.WebRootPath, "uploads");
            _unitOfWork = unitOfWork;
        }

        public async Task<ServiceResult<FileRecord?>> GetFileByPathAsync(
            string relativePath,
            string userId
        )
        {
            if (string.IsNullOrWhiteSpace(relativePath))
                return ServiceResult<FileRecord?>.Fail("File path is required");

            if (string.IsNullOrWhiteSpace(userId))
                return ServiceResult<FileRecord?>.Fail("UserId is required");

            var file = await _unitOfWork.FileRecordRepository.GetByPathAsync(relativePath, userId);

            if (file == null)
                return ServiceResult<FileRecord?>.Fail("File not found");

            return ServiceResult<FileRecord?>.Ok(file, "File retrieved successfully");
        }

        public async Task<ServiceResult<FileRecord?>> GetFileByIdAsync(string fileId, string userId)
        {
            if (string.IsNullOrWhiteSpace(fileId))
                return ServiceResult<FileRecord?>.Fail("FileId is required");

            if (string.IsNullOrWhiteSpace(userId))
                return ServiceResult<FileRecord?>.Fail("UserId is required");

            var file = await _unitOfWork.FileRecordRepository.GetByIdAsync(fileId, userId);

            if (file == null)
                return ServiceResult<FileRecord?>.Fail("File not found");

            return ServiceResult<FileRecord?>.Ok(file, "File retrieved successfully");
        }

        // ===================== UPLOAD SINGLE =====================
        public async Task<ServiceResult<string>> UploadFileAsync(
            IFormFile file,
            string controllerName,
            string userId
        )
        {
            if (file == null || file.Length == 0)
                return ServiceResult<string>.Fail("File cannot be empty.");

            var extension = ValidateExtension(file.FileName);
            if (extension == null)
                return ServiceResult<string>.Fail("File type not supported.");

            var (relativePath, uniqueFileName) = await SavePhysicalFileAsync(
                file,
                controllerName,
                extension
            );

            var record = CreateFileRecord(
                file,
                uniqueFileName,
                relativePath,
                controllerName,
                extension,
                userId
            );

            await _unitOfWork.FileRecordRepository.AddAsync(record);
            await _unitOfWork.SaveChangesAsync();

            return ServiceResult<string>.Ok(relativePath, "Uploaded successfully");
        }

        // ===================== UPLOAD MULTIPLE =====================

        public async Task<ServiceResult<List<string>>> UploadFilesAsync(
            IEnumerable<IFormFile> files,
            string controllerName,
            string userId
        )
        {
            if (files == null || !files.Any())
                return ServiceResult<List<string>>.Fail("No files provided.");

            var records = new List<FileRecord>();
            var paths = new List<string>();

            foreach (var file in files)
            {
                if (file == null || file.Length == 0)
                    continue;

                var extension = ValidateExtension(file.FileName);
                if (extension == null)
                    continue;

                var (relativePath, uniqueFileName) = await SavePhysicalFileAsync(
                    file,
                    controllerName,
                    extension
                );

                records.Add(
                    CreateFileRecord(
                        file,
                        uniqueFileName,
                        relativePath,
                        controllerName,
                        extension,
                        userId
                    )
                );

                paths.Add(relativePath);
            }

            if (records.Any())
            {
                await _unitOfWork.FileRecordRepository.AddRangeAsync(records);
                await _unitOfWork.SaveChangesAsync();
            }

            return ServiceResult<List<string>>.Ok(paths, "Files uploaded successfully");
        }

        // ===================== DOWNLOAD =====================

        public async Task<
            ServiceResult<(byte[] FileBytes, string FileName, string ContentType)>
        > DownloadFileAsync(string filePath, string userId)
        {
            var record = await _unitOfWork.FileRecordRepository.GetByPathAsync(filePath, userId);
            if (record == null)
                return ServiceResult<(byte[], string, string)>.Fail("File not found.");

            var fullPath = GetPhysicalPath(filePath);
            if (!File.Exists(fullPath))
                return ServiceResult<(byte[], string, string)>.Fail("Physical file missing.");

            var bytes = await File.ReadAllBytesAsync(fullPath);
            return ServiceResult<(byte[], string, string)>.Ok(
                (bytes, record.FileName, GetContentType(record.FileType)),
                "File downloaded"
            );
        }

        // ===================== DELETE =====================

        public async Task<ServiceResult<bool>> DeleteFileAsync(string filePath, string userId)
        {
            var record = await _unitOfWork.FileRecordRepository.GetByPathAsync(filePath, userId);
            if (record == null)
                return ServiceResult<bool>.Fail("File not found.");

            DeletePhysicalFile(filePath);

            await _unitOfWork.FileRecordRepository.DeleteAsync(record.Id.ToString(), userId);
            await _unitOfWork.SaveChangesAsync();

            return ServiceResult<bool>.Ok(true, "File deleted");
        }

        // ===================== QUERIES =====================

        public async Task<ServiceResult<IEnumerable<FileRecord>>> GetFilesByUserAsync(string userId)
        {
            var files = await _unitOfWork.FileRecordRepository.GetFilesByUserIdAsync(userId);
            return ServiceResult<IEnumerable<FileRecord>>.Ok(files);
        }

        public async Task<ServiceResult<IEnumerable<FileRecord>>> GetPDFFilesByUserAsync(
            string userId
        )
        {
            if (string.IsNullOrWhiteSpace(userId))
                return ServiceResult<IEnumerable<FileRecord>>.Fail("UserId is required");

            // Use the repository user-scoped method
            var files = await _unitOfWork.FileRecordRepository.GetByTypeAsync(".pdf", userId);

            return ServiceResult<IEnumerable<FileRecord>>.Ok(
                files,
                "PDF files retrieved successfully"
            );
        }

        public async Task<ServiceResult<long>> GetTotalStorageUsedAsync(string userId)
        {
            var files = await _unitOfWork.FileRecordRepository.GetFilesByUserIdAsync(userId);
            var total = files.Sum(f => f.FileSize);
            return ServiceResult<long>.Ok(total);
        }

        // ===================== HELPERS =====================

        private static string? ValidateExtension(string fileName)
        {
            var ext = Path.GetExtension(fileName).ToLowerInvariant();
            return _allowedExtensions.Contains(ext) ? ext : null;
        }

        private async Task<(string RelativePath, string FileName)> SavePhysicalFileAsync(
            IFormFile file,
            string controller,
            string extension
        )
        {
            var folder = GetFileTypeFolder(extension);
            var uploadPath = Path.Combine(_rootPath, controller, folder);
            Directory.CreateDirectory(uploadPath);

            var fileName = $"{Guid.NewGuid()}{extension}";
            var fullPath = Path.Combine(uploadPath, fileName);

            using var stream = new FileStream(fullPath, FileMode.Create);
            await file.CopyToAsync(stream);

            var relativePath = Path.Combine("uploads", controller, folder, fileName)
                .Replace("\\", "/");

            return (relativePath, fileName);
        }

        private static FileRecord CreateFileRecord(
            IFormFile file,
            string fileName,
            string relativePath,
            string controller,
            string extension,
            string userId
        )
        {
            return new FileRecord
            {
                FileName = fileName,
                RelativePath = relativePath,
                ControllerName = controller,
                FileType = extension,
                FileSize = file.Length,
                UploadedAt = DateTime.Now,
                UserId = userId,
            };
        }

        private string GetPhysicalPath(string relativePath)
        {
            return Path.Combine(_rootPath, relativePath.Replace("uploads/", "").Replace("/", "\\"));
        }

        private void DeletePhysicalFile(string relativePath)
        {
            var fullPath = GetPhysicalPath(relativePath);
            if (File.Exists(fullPath))
                File.Delete(fullPath);
        }

        private static string GetFileTypeFolder(string ext) =>
            ext switch
            {
                ".jpg" or ".jpeg" or ".png" or ".gif" => "images",
                ".pdf" => "pdf",
                ".doc" or ".docx" => "word",
                _ => "others",
            };

        private static string GetContentType(string ext) =>
            ext switch
            {
                ".jpg" or ".jpeg" => "image/jpeg",
                ".png" => "image/png",
                ".gif" => "image/gif",
                ".pdf" => "application/pdf",
                ".doc" => "application/msword",
                ".docx" =>
                    "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
                _ => "application/octet-stream",
            };
    }
}
