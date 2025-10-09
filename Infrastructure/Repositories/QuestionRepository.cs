using Domain.Entities;
using Infrastructure.Interfaces;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class QuestionRepository : GenericRepository<Question>, IQuestionRepository
    {
        private new readonly AlMaalyGateSchoolContext _context;
        private new readonly DbSet<Question> _dbSet;

        public QuestionRepository(AlMaalyGateSchoolContext context) : base(context)
        {
            _context = context;
            _dbSet = context.Set<Question>();
        }
    }
}
