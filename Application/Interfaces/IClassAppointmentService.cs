﻿
using Application.DTOs.AppointmentsDTOs;
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
    }
}
