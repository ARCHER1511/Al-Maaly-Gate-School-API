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
        private readonly string[] _allowedExtensions =
        {
            ".jpg", ".jpeg", ".png", ".gif", ".pdf", ".doc", ".docx"
        };
        private readonly IFileRecordRepository _recordRepo;
        private readonly IUnitOfWork _unitOfWork;

        public FileService(IWebHostEnvironment env, IFileRecordRepository recordRepo, IUnitOfWork unitOfWork)
        {
            _rootPath = Path.Combine(env.WebRootPath, "uploads");
            _recordRepo = recordRepo;
            _unitOfWork = unitOfWork;
        }

        public async Task<ServiceResult<string>> UploadFileAsync(IFormFile file, string controllerName)
        {
            if (file == null || file.Length == 0)
                return ServiceResult<string>.Fail("File cannot be empty.");

            var extension = Path.GetExtension(file!.FileName).ToLowerInvariant();
            if (!_allowedExtensions.Contains(extension))
                return ServiceResult<string>.Fail("File type not supported.");

            string typeFolder = GetFileTypeFolder(extension);
            string uploadPath = Path.Combine(_rootPath, controllerName, typeFolder);
            Directory.CreateDirectory(uploadPath);

            string uniqueFileName = $"{Guid.NewGuid()}{extension}";
            string fullPath = Path.Combine(uploadPath, uniqueFileName);

            using (var stream = new FileStream(fullPath, FileMode.Create))
                await file.CopyToAsync(stream);

            string relativePath = Path.Combine("uploads", controllerName, typeFolder, uniqueFileName)
                .Replace("\\", "/");

            // Create record
            var record = new FileRecord
            {
                FileName = uniqueFileName,
                RelativePath = relativePath,
                ControllerName = controllerName,
                FileType = extension,
                FileSize = file.Length,
                UploadedAt = DateTime.Now
            };

            await _recordRepo.AddAsync(record);
            await _unitOfWork.SaveChangesAsync(); // commit DB changes

            return ServiceResult<string>.Ok(relativePath, "Uploaded Successfully");
        }

        public async Task<ServiceResult<List<string>>> UploadFilesAsync(IEnumerable<IFormFile> files, string controllerName)
        {
            if (files == null || !files.Any())
                ServiceResult<List<string>>.Fail("No files provided.");

            var uploadedPaths = new List<string>();
            var records = new List<FileRecord>();

            foreach (var file in files!)
            {
                if (file == null || file.Length == 0)
                    continue;

                var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
                if (!_allowedExtensions.Contains(extension))
                    continue;

                string typeFolder = GetFileTypeFolder(extension);
                string uploadPath = Path.Combine(_rootPath, controllerName, typeFolder);
                Directory.CreateDirectory(uploadPath);

                string uniqueFileName = $"{Guid.NewGuid()}{extension}";
                string fullPath = Path.Combine(uploadPath, uniqueFileName);

                using (var stream = new FileStream(fullPath, FileMode.Create))
                    await file.CopyToAsync(stream);

                string relativePath = Path.Combine("uploads", controllerName, typeFolder, uniqueFileName)
                    .Replace("\\", "/");

                uploadedPaths.Add(relativePath);

                records.Add(new FileRecord
                {
                    FileName = uniqueFileName,
                    RelativePath = relativePath,
                    ControllerName = controllerName,
                    FileType = extension,
                    FileSize = file.Length,
                    UploadedAt = DateTime.Now
                });
            }

            if (records.Count > 0)
            {
                await _recordRepo.AddRangeAsync(records);
                await _unitOfWork.SaveChangesAsync();
            }

            return ServiceResult<List<string>>.Ok(uploadedPaths,"Files are uploaded Successfully");
        }

        public async Task<ServiceResult<string>> ReplaceFileAsync(IFormFile newFile, string existingFilePath, string controllerName)
        {
            var existingRecord = await _recordRepo.GetByPathAsync(existingFilePath);
            if (existingRecord == null)
                return ServiceResult<string>.Fail("File record not found.");

            // Delete old physical file
            string oldFullPath = Path.Combine(_rootPath, existingFilePath.Replace("uploads/", "").Replace("/", "\\"));
            if (File.Exists(oldFullPath))
                File.Delete(oldFullPath);

            // Upload new one
            var extension = Path.GetExtension(newFile.FileName).ToLowerInvariant();
            string typeFolder = GetFileTypeFolder(extension);
            string uploadPath = Path.Combine(_rootPath, controllerName, typeFolder);
            Directory.CreateDirectory(uploadPath);

            string uniqueFileName = $"{Guid.NewGuid()}{extension}";
            string newFullPath = Path.Combine(uploadPath, uniqueFileName);

            using (var stream = new FileStream(newFullPath, FileMode.Create))
                await newFile.CopyToAsync(stream);

            string newRelativePath = Path.Combine("uploads", controllerName, typeFolder, uniqueFileName)
                .Replace("\\", "/");

            // Update record
            existingRecord.FileName = uniqueFileName;
            existingRecord.RelativePath = newRelativePath;
            existingRecord.FileType = extension;
            existingRecord.FileSize = newFile.Length;
            existingRecord.UploadedAt = DateTime.Now;

            await _recordRepo.UpdateAsync(existingRecord);
            await _unitOfWork.SaveChangesAsync(); // commit DB changes

            return ServiceResult<string>.Ok(newRelativePath,"The file Replaced Successfully");
        }

        public async Task<ServiceResult<bool>> DeleteFileAsync(string filePath)
        {
            if (string.IsNullOrEmpty(filePath))
                return ServiceResult<bool>.Fail("No File Path Provided");

            string fullPath = Path.Combine(_rootPath, filePath.Replace("uploads/", "").Replace("/", "\\"));

            // Remove record from database
            var record = await _recordRepo.GetByPathAsync(filePath);
            if (record != null)
            {
                await _recordRepo.DeleteAsync(record);
                await _unitOfWork.SaveChangesAsync();
            }

            if (File.Exists(fullPath))
            {
                File.Delete(fullPath);
                return ServiceResult<bool>.Ok(true,"File Deleted Successfully");
            }

            return ServiceResult<bool>.Ok(false, "File not Deleted");
        }

        public async Task<ServiceResult<FileRecord?>> GetFileByIdAsync(string id)
        {
            var file = await _recordRepo.GetByIdAsync(id);
            return ServiceResult<FileRecord?>.Ok(file, "File Retrieved Successfully");
        }
        public async Task<ServiceResult<FileRecord?>> GetFileByPathAsync(string relativePath)
        {
            var file = await _recordRepo.GetByPathAsync(relativePath);
            return ServiceResult<FileRecord?>.Ok(file, "File Retrieved Successfully");
        }
        public async Task<ServiceResult<IEnumerable<FileRecord>>> GetAllFilesAsync()
        {
            var files = await _recordRepo.GetAllAsync();
            return ServiceResult<IEnumerable<FileRecord>>.Ok(files, "Files Retrieved Successfully");
        }

        public async Task<ServiceResult<IEnumerable<FileRecord>>> GetFilesByControllerAsync(string controllerName)
        {
            var files = await _recordRepo.FindAsync(f => f.ControllerName == controllerName);
            return ServiceResult<IEnumerable<FileRecord>>.Ok(files, "Files Retrieved Successfully");
        }

        public async Task<ServiceResult<IEnumerable<FileRecord>>> GetFilesByTypeAsync(string fileType)
        {
            fileType = fileType.ToLowerInvariant();
            var files = await _recordRepo.FindAsync(f => f.FileType == fileType);
            return ServiceResult<IEnumerable<FileRecord>>.Ok(files, "Files Retrieved Successfully");
        }

        public async Task<ServiceResult<IEnumerable<FileRecord>>> GetRecentFilesAsync(int days = 7)
        {
            var since = DateTime.Now.AddDays(-days);
            var recentFiles = await _recordRepo.FindAsync(f => f.UploadedAt >= since);
            return ServiceResult<IEnumerable<FileRecord>>.Ok(recentFiles, "Files Retrieved Successfully");
        }

        public async Task<ServiceResult<long>> GetTotalStorageUsedAsync()
        {
            var all = await _recordRepo.GetAllAsync();
            var totalStorageUsed = all.Sum(f => f.FileSize);
            return ServiceResult<long>.Ok(totalStorageUsed,$"Total Storage used is ${totalStorageUsed}");
        }

        public async Task<ServiceResult<int>> DeleteFilesByControllerAsync(string controllerName)
        {
            var files = await _recordRepo.FindAsync(f => f.ControllerName == controllerName);
            int deletedCount = 0;

            foreach (var record in files)
            {
                string fullPath = Path.Combine(_rootPath, record.RelativePath.Replace("uploads/", "").Replace("/", "\\"));
                if (File.Exists(fullPath))
                    File.Delete(fullPath);

                await _recordRepo.DeleteAsync(record);
                deletedCount++;
            }

            await _unitOfWork.SaveChangesAsync();
            return ServiceResult<int>.Ok(deletedCount,$"{deletedCount} files deleted successfully");
        }

        public async Task<ServiceResult<(byte[] FileBytes, string FileName, string ContentType)>> DownloadFileAsync(string filePath)
        {
            if (string.IsNullOrWhiteSpace(filePath))
                return ServiceResult<(byte[], string, string)>.Fail("File path not provided.");

            // Retrieve file record for validation
            var record = await _recordRepo.GetByPathAsync(filePath);
            if (record == null)
                return ServiceResult<(byte[], string, string)>.Fail("File record not found in database.");

            // Get physical path
            string fullPath = Path.Combine(_rootPath, filePath.Replace("uploads/", "").Replace("/", "\\"));
            if (!File.Exists(fullPath))
                return ServiceResult<(byte[], string, string)>.Fail("Physical file not found.");

            // Read file into memory
            byte[] fileBytes = await File.ReadAllBytesAsync(fullPath);

            // Determine MIME type
            string contentType = GetContentType(record.FileType);

            return ServiceResult<(byte[], string, string)>.Ok((fileBytes, record.FileName, contentType), "File downloaded successfully.");
        }

        private string GetContentType(string extension)
        {
            return extension.ToLower() switch
            {
                ".jpg" or ".jpeg" => "image/jpeg",
                ".png" => "image/png",
                ".gif" => "image/gif",
                ".pdf" => "application/pdf",
                ".doc" => "application/msword",
                ".docx" => "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
                _ => "application/octet-stream"
            };
        }

        private string GetFileTypeFolder(string extension)
        {
            if (new[] { ".jpg", ".jpeg", ".png", ".gif" }.Contains(extension))
                return "images";
            if (new[] { ".pdf" }.Contains(extension))
                return "pdf";
            if (new[] { ".doc", ".docx" }.Contains(extension))
                return "word";
            return "others";
        }


    }
}
