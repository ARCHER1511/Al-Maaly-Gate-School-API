using Domain.Entities;
using Infrastructure.Data;
using Infrastructure.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using System.Linq.Expressions;

namespace Infrastructure.Repositories
{
    public class QuestionExamTeacherRepository : IQuestionExamTeacherRepository
    {
        private readonly AlMaalyGateSchoolContext _context;
        private readonly DbSet<QuestionExamTeacher> _dbSet;

        public QuestionExamTeacherRepository(AlMaalyGateSchoolContext context)
        {
            _context = context;
            _dbSet = context.Set<QuestionExamTeacher>();
        }

        // 🔹 Ordinary methods
        public async Task<QuestionExamTeacher?> GetByIdAsync(object id)
        {
            return await _dbSet.FindAsync(id);
        }

        public async Task<IEnumerable<QuestionExamTeacher>> GetAllAsync()
        {
            return await _dbSet.ToListAsync();
        }

        public async Task AddAsync(QuestionExamTeacher entity)
        {
            await _dbSet.AddAsync(entity);
        }

        public void Update(QuestionExamTeacher entity)
        {
            _dbSet.Update(entity);
        }

        public void Delete(QuestionExamTeacher entity)
        {
            _dbSet.Remove(entity);
        }

        // 🔹 LINQ-powered methods
        public async Task<QuestionExamTeacher?> GetAsync(Expression<Func<QuestionExamTeacher, bool>> predicate,
                                             Func<IQueryable<QuestionExamTeacher>, IIncludableQueryable<QuestionExamTeacher, object>>? include = null)
        {
            IQueryable<QuestionExamTeacher> query = _dbSet;
            if (include != null) query = include(query);
            return await query.FirstOrDefaultAsync(predicate);
        }

        public async Task<IEnumerable<QuestionExamTeacher>> GetAllAsync(Expression<Func<QuestionExamTeacher, bool>>? predicate = null,
                                                          Func<IQueryable<QuestionExamTeacher>, IIncludableQueryable<QuestionExamTeacher, object>>? include = null,
                                                          Func<IQueryable<QuestionExamTeacher>, IOrderedQueryable<QuestionExamTeacher>>? orderBy = null,
                                                          int? skip = null,
                                                          int? take = null)
        {
            IQueryable<QuestionExamTeacher> query = _dbSet;

            if (predicate != null) query = query.Where(predicate);
            if (include != null) query = include(query);
            if (orderBy != null) query = orderBy(query);
            if (skip.HasValue) query = query.Skip(skip.Value);
            if (take.HasValue) query = query.Take(take.Value);

            return await query.ToListAsync();
        }

        public IQueryable<QuestionExamTeacher> Query(Expression<Func<QuestionExamTeacher, bool>>? predicate = null)
        {
            return predicate != null ? _dbSet.Where(predicate) : _dbSet;
        }
    }
}
