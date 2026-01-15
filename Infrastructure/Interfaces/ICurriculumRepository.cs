using Domain.Entities;

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
