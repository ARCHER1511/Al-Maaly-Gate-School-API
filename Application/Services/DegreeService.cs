using Application.DTOs.DegreesDTOs;
using Application.Interfaces;
using Domain.Entities;
using Domain.Wrappers;
using Infrastructure.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Application.Services
{
    public class DegreeService : IDegreeService
    {
        private readonly IUnitOfWork _unitOfWork;

        public DegreeService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        // ================================================
        // ADD DEGREES
        // ================================================
        public async Task<ServiceResult<string>> AddDegreesAsync(string studentId, List<Degree> degrees)
        {
            var student = await _unitOfWork.Students.GetByIdAsync(studentId);

            if (student == null)
                return ServiceResult<string>.Fail("Student not found");

            foreach (var degree in degrees)
            {
                degree.StudentId = studentId;
                await _unitOfWork.Degrees.AddAsync(degree);
            }

            await _unitOfWork.SaveChangesAsync();
            return ServiceResult<string>.Ok("Degrees added successfully");
        }

        // =====================================================
        // GET STUDENT + DEGREES
        // =====================================================
        public async Task<ServiceResult<StudentDegreesDto>> GetStudentDegreesAsync(string studentId)
        {
            var student = await _unitOfWork.Students.FirstOrDefaultAsync(
                s => s.Id == studentId,
                include: q => q
                    .Include(s => s.Class)
                    .Include(s => s.Degrees)
                        .ThenInclude(d => d.Subject)
            );

            if (student == null)
                return ServiceResult<StudentDegreesDto>.Fail("Student not found");

            var dto = new StudentDegreesDto
            {
                StudentId = student.Id,
                StudentName = student.FullName,
                ClassId = student.ClassId ?? "",
                ClassName = student.Class?.ClassName ?? "",
                Degrees = student.Degrees.Select(d => new DegreeItemDto
                {
                    DegreeId = d.Id,
                    SubjectId = d.SubjectId,
                    SubjectName = d.Subject.SubjectName,
                    Score = d.Score,
                    MaxScore = d.MaxScore,
                    DegreeType = d.DegreeType.ToString()
                }).ToList()
            };

            return ServiceResult<StudentDegreesDto>.Ok(dto);
        }

        // =====================================================
        // GET ALL STUDENTS + DEGREES
        // =====================================================
        public async Task<ServiceResult<List<StudentDegreesDto>>> GetAllStudentsDegreesAsync()
        {
            var students = await _unitOfWork.Students.FindAllAsync(
                predicate: s => true,
                include: q => q
                    .Include(s => s.Class)
                    .Include(s => s.Degrees)
                        .ThenInclude(d => d.Subject)
            );

            var result = students.Select(student => new StudentDegreesDto
            {
                StudentId = student.Id,
                StudentName = student.FullName,
                ClassId = student.ClassId ?? "",
                ClassName = student.Class?.ClassName ?? "",
                Degrees = student.Degrees.Select(d => new DegreeItemDto
                {
                    DegreeId = d.Id,
                    SubjectId = d.SubjectId,
                    SubjectName = d.Subject.SubjectName,
                    Score = d.Score,
                    MaxScore = d.MaxScore,
                    DegreeType = d.DegreeType.ToString()
                }).ToList()

            }).ToList();

            return ServiceResult<List<StudentDegreesDto>>.Ok(result);
        }
    }
}
