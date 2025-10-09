using Domain.Entities;
using Infrastructure.Interfaces;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class TeacherRepository : GenericRepository<Teacher>,ITeacherRepository
    {
        private new readonly AlMaalyGateSchoolContext _context;
        private new readonly DbSet<Teacher> _dbSet;

        public TeacherRepository(AlMaalyGateSchoolContext context) : base(context)
        {
            _context = context;
            _dbSet = context.Set<Teacher>();
        }
    }
}
