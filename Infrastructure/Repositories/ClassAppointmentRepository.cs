using Domain.Entities;
using Domain.Interfaces.ApplicationInterfaces;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class ClassAppointmentRepository : GenericRepository<ClassAppointment>, IClassAppointmentRepository
    {
        private new readonly AlMaalyGateSchoolContext _context;
        private new readonly DbSet<ClassAppointment> _dbSet;

        public ClassAppointmentRepository(AlMaalyGateSchoolContext context) : base(context)
        {
            _context = context;
            _dbSet = context.Set<ClassAppointment>();
        }
    }
}
