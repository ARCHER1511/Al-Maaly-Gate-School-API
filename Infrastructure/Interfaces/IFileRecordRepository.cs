using Domain.Entities;
using System.Linq.Expressions;

namespace Infrastructure.Interfaces
{
    public interface IFileRecordRepository
    {
        Task<FileRecord> AddAsync(FileRecord record);
        Task<IEnumerable<FileRecord>> AddRangeAsync(IEnumerable<FileRecord> entities);
        Task UpdateAsync(FileRecord record);
        Task DeleteAsync(FileRecord record);
        Task<FileRecord?> GetByIdAsync(string id);
        Task<FileRecord?> GetByPathAsync(string relativePath);
        Task<IEnumerable<FileRecord>> GetAllAsync();
        Task<IEnumerable<FileRecord>> FindAsync(Expression<Func<FileRecord, bool>> predicate);


        Task<IEnumerable<FileRecord>> GetByControllerAsync(string controllerName);
        Task<IEnumerable<FileRecord>> GetByTypeAsync(string fileType);
        Task<IEnumerable<FileRecord>> GetRecentFilesAsync(int count = 10);
        Task<bool> ExistsAsync(string relativePath);
        Task DeleteByControllerAsync(string controllerName);
    }
}
