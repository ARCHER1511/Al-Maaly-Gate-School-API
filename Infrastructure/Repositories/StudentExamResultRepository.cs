using Domain.Entities;
using Infrastructure.Data;
using Infrastructure.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class StudentExamResultRepository : GenericRepository<StudentExamResult>, IStudentExamResultRepository
    {
        private new readonly AlMaalyGateSchoolContext _context;
        private new readonly DbSet<StudentExamResult> _dbSet;

        public StudentExamResultRepository(AlMaalyGateSchoolContext context) : base(context)
        {
            _context = context;
            _dbSet = context.Set<StudentExamResult>();

        }
    }
}