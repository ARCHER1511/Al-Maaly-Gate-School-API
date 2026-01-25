using Domain.Entities;
using System.Linq.Expressions;

namespace Infrastructure.Interfaces
{
    public interface IFileRecordRepository:IGenericRepository<FileRecord>
    {
        // CREATE
        new Task<FileRecord> AddAsync(FileRecord record);
        Task<IEnumerable<FileRecord>> AddRangeAsync(IEnumerable<FileRecord> records);

        // READ (user-scoped)
        Task<FileRecord?> GetByIdAsync(string fileId, string userId);
        Task<FileRecord?> GetByPathAsync(string relativePath, string userId);

        Task<IEnumerable<FileRecord>> GetFilesByUserIdAsync(string userId);
        Task<IEnumerable<FileRecord>> GetRecentFilesByUserAsync(string userId, int count = 10);

        Task<IEnumerable<FileRecord>> GetByControllerAsync(string controllerName, string userId);
        Task<IEnumerable<FileRecord>> GetByTypeAsync(string fileType, string userId);

        Task<bool> ExistsAsync(string relativePath, string userId);

        // UPDATE
        new Task UpdateAsync(FileRecord record);

        // DELETE (user-scoped)
        Task DeleteAsync(string fileId, string userId);
        Task DeleteByControllerAsync(string controllerName, string userId);
    }
}
