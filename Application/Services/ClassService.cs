using Application.DTOs.ClassDTOs;
using Application.Interfaces;
using AutoMapper;
using Domain.Entities;
using Domain.Wrappers;
using SpreadsheetLight;
using Infrastructure.Interfaces;
using Microsoft.EntityFrameworkCore;
using static iTextSharp.text.TabStop;
using OfficeOpenXml;
using DocumentFormat.OpenXml.Spreadsheet;

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

        public async Task<ServiceResult<ClassStatisticsDto>> GetClassStatisticsAsync(string classId)
        {
            var classRepo = _unitOfWork.Repository<Class>();
            var teacherClassRepo = _unitOfWork.Repository<TeacherClass>();
            var examRepo = _unitOfWork.Repository<Exam>();
            var degreeRepo = _unitOfWork.Repository<Degree>();

            // Get the class
            var classEntity = await classRepo.FirstOrDefaultAsync(
                c => c.Id == classId,
                q => q.Include(c => c.Students)
                      .Include(c => c.Exams)
            );

            if (classEntity == null)
                return ServiceResult<ClassStatisticsDto>.Fail("Class not found.");

            // Get statistics data
            var totalStudents = classEntity.Students?.Count ?? 0;

            // Get teachers assigned to this class
            var teacherCount = await teacherClassRepo.FindAllAsync(tc => tc.ClassId == classId);

            // Get completed exams (using Status property instead of IsCompleted)
            var completedExams = classEntity.Exams?.Count(e => e.Status == "Completed") ?? 0;

            // Get upcoming exams as "pending assignments"
            var pendingExams = classEntity.Exams?.Count(e => e.Status == "Upcoming") ?? 0;

            // Calculate average scores from degrees
            double averageScore = 0;
            if (totalStudents > 0)
            {
                var studentIds = classEntity.Students?.Select(s => s.Id).ToList() ?? new List<string>();
                if (studentIds.Any())
                {
                    var degrees = await degreeRepo.FindAllAsync(d => studentIds.Contains(d.StudentId));
                    averageScore = degrees.Any() ? degrees.Average(d => d.Score) : 0;
                }
            }

            var statistics = new ClassStatisticsDto
            {
                ClassId = classEntity.Id,
                ClassName = classEntity.ClassName,
                AverageGpa = Math.Round(averageScore, 2), // Using average scores as GPA
                AttendanceRate = 0, // Not available in your entities
                CompletedExams = completedExams,
                PendingAssignments = pendingExams, // Using upcoming exams as pending assignments
                TotalStudents = totalStudents,
                TotalTeachers = teacherCount.Count()
            };

            return ServiceResult<ClassStatisticsDto>.Ok(statistics, "Class statistics retrieved successfully.");
        }

        public async Task<ServiceResult<byte[]>> ExportClassDataAsync(string classId)
        {
            try
            {
                var classRepo = _unitOfWork.Repository<Class>();
                var degreeRepo = _unitOfWork.Repository<Degree>();

                // Get class with related data
                var classEntity = await classRepo.FirstOrDefaultAsync(
                    c => c.Id == classId,
                    q => q.Include(c => c.Students)
                          .ThenInclude(s => s.AppUser)
                          .Include(c => c.TeacherClasses)
                          .ThenInclude(tc => tc.Teacher)
                          .ThenInclude(t => t.AppUser)
                          .Include(c => c.Grade)
                );

                if (classEntity == null)
                    return ServiceResult<byte[]>.Fail("Class not found.");

                // Get student scores from degrees
                var studentIds = classEntity.Students?.Select(s => s.Id).ToList() ?? new List<string>();
                var studentDegrees = studentIds.Any()
                    ? await degreeRepo.FindAllAsync(d => studentIds.Contains(d.StudentId))
                    : new List<Degree>();

                // Create Excel document using SpreadsheetLight
                using var sl = new SLDocument();
                var worksheet = sl;
                sl.RenameWorksheet(SLDocument.DefaultFirstSheetName, "Class Data");

                int row = 1;

                // Add header row
                sl.SetCellValue(row, 1, "Class Information");
                sl.SetCellStyle(row, 1, row, 1, CreateHeaderStyle());

                sl.SetCellValue(row + 1, 1, "Class Name");
                sl.SetCellValue(row + 1, 2, classEntity.ClassName);

                sl.SetCellValue(row + 2, 1, "Grade");
                sl.SetCellValue(row + 2, 2, classEntity.Grade?.GradeName ?? "N/A");

                // Add students section
                row = 5;
                sl.SetCellValue(row, 1, "Students");
                sl.SetCellStyle(row, 1, row, 1, CreateHeaderStyle());
                row++;

                // Student headers
                string[] studentHeaders = { "Student Name", "Email", "Class Year", "Age", "Average Score", "Subject Count" };
                for (int col = 0; col < studentHeaders.Length; col++)
                {
                    sl.SetCellValue(row, col + 1, studentHeaders[col]);
                }
                sl.SetCellStyle(row, 1, row, studentHeaders.Length, CreateHeaderStyle());
                row++;

                if (classEntity.Students != null)
                {
                    foreach (var student in classEntity.Students)
                    {
                        var studentDegreesList = studentDegrees.Where(d => d.StudentId == student.Id).ToList();
                        var averageScore = studentDegreesList.Any() ? studentDegreesList.Average(d => d.Score) : 0;
                        var subjectCount = studentDegreesList.Select(d => d.SubjectId).Distinct().Count();

                        sl.SetCellValue(row, 1, student.AppUser?.FullName ?? "N/A");
                        sl.SetCellValue(row, 2, student.Email);
                        sl.SetCellValue(row, 3, student.ClassYear);
                        sl.SetCellValue(row, 4, student.Age);
                        sl.SetCellValue(row, 5, Math.Round(averageScore, 2));
                        sl.SetCellValue(row, 6, subjectCount);
                        row++;
                    }
                }

                // Add teachers section
                row += 2;
                sl.SetCellValue(row, 1, "Teachers");
                sl.SetCellStyle(row, 1, row, 1, CreateHeaderStyle());
                row++;

                string[] teacherHeaders = { "Teacher Name", "Email" };
                for (int col = 0; col < teacherHeaders.Length; col++)
                {
                    sl.SetCellValue(row, col + 1, teacherHeaders[col]);
                }
                sl.SetCellStyle(row, 1, row, teacherHeaders.Length, CreateHeaderStyle());
                row++;

                if (classEntity.TeacherClasses != null)
                {
                    foreach (var teacherClass in classEntity.TeacherClasses)
                    {
                        sl.SetCellValue(row, 1, teacherClass.Teacher?.AppUser?.FullName ?? "N/A");
                        sl.SetCellValue(row, 2, teacherClass.Teacher?.Email ?? "N/A");
                        row++;
                    }
                }

                // Auto-fit columns
                sl.AutoFitColumn(1, 6);

                // Convert to byte array
                using var stream = new MemoryStream();
                sl.SaveAs(stream);
                return ServiceResult<byte[]>.Ok(stream.ToArray(), "Class data exported successfully.");
            }
            catch (Exception ex)
            {
                return ServiceResult<byte[]>.Fail($"Error exporting class data: {ex.Message}");
            }
        }

        public async Task<ServiceResult<byte[]>> ExportAllClassesAsync()
        {
            try
            {
                var classRepo = _unitOfWork.Repository<Class>();

                // Get all classes with related data
                var classes = await classRepo.FindAllAsync(
                    include: q => q.Include(c => c.Students)
                                  .Include(c => c.TeacherClasses)
                                  .ThenInclude(tc => tc.Teacher)
                                  .ThenInclude(t => t.AppUser)
                                  .Include(c => c.Grade)
                                  .Include(c => c.Exams)
                );

                if (!classes.Any())
                    return ServiceResult<byte[]>.Fail("No classes found.");

                // Create Excel document using SpreadsheetLight
                using var sl = new SLDocument();
                sl.RenameWorksheet(SLDocument.DefaultFirstSheetName, "All Classes");

                // Add header row
                string[] headers = { "Class Name", "Grade", "Student Count", "Teacher Count", "Exam Count" };
                for (int col = 0; col < headers.Length; col++)
                {
                    sl.SetCellValue(1, col + 1, headers[col]);
                }
                sl.SetCellStyle(1, 1, 1, headers.Length, CreateHeaderStyle());

                // Add data rows
                int row = 2;
                foreach (var classEntity in classes)
                {
                    sl.SetCellValue(row, 1, classEntity.ClassName);
                    sl.SetCellValue(row, 2, classEntity.Grade?.GradeName ?? "N/A");
                    sl.SetCellValue(row, 3, classEntity.Students?.Count ?? 0);
                    sl.SetCellValue(row, 4, classEntity.TeacherClasses?.Count ?? 0);
                    sl.SetCellValue(row, 5, classEntity.Exams?.Count ?? 0);
                    row++;
                }

                // Auto-fit columns
                sl.AutoFitColumn(1, headers.Length);

                // Convert to byte array
                using var stream = new MemoryStream();
                sl.SaveAs(stream);
                return ServiceResult<byte[]>.Ok(stream.ToArray(), "All classes data exported successfully.");
            }
            catch (Exception ex)
            {
                return ServiceResult<byte[]>.Fail($"Error exporting all classes data: {ex.Message}");
            }
        }

        // Helper method to create header style
        private SLStyle CreateHeaderStyle()
        {
            var style = new SLStyle();
            style.Font.Bold = true;
            style.Fill.SetPattern(PatternValues.Solid, System.Drawing.Color.LightGray, System.Drawing.Color.LightGray);
            return style;
        }
    }
}