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

        public async Task<ServiceResult<string>> AddDegreesAsync(string studentId, List<DegreeInput> degreeInputs)
        {
            var student = await _unitOfWork.Students.GetByIdAsync(studentId);

            if (student == null)
                return ServiceResult<string>.Fail("Student not found");

            foreach (var input in degreeInputs)
            {
                var degree = new Degree
                {
                    StudentId = studentId,
                    SubjectId = input.SubjectId,
                    DegreeType = input.DegreeType,

                    // Set component scores
                    OralScore = input.OralScore,
                    OralMaxScore = input.OralMaxScore,
                    ExamScore = input.ExamScore,
                    ExamMaxScore = input.ExamMaxScore,
                    PracticalScore = input.PracticalScore,
                    PracticalMaxScore = input.PracticalMaxScore
                };

                // Calculate total if components are provided
                if (input.OralScore.HasValue || input.ExamScore.HasValue || input.PracticalScore.HasValue)
                {
                    degree.CalculateTotalScore();
                }
                else if (input.Score.HasValue && input.MaxScore.HasValue)
                {
                    // Use direct total scores
                    degree.Score = input.Score.Value;
                    degree.MaxScore = input.MaxScore.Value;
                }
                else
                {
                    return ServiceResult<string>.Fail($"Either total scores or component scores must be provided for subject {input.SubjectId}");
                }

                // Get subject name
                var subject = await _unitOfWork.Subjects.GetByIdAsync(input.SubjectId);
                if (subject != null)
                {
                    degree.SubjectName = subject.SubjectName;
                }

                await _unitOfWork.Degrees.AddAsync(degree);
            }

            await _unitOfWork.SaveChangesAsync();
            return ServiceResult<string>.Ok("Degrees added successfully");
        }

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
                    DegreeType = d.DegreeType.ToString(),

                    // Total scores
                    Score = d.Score,
                    MaxScore = d.MaxScore,

                    // Component details
                    OralScore = d.OralScore,
                    OralMaxScore = d.OralMaxScore,
                    ExamScore = d.ExamScore,
                    ExamMaxScore = d.ExamMaxScore,
                    PracticalScore = d.PracticalScore,
                    PracticalMaxScore = d.PracticalMaxScore
                }).ToList()
            };

            return ServiceResult<StudentDegreesDto>.Ok(dto);
        }

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
                    DegreeType = d.DegreeType.ToString(),

                    // Total scores
                    Score = d.Score,
                    MaxScore = d.MaxScore,

                    // Component details
                    OralScore = d.OralScore,
                    OralMaxScore = d.OralMaxScore,
                    ExamScore = d.ExamScore,
                    ExamMaxScore = d.ExamMaxScore,
                    PracticalScore = d.PracticalScore,
                    PracticalMaxScore = d.PracticalMaxScore
                }).ToList()
            }).ToList();

            return ServiceResult<List<StudentDegreesDto>>.Ok(result);
        }
    }
}