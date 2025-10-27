using Domain.Entities;
using Infrastructure.Data;
using Infrastructure.Interfaces;
using Microsoft.EntityFrameworkCore;


namespace Infrastructure.Repositories
{
    public class ChoicesRepository : GenericRepository<Choices>, IChoicesRepository
    {
        private new readonly AlMaalyGateSchoolContext _context;
        private new readonly DbSet<Choices> _dbSet;

        public ChoicesRepository(AlMaalyGateSchoolContext context) : base(context)
        {
            _context = context;
            _dbSet = context.Set<Choices>();
        }
    }
}
