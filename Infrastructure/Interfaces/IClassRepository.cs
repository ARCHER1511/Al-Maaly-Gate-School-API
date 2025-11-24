using Domain.Entities;

namespace Infrastructure.Interfaces
{
    public interface IClassRepository : IGenericRepository<Class>
    {
        Task<IEnumerable<Class>> GetAllWithTeachersAsync();
    }
}
