using Domain.Entities;
using Infrastructure.Interfaces;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class TextAnswersRepository : GenericRepository<TextAnswers>, ITextAnswersRepository
    {
        private new readonly AlMaalyGateSchoolContext _context;
        private new readonly DbSet<TextAnswers> _dbSet;

        public TextAnswersRepository(AlMaalyGateSchoolContext context) : base(context)
        {
            _context = context;
            _dbSet = context.Set<TextAnswers>();
        }
    }
}
