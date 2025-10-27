using Domain.Entities;
using Infrastructure.Data;
using Infrastructure.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class TrueAndFalsesRepository : GenericRepository<TrueAndFalses>, ITrueAndFalsesRepository
    {
        private new readonly AlMaalyGateSchoolContext _context;
        private new readonly DbSet<TrueAndFalses> _dbSet;

        public TrueAndFalsesRepository(AlMaalyGateSchoolContext context) : base(context)
        {
            _context = context;
            _dbSet = context.Set<TrueAndFalses>();
        }
    }
}
