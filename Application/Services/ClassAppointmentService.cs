using Application.DTOs.AppointmentsDTOs;
using Application.DTOs.ClassAppointmentsDTOs;
using Application.Interfaces;
using AutoMapper;
using Domain.Entities;
using Domain.Wrappers;
using Infrastructure.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Application.Services
{
    public class ClassAppointmentService : IClassAppointmentService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public ClassAppointmentService(
            IUnitOfWork unitOfWork,
            IMapper mapper
        )
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        private string GetStatus(DateTime start, DateTime end)
        {
            var now = DateTimeOffset.Now;

            if (now < start)
                return "Upcoming";
            else if (now >= start && now <= end)
                return "Running";
            else
                return "Finished";
        }

        public async Task<
            ServiceResult<IEnumerable<ClassAppointmentDto>>
        > GetAppointmentsByTeacherAsync(string teacherId)
        {
            var result = await _unitOfWork
                .ClassAppointmentRepository.AsQueryable()
                .Where(a => a.TeacherId == teacherId)
                .ToListAsync();
            if (result == null)
                return ServiceResult<IEnumerable<ClassAppointmentDto>>.Fail(
                    "Appointment not found"
                );

            bool isChanged = false;
            foreach (var appointment in result)
            {
                var newStatus = GetStatus(appointment.StartTime, appointment.EndTime);
                if (appointment.Status != newStatus)
                {
                    appointment.Status = newStatus;
                    _unitOfWork.ClassAppointmentRepository.Update(appointment);
                    isChanged = true;
                }
            }
            if (isChanged)
                await _unitOfWork.SaveChangesAsync();

            var resultDto = _mapper.Map<IEnumerable<ClassAppointmentDto>>(result);

            return ServiceResult<IEnumerable<ClassAppointmentDto>>.Ok(
                resultDto,
                "Appointment retrieved successfully"
            );
        }

        public async Task<ServiceResult<IEnumerable<ClassAppointmentDto>>> GetAllAsync()
        {
            var result = await _unitOfWork.ClassAppointmentRepository.GetAllAsync();
            if (result == null)
                return ServiceResult<IEnumerable<ClassAppointmentDto>>.Fail(
                    "Appointment not found"
                );

            bool isChanged = false;
            foreach (var appointment in result)
            {
                var newStatus = GetStatus(appointment.StartTime, appointment.EndTime);
                if (appointment.Status != newStatus)
                {
                    appointment.Status = newStatus;
                    _unitOfWork.ClassAppointmentRepository.Update(appointment);
                    isChanged = true;
                }
            }
            if (isChanged)
                await _unitOfWork.SaveChangesAsync();

            var resultDto = _mapper.Map<IEnumerable<ClassAppointmentDto>>(result);

            return ServiceResult<IEnumerable<ClassAppointmentDto>>.Ok(
                resultDto,
                "Appointment retrieved successfully"
            );
        }

        public async Task<ServiceResult<ClassAppointmentDto>> GetByIdAsync(object id)
        {
            var result = await _unitOfWork.ClassAppointmentRepository.GetByIdAsync(id);
            if (result == null)
                return ServiceResult<ClassAppointmentDto>.Fail("Appointment not found");

            bool isChanged = false;
            var newStatus = GetStatus(result.StartTime, result.EndTime);
            if (result.Status != newStatus)
            {
                result.Status = newStatus;
                _unitOfWork.ClassAppointmentRepository.Update(result);
                isChanged = true;
            }
            if (isChanged)
                await _unitOfWork.SaveChangesAsync();

            var resultDto = _mapper.Map<ClassAppointmentDto>(result);
            return ServiceResult<ClassAppointmentDto>.Ok(
                resultDto,
                "Appointment retrieved successfully"
            );
        }

        public async Task<ServiceResult<ClassAppointmentDto>> CreateAsync(ClassAppointmentDto dto)
        {
            var result = _mapper.Map<ClassAppointment>(dto);

            result.StartTime = dto.StartTime.ToUniversalTime();
            result.EndTime = dto.EndTime.ToUniversalTime();

            await _classAppointmentRepository.AddAsync(result);
        await _unitOfWork.SaveChangesAsync();

            var viewDto = _mapper.Map<ClassAppointmentDto>(result);
            return ServiceResult<ClassAppointmentDto>.Ok(
                viewDto,
                "Appointment created successfully"
            );
        }

        public async Task<ServiceResult<ClassAppointmentDto>> UpdateAsync(ClassAppointmentDto dto)
        {
            var existingresult = await _unitOfWork.ClassAppointmentRepository.GetByIdAsync(dto.Id!);
            if (existingresult == null)
                return ServiceResult<ClassAppointmentDto>.Fail("Appointment not found");

            _mapper.Map(dto, existingresult);

            _unitOfWork.ClassAppointmentRepository.Update(existingresult);
            await _unitOfWork.SaveChangesAsync();

            var viewDto = _mapper.Map<ClassAppointmentDto>(existingresult);
            return ServiceResult<ClassAppointmentDto>.Ok(
                viewDto,
                "Appointment updated successfully"
            );
        }

        public async Task<ServiceResult<bool>> DeleteAsync(object id)
        {
            var result = await _unitOfWork.ClassAppointmentRepository.GetByIdAsync(id);
            if (result == null)
                return ServiceResult<bool>.Fail("Appointment not found");

            _unitOfWork.ClassAppointmentRepository.Delete(result);
            await _unitOfWork.SaveChangesAsync();
            return ServiceResult<bool>.Ok(true, "Appointment deleted successfully");
        }

        public async Task<
            ServiceResult<IEnumerable<StudentClassAppointmentDto>>
        > GetAppointmentsForStudentByClassIdAsync(string ClassId)
        {
            Console.WriteLine($"=== DEBUG: GetAppointmentsForStudentByClassIdAsync called ===");
            Console.WriteLine($"ClassId: {ClassId}");

            try
            {
                // 1. First check if the class exists
                var classExists = await _unitOfWork
                    .Repository<Class>()
                    .AsQueryable()
                    .AnyAsync(c => c.Id == ClassId);
                Console.WriteLine($"Class exists in database: {classExists}");

                // 2. Check appointments without includes first
                var appointmentsQuery = _unitOfWork
                    .ClassAppointmentRepository.AsQueryable()
                    .Where(c => c.ClassId == ClassId);

                var appointmentCount = await appointmentsQuery.CountAsync();
                Console.WriteLine($"Found {appointmentCount} appointments with ClassId: {ClassId}");

                // 3. Get appointments with includes
                var result = await appointmentsQuery
                    .Include(c => c.Subject)
                    .Include(c => c.Teacher)
                    .ToListAsync();

                Console.WriteLine($"After Include, retrieved {result?.Count ?? 0} appointments");

                if (result == null || !result.Any())
                {
                    // Check if there are ANY appointments in the system
                    var totalAppointments = await _unitOfWork
                        .ClassAppointmentRepository.AsQueryable()
                        .CountAsync();
                    Console.WriteLine($"Total appointments in database: {totalAppointments}");

                    // Check what appointments exist in database
                    var allAppointments = await _unitOfWork
                        .ClassAppointmentRepository.AsQueryable()
                        .Take(10)
                        .Select(c => new
                        {
                            c.Id,
                            c.ClassId,
                            c.StartTime,
                            c.EndTime,
                        })
                        .ToListAsync();

                    Console.WriteLine("Sample appointments in database:");
                    foreach (var app in allAppointments)
                    {
                        Console.WriteLine(
                            $"  - ID: {app.Id}, ClassId: {app.ClassId}, Start: {app.StartTime}, End: {app.EndTime}"
                        );
                    }

                    return ServiceResult<IEnumerable<StudentClassAppointmentDto>>.Ok(
                        new List<StudentClassAppointmentDto>(),
                        "No appointments found for this class"
                    );
                }

                // 4. Log details about found appointments
                Console.WriteLine("Found appointments details:");
                foreach (var appointment in result)
                {
                    Console.WriteLine($"  - ID: {appointment.Id}");
                    Console.WriteLine($"    StartTime: {appointment.StartTime}");
                    Console.WriteLine($"    EndTime: {appointment.EndTime}");
                    Console.WriteLine($"    SubjectId: {appointment.SubjectId}");
                    Console.WriteLine(
                        $"    Subject: {(appointment.Subject != null ? appointment.Subject.SubjectName : "NULL")}"
                    );
                    Console.WriteLine($"    TeacherId: {appointment.TeacherId}");
                    Console.WriteLine(
                        $"    Teacher: {(appointment.Teacher != null ? appointment.Teacher.FullName : "NULL")}"
                    );
                }

                // 5. Check status updates
                bool isChanged = false;
                foreach (var appointment in result)
                {
                    var newStatus = GetStatus(appointment.StartTime, appointment.EndTime);
                    if (appointment.Status != newStatus)
                    {
                        Console.WriteLine(
                            $"Updating status for appointment {appointment.Id}: {appointment.Status} -> {newStatus}"
                        );
                        appointment.Status = newStatus;
                        _unitOfWork.ClassAppointmentRepository.Update(appointment);
                        isChanged = true;
                    }
                }

                if (isChanged)
                {
                    await _unitOfWork.SaveChangesAsync();
                    Console.WriteLine("Saved status updates to database");
                }

                // 6. Test mapping manually
                Console.WriteLine("Testing manual mapping...");
                var manualDtos = result
                    .Select(appointment => new StudentClassAppointmentDto
                    {
                        Id = appointment.Id,
                        StartTime = appointment.StartTime,
                        EndTime = appointment.EndTime,
                        Link = appointment.Link,
                        Status = appointment.Status,
                        SubjectId = appointment.SubjectId,
                        SubjectName = appointment.Subject?.SubjectName ?? string.Empty,
                        TeacherId = appointment.TeacherId,
                        TeacherName = appointment.Teacher?.FullName ?? string.Empty,
                    })
                    .ToList();

                Console.WriteLine($"Manual mapping created {manualDtos.Count} DTOs");

                // 7. Test AutoMapper
                var autoMapperDtos = _mapper
                    .Map<IEnumerable<StudentClassAppointmentDto>>(result)
                    .ToList();
                Console.WriteLine($"AutoMapper created {autoMapperDtos.Count} DTOs");

                return ServiceResult<IEnumerable<StudentClassAppointmentDto>>.Ok(
                    autoMapperDtos,
                    "Appointments retrieved successfully"
                );
            }
            catch (Exception ex)
            {
                Console.WriteLine($"ERROR: {ex.Message}");
                Console.WriteLine($"Stack Trace: {ex.StackTrace}");
                return ServiceResult<IEnumerable<StudentClassAppointmentDto>>.Fail(
                    $"Error: {ex.Message}"
                );
            }
        }
    }
}
