using Domain.Entities;
using Domain.Interfaces.ApplicationInterfaces;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class AppUserRoleRepository : GenericRepository<AppUserRole>, IAppUserRoleRepository
    {
        private new readonly AlMaalyGateSchoolContext _context;
        private new readonly DbSet<AppUserRole> _dbSet;

        public AppUserRoleRepository(AlMaalyGateSchoolContext context) : base(context)
        {
            _context = context;
            _dbSet = context.Set<AppUserRole>();
        }
    }
}
