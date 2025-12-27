using Domain.Entities;
using Infrastructure.Data;
using Infrastructure.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class FileRecordRepository : GenericRepository<FileRecord>, IFileRecordRepository
    {
        private new readonly DbSet<FileRecord> _dbSet;

        public FileRecordRepository(AlMaalyGateSchoolContext context) :base (context)
        {
            _dbSet = context.Set<FileRecord>();
        }

        public async new Task<FileRecord> AddAsync(FileRecord record)
        {
            await _dbSet.AddAsync(record);
            return record;
        }

        public async Task<IEnumerable<FileRecord>> AddRangeAsync(
            IEnumerable<FileRecord> records)
        {
            await _dbSet.AddRangeAsync(records);
            return records;
        }

        public async Task<FileRecord?> GetByIdAsync(
            string fileId,
            string userId)
        {
            return await _dbSet.FirstOrDefaultAsync(f =>
                f.Id.ToString() == fileId &&
                f.UserId == userId);
        }

        public async Task<FileRecord?> GetByPathAsync(
            string relativePath,
            string userId)
        {
            return await _dbSet.FirstOrDefaultAsync(f =>
                f.RelativePath == relativePath &&
                f.UserId == userId);
        }

        public async Task<IEnumerable<FileRecord>> GetFilesByUserIdAsync(
            string userId)
        {
            return await _dbSet
                .Where(f => f.UserId == userId)
                .OrderByDescending(f => f.UploadedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<FileRecord>> GetRecentFilesByUserAsync(
            string userId,
            int count = 10)
        {
            return await _dbSet
                .Where(f => f.UserId == userId)
                .OrderByDescending(f => f.UploadedAt)
                .Take(count)
                .ToListAsync();
        }

        public async Task<IEnumerable<FileRecord>> GetByControllerAsync(
            string controllerName,
            string userId)
        {
            return await _dbSet
                .Where(f =>
                    f.ControllerName == controllerName &&
                    f.UserId == userId)
                .OrderByDescending(f => f.UploadedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<FileRecord>> GetByTypeAsync(
            string fileType,
            string userId)
        {
            return await _dbSet
                .Where(f =>
                    f.FileType == fileType &&
                    f.UserId == userId)
                .OrderByDescending(f => f.UploadedAt)
                .ToListAsync();
        }

        public async Task<bool> ExistsAsync(
            string relativePath,
            string userId)
        {
            return await _dbSet.AnyAsync(f =>
                f.RelativePath == relativePath &&
                f.UserId == userId);
        }

        public Task UpdateAsync(FileRecord record)
        {
            _dbSet.Update(record);
            return Task.CompletedTask;
        }

        public async Task DeleteAsync(
            string fileId,
            string userId)
        {
            var record = await GetByIdAsync(fileId, userId);

            if (record != null)
                _dbSet.Remove(record);
        }

        public async Task DeleteByControllerAsync(
            string controllerName,
            string userId)
        {
            var records = await _dbSet
                .Where(f =>
                    f.ControllerName == controllerName &&
                    f.UserId == userId)
                .ToListAsync();

            if (records.Any())
                _dbSet.RemoveRange(records);
        }
    }
}
