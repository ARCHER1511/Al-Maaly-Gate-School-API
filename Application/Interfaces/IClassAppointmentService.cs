using Application.DTOs.ClassAppointmentDTOs;
using Domain.Entities;
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
        Task<ServiceResult<IEnumerable<ClassAppointmentViewDto>>> GetAllAsync();
        Task<ServiceResult<ClassAppointmentViewDto>> GetByIdAsync(string id);
        Task<ServiceResult<IEnumerable<ClassAppointmentViewDto>>> GetByTeacherAsync(string teacherId);
        Task<ServiceResult<ClassAppointmentViewDto>> CreateAsync(CreateClassAppointmentDto dto);
        Task<ServiceResult<ClassAppointmentViewDto>> UpdateAsync(string id, UpdateClassAppointmentDto dto);
        Task<ServiceResult<bool>> DeleteAsync(string id);
    }
}
