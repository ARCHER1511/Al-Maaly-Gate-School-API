using Domain.Entities;
using Infrastructure.Data;
using Infrastructure.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class StudentRepository : GenericRepository<Student>,IStudentRepository
    {
        private new readonly AlMaalyGateSchoolContext _context;
        private new readonly DbSet<Student> _dbSet;

        public StudentRepository(AlMaalyGateSchoolContext context) : base(context)
        {
            _context = context;
            _dbSet = context.Set<Student>();
        }
    }
}
