using Domain.Interfaces.ApplicationInterfaces;
using Infrastructure.Data;

namespace Infrastructure.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AlMaalyGateSchoolContext _context;

        public UnitOfWork(AlMaalyGateSchoolContext context)
        {
            _context = context;
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }
    }
}
