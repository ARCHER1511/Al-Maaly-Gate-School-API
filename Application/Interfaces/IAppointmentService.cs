
using Application.DTOs.AppointmentsDTOs;
using Domain.Wrappers;


namespace Application.Interfaces
{
    public interface IAppointmentService
    {
        Task<ServiceResult<ViewAppointmentDto>> GetByIdAsync(object id);
        Task<ServiceResult<IEnumerable<ViewAppointmentDto>>> GetAllAsync();
        Task<ServiceResult<AppointmentDto>> CreateAsync(AppointmentDto dto);
        Task<ServiceResult<AppointmentDto>> UpdateAsync(AppointmentDto admin);
        Task<ServiceResult<bool>> DeleteAsync(object id);
    }
}
