using Domain.Entities;
using Infrastructure.Interfaces;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class ClassAssetsRepository : GenericRepository<ClassAssets>,IClassAssetsRepository
    {
        private new readonly AlMaalyGateSchoolContext _context;
        private new readonly DbSet<ClassAssets> _dbSet;

        public ClassAssetsRepository(AlMaalyGateSchoolContext context) : base(context)
        {
            _context = context;
            _dbSet = context.Set<ClassAssets>();
        }
    }
}
