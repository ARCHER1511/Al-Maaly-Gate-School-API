using Domain.Entities;
using Domain.Interfaces.ApplicationInterfaces;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class ClassSubjectRepository : GenericRepository<ClassSubject>,IClassSubjectRepository
    {
        private new readonly AlMaalyGateSchoolContext _context;
        private new readonly DbSet<ClassSubject> _dbSet;

        public ClassSubjectRepository(AlMaalyGateSchoolContext context) : base(context)
        {
            _context = context;
            _dbSet = context.Set<ClassSubject>();
        }
    }
}
