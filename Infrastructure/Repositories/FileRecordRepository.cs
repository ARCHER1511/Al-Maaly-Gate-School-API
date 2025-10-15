using Domain.Entities;
using Infrastructure.Data;
using Infrastructure.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Infrastructure.Repositories
{
    public class FileRecordRepository : IFileRecordRepository
    {
        private readonly AlMaalyGateSchoolContext _context;
        private readonly DbSet<FileRecord> _dbSet;

        public FileRecordRepository(AlMaalyGateSchoolContext context)
        {
            _context = context;
            _dbSet = context.Set<FileRecord>();
        }

        public async Task<FileRecord> AddAsync(FileRecord record)
        {
            await _dbSet.AddAsync(record);
            return record;
        }

        public async Task<IEnumerable<FileRecord>> AddRangeAsync(IEnumerable<FileRecord> entities)
        {
            await _dbSet.AddRangeAsync(entities);
            return entities;
        }

        public Task UpdateAsync(FileRecord record)
        {
            _dbSet.Update(record);
            return Task.CompletedTask;
        }

        public Task DeleteAsync(FileRecord record)
        {
            _dbSet.Remove(record);
            return Task.CompletedTask;
        }

        public async Task<FileRecord?> GetByIdAsync(string id)
        {
            return await _dbSet.FindAsync(id);
        }

        public async Task<FileRecord?> GetByPathAsync(string relativePath)
        {
            return await _dbSet.FirstOrDefaultAsync(f => f.RelativePath == relativePath);
        }

        public async Task<IEnumerable<FileRecord>> GetAllAsync()
        {
            return await _dbSet.ToListAsync();
        }

        public async Task<IEnumerable<FileRecord>> FindAsync(Expression<Func<FileRecord, bool>> predicate)
        {
            return await _dbSet.Where(predicate).ToListAsync();
        }

        public async Task<IEnumerable<FileRecord>> GetByControllerAsync(string controllerName)
        {
            return await _dbSet.Where(f => f.ControllerName == controllerName).ToListAsync();
        }

        public async Task<IEnumerable<FileRecord>> GetByTypeAsync(string fileType)
        {
            return await _dbSet.Where(f => f.FileType == fileType).ToListAsync();
        }

        public async Task<bool> ExistsAsync(string relativePath)
        {
            return await _dbSet.AnyAsync(f => f.RelativePath == relativePath);
        }

        public async Task<IEnumerable<FileRecord>> GetRecentFilesAsync(int count = 10)
        {
            return await _dbSet.OrderByDescending(f => f.UploadedAt).Take(count).ToListAsync();
        }

        public async Task DeleteByControllerAsync(string controllerName)
        {
            var records = await _dbSet.Where(f => f.ControllerName == controllerName).ToListAsync();

            if (records.Any())
                _dbSet.RemoveRange(records);
        }
    }
}
