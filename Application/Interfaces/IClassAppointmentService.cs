
using Application.DTOs.AppointmentsDTOs;
using Domain.Wrappers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


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
    }
}
