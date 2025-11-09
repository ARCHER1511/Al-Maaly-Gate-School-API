
using Application.DTOs.AppointmentsDTOs;
using Application.DTOs.ClassAppointmentsDTOs;
using Domain.Wrappers;

namespace Application.Interfaces
{
    public interface IClassAppointmentService
    {
        Task<ServiceResult<ClassAppointmentDto>> GetByIdAsync(object id);
        Task<ServiceResult<IEnumerable<ClassAppointmentDto>>> GetAllAsync();
        Task<ServiceResult<ClassAppointmentDto>> CreateAsync(ClassAppointmentDto dto);
        Task<ServiceResult<ClassAppointmentDto>> UpdateAsync(ClassAppointmentDto admin);
        Task<ServiceResult<bool>> DeleteAsync(object id);
        Task<ServiceResult<IEnumerable<ClassAppointmentDto>>> GetAppointmentsByTeacherAsync(string teacherId);
        Task<ServiceResult<IEnumerable<StudentClassAppointmentDto>>> GetAppointmentsForStudentByClassIdAsync(string ClassId);
    }
}
