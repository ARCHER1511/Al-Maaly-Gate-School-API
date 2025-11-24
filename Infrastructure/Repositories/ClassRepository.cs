using Domain.Entities;
using Infrastructure.Data;
using Infrastructure.Interfaces;
using Microsoft.EntityFrameworkCore;

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
        public async Task<IEnumerable<Class>> GetAllWithTeachersAsync() 
        {
            return await _dbSet
            .Include(c => c.TeacherClasses)
                .ThenInclude(tc => tc.Teacher)
            .ToListAsync();

        public async Task<List<Student>> GetStudentsByClassIdAsync(string classId)
        {
            return await _context.Students
                .Where(s => s.ClassId == classId)
                .ToListAsync();
        }

        public async Task<List<Subject>> GetSubjectsByClassIdAsync(string classId)
        {
            return await _context.Subjects
                .Where(s => s.ClassId == classId)
                .ToListAsync();
        }
    }
}
