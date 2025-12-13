using Application.DTOs.ParentDTOs;
using Application.DTOs.StudentDTOs;
using Application.Interfaces;
using AutoMapper;
using Domain.Entities;
using Domain.Wrappers;
using Infrastructure.Interfaces;
using Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace Application.Services
{
    public class StudentService : IStudentService
    {
        private readonly IStudentRepository _studentRepository;
        private readonly IParentStudentRepository _ParentStudentRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public StudentService(IStudentRepository studentRepository, IUnitOfWork unitOfWork, IParentStudentRepository parentStudentRepository, IMapper mapper)
        {
            _studentRepository = studentRepository;
            _ParentStudentRepository = parentStudentRepository;
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
        public async Task<ServiceResult<List<StudentSearchResultDto>>> SearchStudentsAsync(SearchTermDto dto)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(dto.SearchTerm) || dto.SearchTerm.Length < 2)
                    return ServiceResult<List<StudentSearchResultDto>>.Ok(new List<StudentSearchResultDto>());

                var students = await _studentRepository.FindAllAsync(
                    s => s.AccountStatus == Domain.Enums.AccountStatus.Active &&
                    (s.FullName.Contains(dto.SearchTerm) ||
                     s.Email.Contains(dto.SearchTerm) ||
                     s.IqamaNumber.Contains(dto.SearchTerm) ||
                     s.PassportNumber.Contains(dto.SearchTerm))
                );

                if (students == null || !students.Any())
                    return ServiceResult<List<StudentSearchResultDto>>.Ok(new List<StudentSearchResultDto>());

                var studentIds = students.Select(s => s.Id).ToList();

                var parentRelationships = await _ParentStudentRepository
                    .FindAllAsync(ps => ps.ParentId == dto.ParentId && studentIds.Contains(ps.StudentId));

                var relatedStudentIds = new HashSet<string>(parentRelationships.Select(pr => pr.StudentId));

                var result = new List<StudentSearchResultDto>();

                foreach (var student in students)
                {
                    var ResultDto = new StudentSearchResultDto
                    {
                        Id = student.Id,
                        FullName = student.FullName,
                        Email = student.Email,
                        IqamaNumber = student.IqamaNumber,
                        PassportNumber = student.PassportNumber,
                        IsInRelation = relatedStudentIds.Contains(student.Id)
                    };

                    result.Add(ResultDto);
                }

                return ServiceResult<List<StudentSearchResultDto>>.Ok(result);
            }
            catch (Exception)
            {
                return ServiceResult<List<StudentSearchResultDto>>.Fail("An error occurred while searching for students");
            }
        }
    }
}
