using Application.DTOs.ClassDTOs;
using Application.Interfaces;
using AutoMapper;
using Domain.Entities;
using Domain.Wrappers;
using Infrastructure.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Application.Services
{
    public class ClassService : IClassService
    {
        private readonly IClassRepository _classRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public ClassService(IClassRepository classRepository, IUnitOfWork unitOfWork, IMapper mapper)
        {
            _classRepository = classRepository;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<ServiceResult<IEnumerable<ClassViewDto>>> GetAllAsync()
        {
            var result = await _classRepository.FindAllAsync(
                include: q => q.Include(t => t.TeacherClasses)
                               .ThenInclude(tc => tc.Teacher)
                              .Include(s => s.Students)
                              .Include(ca => ca.ClassAssets)
                              .Include(cp => cp.ClassAppointments)
                              .Include(c => c.Grade)!); // ADDED: Include Grade

            if (result == null)
                return ServiceResult<IEnumerable<ClassViewDto>>.Fail("Classes not found");

            var resultDto = _mapper.Map<IEnumerable<ClassViewDto>>(result);
            return ServiceResult<IEnumerable<ClassViewDto>>.Ok(resultDto, "Classes retrieved successfully");
        }

        public async Task<ServiceResult<ClassViewDto>> GetByIdAsync(object id)
        {
            var result = await _classRepository.GetByIdAsync(id);
            if (result == null)
                return ServiceResult<ClassViewDto>.Fail("Class not found");

            var resultDto = _mapper.Map<ClassViewDto>(result);
            return ServiceResult<ClassViewDto>.Ok(resultDto, "Class retrieved successfully");
        }

        public async Task<ServiceResult<ClassDto>> CreateAsync(CreateClassDto dto)
        {
            var result = _mapper.Map<Class>(dto);
            result.Id = Guid.NewGuid().ToString(); // Always generate new ID

            await _classRepository.AddAsync(result);
            await _unitOfWork.SaveChangesAsync();

            var viewDto = _mapper.Map<ClassDto>(result);
            return ServiceResult<ClassDto>.Ok(viewDto, "Class created successfully");
        }

        public async Task<ServiceResult<ClassDto>> UpdateAsync(UpdateClassDto dto)
        {
            var existingResult = await _classRepository.GetByIdAsync(dto.Id);
            if (existingResult == null)
                return ServiceResult<ClassDto>.Fail("Class not found");

            _mapper.Map(dto, existingResult);

            _classRepository.Update(existingResult);
            await _unitOfWork.SaveChangesAsync();

            var viewDto = _mapper.Map<ClassDto>(existingResult);
            return ServiceResult<ClassDto>.Ok(viewDto, "Class updated successfully");
        }

        public async Task<ServiceResult<bool>> DeleteAsync(object id)
        {
            var result = await _classRepository.GetByIdAsync(id);
            if (result == null)
                return ServiceResult<bool>.Fail("Class not found");

            _classRepository.Delete(result);
            await _unitOfWork.SaveChangesAsync();
            return ServiceResult<bool>.Ok(true, "Class deleted successfully");
        }

        public async Task<ServiceResult<IEnumerable<ClassViewDto>>> GetAllWithTeachersAsync()
        {
            var result = await _classRepository.GetAllWithTeachersAsync();

            if (result == null || !result.Any())
                return ServiceResult<IEnumerable<ClassViewDto>>.Fail("No classes found");

            var dto = _mapper.Map<IEnumerable<ClassViewDto>>(result);
            return ServiceResult<IEnumerable<ClassViewDto>>.Ok(dto, "Classes with teachers retrieved successfully");
        }

        public async Task<ServiceResult<List<Student>>> GetStudentsByClassIdAsync(string classId)
        {
            var students = await _classRepository.GetStudentsByClassIdAsync(classId);
            return ServiceResult<List<Student>>.Ok(students);
        }

        public async Task<ServiceResult<List<Subject>>> GetSubjectsByClassIdAsync(string classId)
        {
            var subjects = await _classRepository.GetSubjectsByClassIdAsync(classId);
            return ServiceResult<List<Subject>>.Ok(subjects);
        }
    }
}