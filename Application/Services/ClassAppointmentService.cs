using Application.DTOs.ClassAppointmentDTOs;
using Application.Interfaces;
using AutoMapper;
using Domain.Entities;
using Domain.Wrappers;
using Infrastructure.Interfaces;

public class ClassAppointmentService : IClassAppointmentService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public ClassAppointmentService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<ServiceResult<IEnumerable<ClassAppointmentViewDto>>> GetAllAsync()
    {
        var repo = _unitOfWork.Repository<ClassAppointment>();
        var appointments = await repo.GetAllAsync();
        var result = appointments.Select(_mapper.Map<ClassAppointmentViewDto>);
        return ServiceResult<IEnumerable<ClassAppointmentViewDto>>.Ok(result);
    }

    public async Task<ServiceResult<ClassAppointmentViewDto>> GetByIdAsync(string id)
    {
        var repo = _unitOfWork.Repository<ClassAppointment>();
        var appointment = await repo.GetByIdAsync(id);
        if (appointment == null)
            return ServiceResult<ClassAppointmentViewDto>.Fail("Appointment not found");

        return ServiceResult<ClassAppointmentViewDto>.Ok(_mapper.Map<ClassAppointmentViewDto>(appointment));
    }

    public async Task<ServiceResult<IEnumerable<ClassAppointmentViewDto>>> GetByTeacherAsync(string teacherId)
    {
        var repo = _unitOfWork.Repository<ClassAppointment>();
        var appointments = await repo.FindAllAsync(a => a.TeacherId == teacherId);
        var result = appointments.Select(_mapper.Map<ClassAppointmentViewDto>);
        return ServiceResult<IEnumerable<ClassAppointmentViewDto>>.Ok(result);
    }

    public async Task<ServiceResult<ClassAppointmentViewDto>> CreateAsync(CreateClassAppointmentDto dto)
    {
        var repo = _unitOfWork.Repository<ClassAppointment>();
        var appointment = _mapper.Map<ClassAppointment>(dto);
        await repo.AddAsync(appointment);
        await _unitOfWork.SaveChangesAsync();
        return ServiceResult<ClassAppointmentViewDto>.Ok(_mapper.Map<ClassAppointmentViewDto>(appointment));
    }

    public async Task<ServiceResult<ClassAppointmentViewDto>> UpdateAsync(string id, UpdateClassAppointmentDto dto)
    {
        var repo = _unitOfWork.Repository<ClassAppointment>();
        var appointment = await repo.GetByIdAsync(id);
        if (appointment == null)
            return ServiceResult<ClassAppointmentViewDto>.Fail("Appointment not found");

        _mapper.Map(dto, appointment);
        repo.Update(appointment);
        await _unitOfWork.SaveChangesAsync();

        return ServiceResult<ClassAppointmentViewDto>.Ok(_mapper.Map<ClassAppointmentViewDto>(appointment));
    }

    public async Task<ServiceResult<bool>> DeleteAsync(string id)
    {
        var repo = _unitOfWork.Repository<ClassAppointment>();
        var appointment = await repo.GetByIdAsync(id);
        if (appointment == null)
            return ServiceResult<bool>.Fail("Appointment not found");

        repo.Delete(appointment);
        await _unitOfWork.SaveChangesAsync();
        return ServiceResult<bool>.Ok(true);
    }
}