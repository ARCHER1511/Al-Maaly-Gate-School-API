using Domain.Entities;
using Domain.Interfaces.ApplicationInterfaces;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class StudentSubjectExamRepository : GenericRepository<StudentSubjectExam>,IStudentSubjectExamRepository
    {
        private new readonly AlMaalyGateSchoolContext _context;
        private new readonly DbSet<StudentSubjectExam> _dbSet;

        public StudentSubjectExamRepository(AlMaalyGateSchoolContext context) : base(context)
        {
            _context = context;
            _dbSet = context.Set<StudentSubjectExam>();
        }
    }
}
