using Domain.Entities;
using Infrastructure.Interfaces;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class AdminRepository : GenericRepository<Admin>,IAdminRepository
    {
        private new readonly AlMaalyGateSchoolContext _context;
        private new readonly DbSet<Admin> _dbSet;

        public AdminRepository(AlMaalyGateSchoolContext context) : base(context)
        {
            _context = context;
            _dbSet = context.Set<Admin>();
        }
        public async Task<Admin?> GetByAppUserIdAsync(string appUserId)
        {
            return await _dbSet.FirstOrDefaultAsync(t => t.AppUserId == appUserId);
        }
    }
}

