using Domain.Entities;
using Domain.Interfaces.ApplicationInterfaces;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class ParentStudentRepository : GenericRepository<ParentStudent>, IParentStudentRepository
    {
        private new readonly AlMaalyGateSchoolContext _context;
        private new readonly DbSet<ParentStudent> _dbSet;

        public ParentStudentRepository(AlMaalyGateSchoolContext context) : base(context)
        {
            _context = context;
            _dbSet = context.Set<ParentStudent>();
        }
    }
}
