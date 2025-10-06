using Domain.Entities;
using Infrastructure.Data;
using Domain.Interfaces.ApplicationInterfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using System.Linq.Expressions;

namespace Infrastructure.Repositories
{
    public class ClassRepository : GenericRepository<Class>,IClassRepository
    {
        private new readonly AlMaalyGateSchoolContext _context;
        private new readonly DbSet<Class> _dbSet;

        public ClassRepository(AlMaalyGateSchoolContext context) : base(context)
        {
            _context = context;
            _dbSet = context.Set<Class>();
        }
    }
}
