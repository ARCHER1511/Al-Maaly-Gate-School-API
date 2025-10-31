using Application.DTOs.AppointmentsDTOs;
using Application.Interfaces;
using AutoMapper;
using Domain.Entities;
using Domain.Wrappers;
using Infrastructure.Interfaces;

namespace Application.Services
{
    public class ClassAppointmentService : IAppointmentService
    {
        private readonly IClassAppointmentRepository _classAppointmentRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public ClassAppointmentService(IClassAppointmentRepository classAppointmentRepository, IUnitOfWork unitOfWork, IMapper mapper)
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
        public async Task<ServiceResult<IEnumerable<ClassAppointmentDto>>> GetAllAsync()
        {
            var result = await _classAppointmentRepository.GetAllAsync();
            if (result == null) return ServiceResult<IEnumerable<ClassAppointmentDto>>.Fail("Appointment not found");

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

            var resultDto = _mapper.Map<IEnumerable<ClassAppointmentDto>>(result);

            return ServiceResult<IEnumerable<ClassAppointmentDto>>.Ok(resultDto, "Appointment retrieved successfully");
        }
        public async Task<ServiceResult<ClassAppointmentDto>> GetByIdAsync(object id)
        {
            var result = await _classAppointmentRepository.GetByIdAsync(id);
            if (result == null) return ServiceResult<ClassAppointmentDto>.Fail("Appointment not found");

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

            var resultDto = _mapper.Map<ClassAppointmentDto>(result);
            return ServiceResult<ClassAppointmentDto>.Ok(resultDto, "Appointment retrieved successfully");
        }
        public async Task<ServiceResult<ClassAppointmentDto>> CreateAsync(ClassAppointmentDto dto)
        {
            var result = _mapper.Map<ClassAppointment>(dto);

            await _classAppointmentRepository.AddAsync(result);
            await _unitOfWork.SaveChangesAsync();

            var viewDto = _mapper.Map<ClassAppointmentDto>(result);
            return ServiceResult<ClassAppointmentDto>.Ok(viewDto, "Appointment created successfully");
        }
        public async Task<ServiceResult<ClassAppointmentDto>> UpdateAsync(ClassAppointmentDto dto)
        {
            var existingresult = await _classAppointmentRepository.GetByIdAsync(dto.Id!);
            if (existingresult == null)
                return ServiceResult<ClassAppointmentDto>.Fail("Appointment not found");

            _mapper.Map(dto, existingresult);

            _classAppointmentRepository.Update(existingresult);
            await _unitOfWork.SaveChangesAsync();

            var viewDto = _mapper.Map<ClassAppointmentDto>(existingresult);
            return ServiceResult<ClassAppointmentDto>.Ok(viewDto, "Appointment updated successfully");
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
