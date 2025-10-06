using Domain.Entities;
using Infrastructure.Data;
using Domain.Interfaces.ApplicationInterfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using System.Linq.Expressions;

namespace Infrastructure.Repositories
{
    public class StudentRepository : GenericRepository<Student>,IStudentRepository
    {
        private new readonly AlMaalyGateSchoolContext _context;
        private new readonly DbSet<Student> _dbSet;

        public StudentRepository(AlMaalyGateSchoolContext context) : base(context)
        {
            _context = context;
            _dbSet = context.Set<Student>();
        }
    }
}
