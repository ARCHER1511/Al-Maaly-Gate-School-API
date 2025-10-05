using Domain.Entities;
using Infrastructure.Data;
using Infrastructure.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using System.Linq.Expressions;

namespace Infrastructure.Repositories
{
    public class StudentQuestionAnswerExamRepository : IStudentQuestionAnswerExamRepository
    {
        private readonly AlMaalyGateSchoolContext _context;
        private readonly DbSet<StudentQuestionAnswerExam> _dbSet;

        public StudentQuestionAnswerExamRepository(AlMaalyGateSchoolContext context)
        {
            _context = context;
            _dbSet = context.Set<StudentQuestionAnswerExam>();
        }

        // 🔹 Ordinary methods
        public async Task<StudentQuestionAnswerExam?> GetByIdAsync(object id)
        {
            return await _dbSet.FindAsync(id);
        }

        public async Task<IEnumerable<StudentQuestionAnswerExam>> GetAllAsync()
        {
            return await _dbSet.ToListAsync();
        }

        public async Task AddAsync(StudentQuestionAnswerExam entity)
        {
            await _dbSet.AddAsync(entity);
        }

        public void Update(StudentQuestionAnswerExam entity)
        {
            _dbSet.Update(entity);
        }

        public void Delete(StudentQuestionAnswerExam entity)
        {
            _dbSet.Remove(entity);
        }

        // 🔹 LINQ-powered methods
        public async Task<StudentQuestionAnswerExam?> GetAsync(Expression<Func<StudentQuestionAnswerExam, bool>> predicate,
                                             Func<IQueryable<StudentQuestionAnswerExam>, IIncludableQueryable<StudentQuestionAnswerExam, object>>? include = null)
        {
            IQueryable<StudentQuestionAnswerExam> query = _dbSet;
            if (include != null) query = include(query);
            return await query.FirstOrDefaultAsync(predicate);
        }

        public async Task<IEnumerable<StudentQuestionAnswerExam>> GetAllAsync(Expression<Func<StudentQuestionAnswerExam, bool>>? predicate = null,
                                                          Func<IQueryable<StudentQuestionAnswerExam>, IIncludableQueryable<StudentQuestionAnswerExam, object>>? include = null,
                                                          Func<IQueryable<StudentQuestionAnswerExam>, IOrderedQueryable<StudentQuestionAnswerExam>>? orderBy = null,
                                                          int? skip = null,
                                                          int? take = null)
        {
            IQueryable<StudentQuestionAnswerExam> query = _dbSet;

            if (predicate != null) query = query.Where(predicate);
            if (include != null) query = include(query);
            if (orderBy != null) query = orderBy(query);
            if (skip.HasValue) query = query.Skip(skip.Value);
            if (take.HasValue) query = query.Take(take.Value);

            return await query.ToListAsync();
        }

        public IQueryable<StudentQuestionAnswerExam> Query(Expression<Func<StudentQuestionAnswerExam, bool>>? predicate = null)
        {
            return predicate != null ? _dbSet.Where(predicate) : _dbSet;
        }
    }
}
