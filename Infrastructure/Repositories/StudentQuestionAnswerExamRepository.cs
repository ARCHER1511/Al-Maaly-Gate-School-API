using Domain.Entities;
using Infrastructure.Interfaces;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class StudentQuestionAnswerExamRepository : GenericRepository<StudentQuestionAnswerExam>,IStudentQuestionAnswerExamRepository
    {
        private new readonly AlMaalyGateSchoolContext _context;
        private new readonly DbSet<StudentQuestionAnswerExam> _dbSet;

        public StudentQuestionAnswerExamRepository(AlMaalyGateSchoolContext context) : base(context)
        {
            _context = context;
            _dbSet = context.Set<StudentQuestionAnswerExam>();
        }
    }
}
