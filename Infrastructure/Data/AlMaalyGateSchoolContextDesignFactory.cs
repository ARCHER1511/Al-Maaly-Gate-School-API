using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Infrastructure.Data
{
    public class AlMaalyGateSchoolContextDesignFactory : IDesignTimeDbContextFactory<AlMaalyGateSchoolContext>
    {
        public AlMaalyGateSchoolContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<AlMaalyGateSchoolContext>();
            optionsBuilder.UseSqlServer("Server=.;Database=AlMaalyGateSchoolDb;Trusted_Connection=True;Encrypt=False");

            return new AlMaalyGateSchoolContext(optionsBuilder.Options);
        }
    }
}
