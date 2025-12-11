using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Interfaces
{
    public interface ICurriculumRepository : IGenericRepository<Curriculum>
    {
        Task<Curriculum?> GetWithDetailsAsync(string id);
        Task<IEnumerable<Curriculum>> GetAllWithGradesAsync();
        Task<Curriculum?> GetByNameAsync(string name);
        Task<bool> ExistsByNameAsync(string name);
    }
}
