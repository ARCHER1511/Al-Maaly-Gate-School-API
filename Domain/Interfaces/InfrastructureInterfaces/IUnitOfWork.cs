namespace Domain.Interfaces.ApplicationInterfaces
{
    public interface IUnitOfWork
    {
        Task<int> SaveChangesAsync();
    }
}
