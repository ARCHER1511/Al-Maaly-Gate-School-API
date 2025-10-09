using Domain.Entities;
using Infrastructure.Interfaces;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class TeacherSubjectExamRepository : GenericRepository<TeacherSubjectExam>,ITeacherSubjectExamRepository
    {
        private new readonly AlMaalyGateSchoolContext _context;
        private new readonly DbSet<TeacherSubjectExam> _dbSet;

        public TeacherSubjectExamRepository(AlMaalyGateSchoolContext context) : base(context)
        {
            _context = context;
            _dbSet = context.Set<TeacherSubjectExam>();
        }
    }
}
