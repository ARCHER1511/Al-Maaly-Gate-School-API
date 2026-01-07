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
        private readonly ICurriculumRepository _curriculumRepository; // Add this
        private readonly IGradeRepository _gradeRepository; // Add this
        private readonly IClassRepository _classRepository; // Add this
        public StudentService(
            IStudentRepository studentRepository,
            IUnitOfWork unitOfWork,
            IParentStudentRepository parentStudentRepository,
            IMapper mapper,
            ICurriculumRepository curriculumRepository,
            IGradeRepository gradeRepository,
            IClassRepository classRepository)
        {
            _studentRepository = studentRepository;
            _ParentStudentRepository = parentStudentRepository;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _curriculumRepository = curriculumRepository;
            _gradeRepository = gradeRepository;
            _classRepository = classRepository;
        }

        public async Task<ServiceResult<IEnumerable<StudentViewDto>>> GetAllAsync()
        {
            var students = await _studentRepository.GetAllWithDetailsAsync(); // Updated method

            // Return empty array instead of error
            if (students == null || !students.Any())
                return ServiceResult<IEnumerable<StudentViewDto>>.Ok(Enumerable.Empty<StudentViewDto>(), "No students found");

            var studentsDto = _mapper.Map<IEnumerable<StudentViewDto>>(students);
            return ServiceResult<IEnumerable<StudentViewDto>>.Ok(studentsDto, "Students retrieved successfully");
        }

        public async Task<ServiceResult<StudentViewDto>> GetByIdAsync(object id)
        {
            var student = await _studentRepository.GetByIdWithDetailsAsync(id); // Updated method
            if (student == null)
                return ServiceResult<StudentViewDto>.Fail("Student not found");

            var studentDto = _mapper.Map<StudentViewDto>(student);
            return ServiceResult<StudentViewDto>.Ok(studentDto, "Student retrieved successfully");
        }

        public async Task<ServiceResult<StudentViewDto>> CreateAsync(CreateStudentDto dto) // Changed parameter type
        {
            // Validate curriculum exists
            var curriculum = await _curriculumRepository.GetByIdAsync(dto.CurriculumId);
            if (curriculum == null)
                return ServiceResult<StudentViewDto>.Fail("Curriculum not found");

            // Validate class exists and belongs to same curriculum
            if (!string.IsNullOrEmpty(dto.ClassId))
            {
                var classEntity = await _classRepository.GetByIdAsync(dto.ClassId);
                if (classEntity == null)
                    return ServiceResult<StudentViewDto>.Fail("Class not found");

                // Check if class belongs to same curriculum
                if (classEntity.Grade?.CurriculumId != dto.CurriculumId)
                    return ServiceResult<StudentViewDto>.Fail("Class does not belong to the selected curriculum");
            }

            var student = _mapper.Map<Student>(dto);
            student.Id = Guid.NewGuid().ToString();

            await _studentRepository.AddAsync(student);
            await _unitOfWork.SaveChangesAsync();

            // Load with details for response
            var createdStudent = await _studentRepository.GetByIdWithDetailsAsync(student.Id);
            var viewDto = _mapper.Map<StudentViewDto>(createdStudent ?? student);
            return ServiceResult<StudentViewDto>.Ok(viewDto, "Student created successfully");
        }

        public async Task<ServiceResult<StudentViewDto>> UpdateStudentAdditionalInfoAsync(string id, UpdateStudentDto dto)
        {
            var existingStudent = await _studentRepository.GetByIdAsync(id);
            if (existingStudent == null)
                return ServiceResult<StudentViewDto>.Fail("Student not found");

            existingStudent.PassportNumber = dto.PassportNumber;
            existingStudent.Nationality = dto.Nationality;
            existingStudent.IqamaNumber = dto.IqamaNumber;

            _studentRepository.Update(existingStudent);
            await _unitOfWork.SaveChangesAsync();

            var updatedStudent = await _studentRepository.GetByIdWithDetailsAsync(id);
            var viewDto = _mapper.Map<StudentViewDto>(updatedStudent ?? existingStudent);
            return ServiceResult<StudentViewDto>.Ok(viewDto, "Student additional info updated successfully");
        }

        public async Task<ServiceResult<StudentViewDto>> UpdateAsync(string id, UpdateStudentDto dto) // Changed parameters
        {
            var existingStudent = await _studentRepository.GetByIdAsync(id);
            if (existingStudent == null)
                return ServiceResult<StudentViewDto>.Fail("Student not found");

            // Validate curriculum exists if changing
            if (dto.CurriculumId != existingStudent.CurriculumId)
            {
                var curriculum = await _curriculumRepository.GetByIdAsync(dto.CurriculumId);
                if (curriculum == null)
                    return ServiceResult<StudentViewDto>.Fail("Curriculum not found");
            }

            // Validate class exists and belongs to same curriculum
            if (dto.ClassId != existingStudent.ClassId && !string.IsNullOrEmpty(dto.ClassId))
            {
                var classEntity = await _classRepository.GetByIdAsync(dto.ClassId);
                if (classEntity == null)
                    return ServiceResult<StudentViewDto>.Fail("Class not found");

                // Check if class belongs to same curriculum
                if (classEntity.Grade?.CurriculumId != dto.CurriculumId)
                    return ServiceResult<StudentViewDto>.Fail("Class does not belong to the selected curriculum");
            }

            _mapper.Map(dto, existingStudent);
            _studentRepository.Update(existingStudent);
            await _unitOfWork.SaveChangesAsync();

            // Load with details for response
            var updatedStudent = await _studentRepository.GetByIdWithDetailsAsync(id);
            var viewDto = _mapper.Map<StudentViewDto>(updatedStudent ?? existingStudent);
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

        // New method: Get students by curriculum
        public async Task<ServiceResult<IEnumerable<StudentViewDto>>> GetStudentsByCurriculumAsync(string curriculumId)
        {
            var students = await _studentRepository.GetStudentsByCurriculumAsync(curriculumId);
            if (students == null || !students.Any())
                return ServiceResult<IEnumerable<StudentViewDto>>.Fail("No students found in this curriculum");

            var studentsDto = _mapper.Map<IEnumerable<StudentViewDto>>(students);
            return ServiceResult<IEnumerable<StudentViewDto>>.Ok(studentsDto, "Students retrieved successfully");
        }

        // New method: Move student to different curriculum
        public async Task<ServiceResult<StudentViewDto>> MoveStudentToCurriculumAsync(string studentId, string newCurriculumId)
        {
            var student = await _studentRepository.GetByIdAsync(studentId);
            if (student == null)
                return ServiceResult<StudentViewDto>.Fail("Student not found");

            // Check if curriculum exists
            var curriculum = await _curriculumRepository.GetByIdAsync(newCurriculumId);
            if (curriculum == null)
                return ServiceResult<StudentViewDto>.Fail("Curriculum not found");

            // If student has a class, check if it belongs to new curriculum
            if (!string.IsNullOrEmpty(student.ClassId))
            {
                var classEntity = await _classRepository.GetByIdAsync(student.ClassId);
                if (classEntity?.Grade?.CurriculumId != newCurriculumId)
                {
                    // Remove class assignment if it doesn't belong to new curriculum
                    student.ClassId = null;
                }
            }

            student.CurriculumId = newCurriculumId;
            _studentRepository.Update(student);
            await _unitOfWork.SaveChangesAsync();

            var updatedStudent = await _studentRepository.GetByIdWithDetailsAsync(studentId);
            var viewDto = _mapper.Map<StudentViewDto>(updatedStudent ?? student);
            return ServiceResult<StudentViewDto>.Ok(viewDto, "Student moved to new curriculum successfully");
        }

        // New method: Get student count by curriculum
        public async Task<ServiceResult<int>> GetStudentCountByCurriculumAsync(string curriculumId)
        {
            var count = await _studentRepository.GetStudentCountByCurriculumAsync(curriculumId);
            return ServiceResult<int>.Ok(count, $"Total students in curriculum: {count}");
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