using Application.DTOs.AppointmentsDTOs;
using Application.Interfaces;
using AutoMapper;
using Domain.Entities;
using Domain.Wrappers;
using Infrastructure.Interfaces;

namespace Application.Services
{
    public class AppointmentService : IAppointmentService
    {
        private readonly IClassAppointmentRepository _classAppointmentRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public AppointmentService(IClassAppointmentRepository classAppointmentRepository, IUnitOfWork unitOfWork, IMapper mapper)
        {
            _classAppointmentRepository = classAppointmentRepository;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }
        private string GetStatus(DateTime start, DateTime end)
        {
            var now = DateTime.Now;

            if (now < start)
                return "Upcoming";
            else if (now >= start && now <= end)
                return "Running";
            else
                return "Finished";
        }
        public async Task<ServiceResult<IEnumerable<ViewAppointmentDto>>> GetAllAsync()
        {
            var result = await _classAppointmentRepository.GetAllAsync();
            if (result == null) return ServiceResult<IEnumerable<ViewAppointmentDto>>.Fail("Appointment not found");

            bool isChanged = false;
            foreach (var appointment in result)
            {
                var newStatus = GetStatus(appointment.StartTime, appointment.EndTime);
                if (appointment.Status != newStatus)
                {
                    appointment.Status = newStatus;
                    _classAppointmentRepository.Update(appointment);
                    isChanged = true;
                }
            }
            if (isChanged)
                await _unitOfWork.SaveChangesAsync();

            var resultDto = _mapper.Map<IEnumerable<ViewAppointmentDto>>(result);

            return ServiceResult<IEnumerable<ViewAppointmentDto>>.Ok(resultDto, "Appointment retrieved successfully");
        }
        public async Task<ServiceResult<ViewAppointmentDto>> GetByIdAsync(object id)
        {
            var result = await _classAppointmentRepository.GetByIdAsync(id);
            if (result == null) return ServiceResult<ViewAppointmentDto>.Fail("Appointment not found");

            bool isChanged = false;
            var newStatus = GetStatus(result.StartTime, result.EndTime);
            if (result.Status != newStatus)
            {
                result.Status = newStatus;
                _classAppointmentRepository.Update(result);
                isChanged = true;
            }
            if (isChanged)
                await _unitOfWork.SaveChangesAsync();

            var resultDto = _mapper.Map<ViewAppointmentDto>(result);
            return ServiceResult<ViewAppointmentDto>.Ok(resultDto, "Appointment retrieved successfully");
        }
        public async Task<ServiceResult<AppointmentDto>> CreateAsync(AppointmentDto dto)
        {
            var result = _mapper.Map<ClassAppointment>(dto);

            await _classAppointmentRepository.AddAsync(result);
            await _unitOfWork.SaveChangesAsync();

            var viewDto = _mapper.Map<AppointmentDto>(result);
            return ServiceResult<AppointmentDto>.Ok(viewDto, "Appointment created successfully");
        }
        public async Task<ServiceResult<AppointmentDto>> UpdateAsync(AppointmentDto dto)
        {
            var existingresult = await _classAppointmentRepository.GetByIdAsync(dto.Id!);
            if (existingresult == null)
                return ServiceResult<AppointmentDto>.Fail("Appointment not found");

            _mapper.Map(dto, existingresult);

            _classAppointmentRepository.Update(existingresult);
            await _unitOfWork.SaveChangesAsync();

            var viewDto = _mapper.Map<AppointmentDto>(existingresult);
            return ServiceResult<AppointmentDto>.Ok(viewDto, "Appointment updated successfully");
        }
        public async Task<ServiceResult<bool>> DeleteAsync(object id)
        {
            var result = await _classAppointmentRepository.GetByIdAsync(id);
            if (result == null)
                return ServiceResult<bool>.Fail("Appointment not found");

            _classAppointmentRepository.Delete(result);
            await _unitOfWork.SaveChangesAsync();
            return ServiceResult<bool>.Ok(true, "Appointment deleted successfully");
        }
    }
}
