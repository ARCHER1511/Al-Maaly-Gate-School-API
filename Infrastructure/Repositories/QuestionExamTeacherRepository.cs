using Domain.Entities;
using Infrastructure.Data;
using Infrastructure.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class QuestionExamTeacherRepository : GenericRepository<QuestionExamTeacher>, IQuestionExamTeacherRepository
    {
        private new readonly AlMaalyGateSchoolContext _context;
        private new readonly DbSet<QuestionExamTeacher> _dbSet;

        public QuestionExamTeacherRepository(AlMaalyGateSchoolContext context) : base(context)
        {
            _context = context;
            _dbSet = context.Set<QuestionExamTeacher>();
        }
    }
}
