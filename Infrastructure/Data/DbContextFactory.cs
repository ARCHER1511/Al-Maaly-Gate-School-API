using Microsoft.EntityFrameworkCore;
using System;

namespace Infrastructure.Data
{
    public class DbContextFactory : IDbContextFactory<AlMaalyGateSchoolContext>
    {
        private readonly DbContextOptions<AlMaalyGateSchoolContext> _options;

        public DbContextFactory(DbContextOptions<AlMaalyGateSchoolContext> options)
        {
            _options = options;
        }

        public AlMaalyGateSchoolContext CreateDbContext()
        {
            return new AlMaalyGateSchoolContext(_options);
        }
    }
}
