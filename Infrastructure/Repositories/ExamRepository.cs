using Domain.Entities;
using Domain.Interfaces.ApplicationInterfaces;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class ExamRepository : GenericRepository<Exam>,IExamRepository
    {
        private new readonly AlMaalyGateSchoolContext _context;
        private new readonly DbSet<Exam> _dbSet;

        public ExamRepository(AlMaalyGateSchoolContext context) : base(context)
        {
            _context = context;
            _dbSet = context.Set<Exam>();
        }
    }
}
