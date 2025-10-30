using Domain.Entities;

namespace Infrastructure.Interfaces
{
    public interface IClassAppointmentRepository: IGenericRepository<ClassAppointment>
    {
        Task<IEnumerable<ClassAppointment>> GetByTeacherIdAsync(string teacherId);
    }
}
