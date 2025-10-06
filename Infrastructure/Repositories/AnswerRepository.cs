using Domain.Entities;
using Domain.Interfaces.ApplicationInterfaces;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class AnswerRepository : GenericRepository<Answer>,IAnswerRepository
    {
        private new readonly AlMaalyGateSchoolContext _context;
        private new readonly DbSet<Answer> _dbSet;

        public AnswerRepository(AlMaalyGateSchoolContext context) : base(context)
        {
            _context = context;
            _dbSet = context.Set<Answer>();
        }
    }
}
