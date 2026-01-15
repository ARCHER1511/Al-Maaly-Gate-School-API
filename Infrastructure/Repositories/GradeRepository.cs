using Domain.Entities;
using Infrastructure.Data;
using Infrastructure.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class GradeRepository : GenericRepository<Grade>, IGradeRepository
    {
        private new readonly AlMaalyGateSchoolContext _context;
        private new readonly DbSet<Grade> _dbSet;
        private readonly DbSet<Class> _classDbSet;
        private readonly DbSet<Subject> _subjectDbSet;

        public GradeRepository(AlMaalyGateSchoolContext context) : base(context)
        {
            _context = context;
            _dbSet = context.Set<Grade>();
            _classDbSet = context.Set<Class>();
            _subjectDbSet = context.Set<Subject>();
        }

        public async Task<Grade?> GetByIdWithDetailsAsync(string id)
        {
            return await _dbSet
                .Include(g => g.Classes)
                .Include(g => g.Subjects)
                .FirstOrDefaultAsync(g => g.Id == id);
        }

        public async Task<IEnumerable<Grade>> GetAllWithDetailsAsync()
        {
            return await _dbSet
                .Include(g => g.Classes)
                .Include(g => g.Subjects)
                .ToListAsync();
        }

        public async Task<Grade?> GetByNameAsync(string gradeName)
        {
            return await _dbSet
                .Include(g => g.Classes)
                .Include(g => g.Subjects)
                .FirstOrDefaultAsync(g => g.GradeName == gradeName);
        }

        public async Task<bool> AddClassToGradeAsync(string gradeId, Class classEntity)
        {
            var grade = await _dbSet.FindAsync(gradeId);
            if (grade == null) return false;

            classEntity.GradeId = gradeId;
            await _classDbSet.AddAsync(classEntity);
            return true;
        }

        public async Task<bool> AddSubjectToGradeAsync(string gradeId, Subject subject)
        {
            var grade = await _dbSet.FindAsync(gradeId);
            if (grade == null) return false;

            subject.GradeId = gradeId;
            await _subjectDbSet.AddAsync(subject);
            return true;
        }

        public async Task<bool> RemoveClassFromGradeAsync(string classId)
        {
            var classEntity = await _classDbSet.FindAsync(classId);
            if (classEntity == null) return false;

            _classDbSet.Remove(classEntity);
            return true;
        }

        public async Task<bool> RemoveSubjectFromGradeAsync(string subjectId)
        {
            var subject = await _subjectDbSet.FindAsync(subjectId);
            if (subject == null) return false;

            _subjectDbSet.Remove(subject);
            return true;
        }

        public async Task<bool> MoveClassToAnotherGradeAsync(string classId, string newGradeId)
        {
            var classEntity = await _classDbSet.FindAsync(classId);
            var newGrade = await _dbSet.FindAsync(newGradeId);

            if (classEntity == null || newGrade == null) return false;

            classEntity.GradeId = newGradeId;
            _classDbSet.Update(classEntity);
            return true;
        }

        public async Task<bool> MoveSubjectToAnotherGradeAsync(string subjectId, string newGradeId)
        {
            var subject = await _subjectDbSet.FindAsync(subjectId);
            var newGrade = await _dbSet.FindAsync(newGradeId);

            if (subject == null || newGrade == null) return false;

            subject.GradeId = newGradeId;
            _subjectDbSet.Update(subject);
            return true;
        }

        public async Task<IEnumerable<Class>> GetClassesByGradeIdAsync(string gradeId)
        {
            return await _classDbSet
                .Where(c => c.GradeId == gradeId)
                .Include(c => c.TeacherClasses)
                .Include(c => c.Students)
                .ToListAsync();
        }

        public async Task<IEnumerable<Subject>> GetSubjectsByGradeIdAsync(string gradeId)
        {
            return await _subjectDbSet
                .Where(s => s.GradeId == gradeId)
                .Include(s => s.TeacherSubjects)
                .Include(s => s.Exams)
                .ToListAsync();
        }

        public async Task<Grade?> GetByNameAndCurriculumAsync(string gradeName, string curriculumId)
        {
            return await _context.Grades
                .FirstOrDefaultAsync(g => g.GradeName.ToLower() == gradeName.ToLower()!
                                       && g.CurriculumId == curriculumId);
        }

        public async Task<Grade?> GetByIdWithCurriculumAsync(string id)
        {
            return await _context.Grades
                .Include(g => g.Curriculum)
                .FirstOrDefaultAsync(g => g.Id == id);
        }

        public async Task<Grade?> GetByNameWithCurriculumAsync(string name)
        {
            return await _context.Grades
                .Include(g => g.Curriculum)
                .FirstOrDefaultAsync(g => g.GradeName.ToLower() == name.ToLower()!);
        }

        public async Task<IEnumerable<Grade>> GetGradesByCurriculumAsync(string curriculumId)
        {
            return await _context.Grades
                .Include(g => g.Curriculum)
                .Where(g => g.CurriculumId == curriculumId)
                .ToListAsync();
        }


        // Override GetAllAsync to include Curriculum
        public override async Task<IEnumerable<Grade>> GetAllAsync()
        {
            return await _context.Grades
                .Include(g => g.Curriculum)
                .ToListAsync();
        }

        // Override GetByIdAsync to include Curriculum
        public override async Task<Grade?> GetByIdAsync(object id)
        {
            return await _context.Grades
                .Include(g => g.Curriculum)
                .FirstOrDefaultAsync(g => g.Id == (string)id);
        }
    }
}
