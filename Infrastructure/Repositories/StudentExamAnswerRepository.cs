using Domain.Entities;
using Infrastructure.Data;
using Infrastructure.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class StudentExamAnswerRepository : GenericRepository<StudentExamAnswer>, IStudentExamAnswerRepository
    {
        private new readonly AlMaalyGateSchoolContext _context;
        private new readonly DbSet<StudentExamAnswer> _dbSet;

        public StudentExamAnswerRepository(AlMaalyGateSchoolContext context) : base(context)
        {
            _context = context;
            _dbSet = context.Set<StudentExamAnswer>();
        }

    }
}
