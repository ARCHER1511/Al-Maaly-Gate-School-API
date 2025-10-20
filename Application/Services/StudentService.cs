using Application.DTOs.StudentDTOs;
using Application.Interfaces;
using AutoMapper;
using Domain.Entities;
using Domain.Wrappers;
using Infrastructure.Interfaces;

namespace Application.Services
{
    public class StudentService : IStudentService
    {
        private readonly IStudentRepository _studentRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public StudentService(IStudentRepository studentRepository, IUnitOfWork unitOfWork, IMapper mapper)
        {
            _studentRepository = studentRepository;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<ServiceResult<IEnumerable<StudentViewDto>>> GetAllAsync()
        {
            var students = await _studentRepository.GetAllAsync();
            if (students == null) return ServiceResult<IEnumerable<StudentViewDto>>.Fail("students not found");

            var studentsDto = _mapper.Map<IEnumerable<StudentViewDto>>(students);
            return ServiceResult<IEnumerable<StudentViewDto>>.Ok(studentsDto, "Students retrieved successfully");
        }
        public async Task<ServiceResult<StudentViewDto>> GetByIdAsync(object id)
        {
            var student = await _studentRepository.GetByIdAsync(id);
            if (student == null) return ServiceResult<StudentViewDto>.Fail("student not found");

            var studentDto = _mapper.Map<StudentViewDto>(student);
            return ServiceResult<StudentViewDto>.Ok(studentDto, "Student retrieved successfully");
        }
        public async Task<ServiceResult<StudentViewDto>> CreateAsync(StudentViewDto dto)
        {
            var student = _mapper.Map<Student>(dto);

            await _studentRepository.AddAsync(student);
            await _unitOfWork.SaveChangesAsync();

            var viewDto = _mapper.Map<StudentViewDto>(student);
            return ServiceResult<StudentViewDto>.Ok(viewDto, "Student created successfully");
        }
        public async Task<ServiceResult<StudentViewDto>> UpdateAsync(StudentViewDto dto)
        {
            var existingStudent = await _studentRepository.GetByIdAsync(dto.Id);
            if (existingStudent == null)
                return ServiceResult<StudentViewDto>.Fail("Student not found");

            _mapper.Map(dto, existingStudent);

            _studentRepository.Update(existingStudent);
            await _unitOfWork.SaveChangesAsync();

            var viewDto = _mapper.Map<StudentViewDto>(existingStudent);
            return ServiceResult<StudentViewDto>.Ok(viewDto, "Student updated successfully");
        }
        public async Task<ServiceResult<bool>> DeleteAsync(object id)
        {
            var student = await _studentRepository.GetByIdAsync(id);
            if (student == null)
                return ServiceResult<bool>.Fail("Student not found");

            _studentRepository.Delete(student);
            await _unitOfWork.SaveChangesAsync();
            return ServiceResult<bool>.Ok(true, "Student deleted successfully");
        }
    }
}
