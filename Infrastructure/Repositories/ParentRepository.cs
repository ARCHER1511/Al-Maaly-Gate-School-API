using Domain.Entities;
using Domain.Interfaces.ApplicationInterfaces;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class ParentRepository : GenericRepository<Parent>,IParentRepository
    {
        private new readonly AlMaalyGateSchoolContext _context;
        private new readonly DbSet<Parent> _dbSet;

        public ParentRepository(AlMaalyGateSchoolContext context) : base(context)
        {
            _context = context;
            _dbSet = context.Set<Parent>();
        }
    }
}
