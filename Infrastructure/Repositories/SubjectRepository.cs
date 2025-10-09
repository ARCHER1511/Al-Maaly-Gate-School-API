using Domain.Entities;
using Infrastructure.Interfaces;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class SubjectRepository : GenericRepository<Subject>,ISubjectRepository
    {
        private new readonly AlMaalyGateSchoolContext _context;
        private new readonly DbSet<Subject> _dbSet;

        public SubjectRepository(AlMaalyGateSchoolContext context) : base(context)
        {
            _context = context;
            _dbSet = context.Set<Subject>();
        }
    }
}
