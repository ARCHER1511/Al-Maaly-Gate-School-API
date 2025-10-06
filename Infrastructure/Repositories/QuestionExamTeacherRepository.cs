using Domain.Entities;
using Infrastructure.Data;
using Domain.Interfaces.ApplicationInterfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using System.Linq.Expressions;

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
