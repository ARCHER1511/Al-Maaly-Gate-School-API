using Domain.Entities;
using Infrastructure.Interfaces;
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

        public async Task<IEnumerable<ClassAppointment>> GetByTeacherIdAsync(string teacherId)
        {
            return await _dbSet
                .Where(c => c.TeacherId == teacherId)
                .Include(c => c.Subject)
                .Include(c => c.Class)
                .ToListAsync();
        }
    }
}
