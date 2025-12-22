using Application.DTOs.ClassDTOs;
using Application.DTOs.CurriculumDTOs;
using Application.DTOs.SubjectDTOs;
using Application.DTOs.TeacherDTOs;
using Application.Interfaces;
using AutoMapper;
using Domain.Entities;
using Domain.Wrappers;
using Infrastructure.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Application.Services
{
    public class TeacherService : ITeacherService
    {
        private readonly ITeacherRepository _teacherRepo;
        private readonly ISubjectRepository _subjectRepo;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ICurriculumRepository _curriculumRepository; // Add this

        public TeacherService(
            ITeacherRepository teacherRepo,
            ISubjectRepository subjectRepo,
            IUnitOfWork unitOfWork,
            IMapper mapper,
            ICurriculumRepository curriculumRepository
        ) // Add this
        {
            _teacherRepo = teacherRepo;
            _subjectRepo = subjectRepo;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _curriculumRepository = curriculumRepository;
        }

        public async Task<ServiceResult<IEnumerable<TeacherViewDto>>> GetAllAsync()
        {
            var teachers = await _teacherRepo.FindAllAsync(
                predicate: null,
                include: q =>
                    q.Include(t => t.AppUser)
                        .Include(t => t.TeacherSubjects!)
                        .ThenInclude(ts => ts.Subject)
                        .Include(t => t.TeacherClasses)
                        .ThenInclude(tc => tc.Class)
                        .Include(t => t.SpecializedCurricula) // Add this
            );

            var data = teachers.Select(t => new TeacherViewDto
            {
                Id = t.Id,
                FullName = t.AppUser?.FullName ?? "[No Name]",
                Email = t.Email ?? string.Empty,
                ContactInfo = t.ContactInfo ?? string.Empty,
                AppUserId = t.AppUserId ?? string.Empty,
                AccountStatus = t.AccountStatus,
                Subjects =
                    t.TeacherSubjects?.Select(ts => ts.Subject?.SubjectName ?? "[Unknown]").ToList()
                    ?? new List<string>(),
                ClassNames =
                    t.TeacherClasses?.Select(tc => tc.Class?.ClassName ?? "[Unknown]").ToList()
                    ?? new List<string>(),
                SpecializedCurricula =
                    t.SpecializedCurricula?.Select(c => c.Name).ToList() // Add this
                    ?? new List<string>(),
                SpecializedCurriculumIds =
                    t.SpecializedCurricula?.Select(c => c.Id).ToList() // Add this
                    ?? new List<string>(),
            });

            return ServiceResult<IEnumerable<TeacherViewDto>>.Ok(data);
        }

        public async Task<ServiceResult<TeacherViewDto>> GetByIdAsync(string id)
        {
            var teacher = await _teacherRepo.FirstOrDefaultAsync(
                t => t.Id == id,
                include: q =>
                    q.Include(t => t.AppUser)
                        .Include(t => t.TeacherSubjects!)
                        .ThenInclude(ts => ts.Subject)
                        .Include(t => t.TeacherClasses)
                        .ThenInclude(tc => tc.Class)
                        .Include(t => t.SpecializedCurricula) // Add this
            );

            if (teacher == null)
                return ServiceResult<TeacherViewDto>.Fail("Teacher not found");

            var dto = new TeacherViewDto
            {
                Id = teacher.Id,
                FullName = teacher.AppUser?.FullName ?? "[No Name]",
                Email = teacher.Email ?? string.Empty,
                ContactInfo = teacher.ContactInfo ?? string.Empty,
                AppUserId = teacher.AppUserId ?? string.Empty,
                AccountStatus = teacher.AccountStatus,
                Subjects =
                    teacher
                        .TeacherSubjects?.Select(ts => ts.Subject?.SubjectName ?? "[Unknown]")
                        .ToList() ?? new List<string>(),
                ClassNames =
                    teacher
                        .TeacherClasses?.Select(tc => tc.Class?.ClassName ?? "[Unknown]")
                        .ToList() ?? new List<string>(),
                SpecializedCurricula =
                    teacher.SpecializedCurricula?.Select(c => c.Name).ToList() // Add this
                    ?? new List<string>(),
                SpecializedCurriculumIds =
                    teacher.SpecializedCurricula?.Select(c => c.Id).ToList() // Add this
                    ?? new List<string>(),
            };

            return ServiceResult<TeacherViewDto>.Ok(dto);
        }

        public async Task<ServiceResult<TeacherViewDto>> CreateAsync(CreateTeacherDto dto)
        {
            var teacher = _mapper.Map<Teacher>(dto);

            // Handle specialized curricula
            if (dto.SpecializedCurriculumIds != null && dto.SpecializedCurriculumIds.Any())
            {
                var curricula = await _curriculumRepository.FindAllAsync(c =>
                    dto.SpecializedCurriculumIds.Contains(c.Id)
                );

                // Clear and add new specialized curricula
                teacher.SpecializedCurricula = curricula.ToList();
            }

            await _teacherRepo.AddAsync(teacher);
            await _unitOfWork.SaveChangesAsync();

            // Reload with details
            var createdTeacher = await _teacherRepo.FirstOrDefaultAsync(
                t => t.Id == teacher.Id,
                include: q => q.Include(t => t.SpecializedCurricula)
            );

            return ServiceResult<TeacherViewDto>.Ok(
                _mapper.Map<TeacherViewDto>(createdTeacher ?? teacher),
                "Teacher created successfully"
            );
        }

        public async Task<ServiceResult<TeacherViewDto>> UpdateAsync(
            string id,
            UpdateTeacherDto dto
        )
        {
            var teacher = await _teacherRepo.FirstOrDefaultAsync(
                t => t.Id == id,
                include: q => q.Include(t => t.SpecializedCurricula)
            );

            if (teacher == null)
                return ServiceResult<TeacherViewDto>.Fail("Teacher not found");

            _mapper.Map(dto, teacher);

            // Handle specialized curricula updates
            if (dto.SpecializedCurriculumIds != null)
            {
                var curricula = await _curriculumRepository.FindAllAsync(c =>
                    dto.SpecializedCurriculumIds.Contains(c.Id)
                );

                teacher.SpecializedCurricula = curricula.ToList();
            }

            _teacherRepo.Update(teacher);
            await _unitOfWork.SaveChangesAsync();

            // Reload with details
            var updatedTeacher = await _teacherRepo.FirstOrDefaultAsync(
                t => t.Id == id,
                include: q => q.Include(t => t.SpecializedCurricula)
            );

            return ServiceResult<TeacherViewDto>.Ok(
                _mapper.Map<TeacherViewDto>(updatedTeacher ?? teacher),
                "Teacher updated successfully"
            );
        }

        public async Task<ServiceResult<bool>> DeleteAsync(string id)
        {
            var teacher = await _teacherRepo.GetByIdAsync(id);
            if (teacher == null)
                return ServiceResult<bool>.Fail("Teacher not found");

            _teacherRepo.Delete(teacher);
            await _unitOfWork.SaveChangesAsync();
            return ServiceResult<bool>.Ok(true, "Teacher deleted successfully");
        }

        // New method: Get teachers by curriculum
        public async Task<ServiceResult<IEnumerable<TeacherViewDto>>> GetTeachersByCurriculumAsync(
            string curriculumId
        )
        {
            var teachers = await _teacherRepo.FindAllAsync(
                predicate: t => t.SpecializedCurricula.Any(c => c.Id == curriculumId),
                include: q =>
                    q.Include(t => t.AppUser)
                        .Include(t => t.TeacherSubjects!)
                        .ThenInclude(ts => ts.Subject)
                        .Include(t => t.TeacherClasses)
                        .ThenInclude(tc => tc.Class)
                        .Include(t => t.SpecializedCurricula)
            );

            var data = teachers.Select(t => new TeacherViewDto
            {
                Id = t.Id,
                FullName = t.AppUser?.FullName ?? "[No Name]",
                Email = t.Email ?? string.Empty,
                ContactInfo = t.ContactInfo ?? string.Empty,
                AppUserId = t.AppUserId ?? string.Empty,
                AccountStatus = t.AccountStatus,
                Subjects =
                    t.TeacherSubjects?.Select(ts => ts.Subject?.SubjectName ?? "[Unknown]").ToList()
                    ?? new List<string>(),
                ClassNames =
                    t.TeacherClasses?.Select(tc => tc.Class?.ClassName ?? "[Unknown]").ToList()
                    ?? new List<string>(),
                SpecializedCurricula =
                    t.SpecializedCurricula?.Select(c => c.Name).ToList() ?? new List<string>(),
                SpecializedCurriculumIds =
                    t.SpecializedCurricula?.Select(c => c.Id).ToList() ?? new List<string>(),
            });

            return ServiceResult<IEnumerable<TeacherViewDto>>.Ok(
                data,
                $"Found {data.Count()} teachers specialized in this curriculum"
            );
        }

        // New method: Add teacher to curriculum
        public async Task<ServiceResult<TeacherViewDto>> AddTeacherToCurriculumAsync(
            string teacherId,
            string curriculumId
        )
        {
            var teacher = await _teacherRepo.FirstOrDefaultAsync(
                t => t.Id == teacherId,
                include: q => q.Include(t => t.SpecializedCurricula)
            );

            if (teacher == null)
                return ServiceResult<TeacherViewDto>.Fail("Teacher not found");

            var curriculum = await _curriculumRepository.GetByIdAsync(curriculumId);
            if (curriculum == null)
                return ServiceResult<TeacherViewDto>.Fail("Curriculum not found");

            // Check if already specialized in this curriculum
            if (teacher.SpecializedCurricula.Any(c => c.Id == curriculumId))
                return ServiceResult<TeacherViewDto>.Fail(
                    "Teacher already specialized in this curriculum"
                );

            teacher.SpecializedCurricula.Add(curriculum);
            _teacherRepo.Update(teacher);
            await _unitOfWork.SaveChangesAsync();

            var updatedTeacher = await _teacherRepo.FirstOrDefaultAsync(
                t => t.Id == teacherId,
                include: q => q.Include(t => t.SpecializedCurricula)
            );

            return ServiceResult<TeacherViewDto>.Ok(
                _mapper.Map<TeacherViewDto>(updatedTeacher ?? teacher),
                "Teacher added to curriculum successfully"
            );
        }

        // New method: Remove teacher from curriculum
        public async Task<ServiceResult<TeacherViewDto>> RemoveTeacherFromCurriculumAsync(
            string teacherId,
            string curriculumId
        )
        {
            var teacher = await _teacherRepo.FirstOrDefaultAsync(
                t => t.Id == teacherId,
                include: q => q.Include(t => t.SpecializedCurricula)
            );

            if (teacher == null)
                return ServiceResult<TeacherViewDto>.Fail("Teacher not found");

            var curriculum = teacher.SpecializedCurricula.FirstOrDefault(c => c.Id == curriculumId);
            if (curriculum == null)
                return ServiceResult<TeacherViewDto>.Fail(
                    "Teacher is not specialized in this curriculum"
                );

            teacher.SpecializedCurricula.Remove(curriculum);
            _teacherRepo.Update(teacher);
            await _unitOfWork.SaveChangesAsync();

            var updatedTeacher = await _teacherRepo.FirstOrDefaultAsync(
                t => t.Id == teacherId,
                include: q => q.Include(t => t.SpecializedCurricula)
            );

            return ServiceResult<TeacherViewDto>.Ok(
                _mapper.Map<TeacherViewDto>(updatedTeacher ?? teacher),
                "Teacher removed from curriculum successfully"
            );
        }

        // New method: Get teacher count by curriculum
        public async Task<ServiceResult<int>> GetTeacherCountByCurriculumAsync(string curriculumId)
        {
            var count = await _teacherRepo.GetCountAsync(t =>
                t.SpecializedCurricula.Any(c => c.Id == curriculumId)
            );

            return ServiceResult<int>.Ok(
                count,
                $"Total teachers specialized in curriculum: {count}"
            );
        }

        // New method: Get detailed teacher view
        public async Task<ServiceResult<TeacherDetailsDto>> GetTeacherDetailsAsync(string id)
        {
            var teacher = await _teacherRepo.FirstOrDefaultAsync(
                t => t.Id == id,
                include: q =>
                    q.Include(t => t.AppUser)
                        .Include(t => t.TeacherSubjects!)
                        .ThenInclude(ts => ts.Subject)
                        .Include(t => t.TeacherClasses)
                        .ThenInclude(tc => tc.Class)
                        .Include(t => t.SpecializedCurricula)
            );

            if (teacher == null)
                return ServiceResult<TeacherDetailsDto>.Fail("Teacher not found");

            var dto = new TeacherDetailsDto
            {
                Id = teacher.Id,
                FullName = teacher.AppUser?.FullName ?? "[No Name]",
                Email = teacher.Email ?? string.Empty,
                ContactInfo = teacher.ContactInfo ?? string.Empty,
                AppUserId = teacher.AppUserId ?? string.Empty,
                AccountStatus = teacher.AccountStatus,
                Subjects =
                    teacher
                        .TeacherSubjects?.Select(ts => ts.Subject?.SubjectName ?? "[Unknown]")
                        .ToList() ?? new List<string>(),
                ClassNames =
                    teacher
                        .TeacherClasses?.Select(tc => tc.Class?.ClassName ?? "[Unknown]")
                        .ToList() ?? new List<string>(),
                SpecializedCurricula =
                    teacher
                        .SpecializedCurricula?.Select(c => new CurriculumDto
                        {
                            Id = c.Id,
                            Name = c.Name,
                            Code = c.Code,
                            Description = c.Description,
                        })
                        .ToList() ?? new List<CurriculumDto>(),
                AssignedClasses =
                    teacher
                        .TeacherClasses?.Select(tc => new ClassDto
                        {
                            Id = tc.Class?.Id ?? string.Empty,
                            ClassName = tc.Class?.ClassName ?? "[Unknown]",
                            GradeId = tc.Class?.GradeId ?? string.Empty,
                        })
                        .ToList() ?? new List<ClassDto>(),
                AssignedSubjects =
                    teacher
                        .TeacherSubjects?.Select(ts => new SubjectViewDto
                        {
                            Id = ts.Subject?.Id ?? string.Empty,
                            SubjectName = ts.Subject?.SubjectName ?? "[Unknown]",
                            GradeId = ts.Subject?.GradeId ?? string.Empty,
                        })
                        .ToList() ?? new List<SubjectViewDto>(),
            };

            return ServiceResult<TeacherDetailsDto>.Ok(dto);
        }

        public async Task<
            ServiceResult<IEnumerable<TeacherViewDto>>
        > GetTeachersNotAssignedToThisSubject(string subjectId)
        {
            if (string.IsNullOrWhiteSpace(subjectId))
                return ServiceResult<IEnumerable<TeacherViewDto>>.Fail("SubjectId is required");
            var subjectExists = await _subjectRepo.AnyAsync(s => s.Id == subjectId);

            if (!subjectExists)
                return ServiceResult<IEnumerable<TeacherViewDto>>.Fail("Subject not found");

            var teachers = await _teacherRepo.FindAllAsync(t =>
                t.TeacherSubjects == null || !t.TeacherSubjects.Any(ts => ts.SubjectId == subjectId)
            );

            var mapped = _mapper.Map<IEnumerable<TeacherViewDto>>(teachers);
            return ServiceResult<IEnumerable<TeacherViewDto>>.Ok(
                mapped,
                mapped.Any()
                    ? $"Teachers not assigned to subject {subjectId} loaded"
                    : "No unassigned teachers found"
            );
        }
        public async Task<ServiceResult<IEnumerable<TeacherViewDto>>> GetTeachersAssignedToThisSubject(string subjectId)
        {
            if (string.IsNullOrWhiteSpace(subjectId))
                return ServiceResult<IEnumerable<TeacherViewDto>>.Fail("SubjectId is required");
            var subjectExists = await _subjectRepo.AnyAsync(s => s.Id == subjectId);

            if (!subjectExists)
                return ServiceResult<IEnumerable<TeacherViewDto>>.Fail("Subject not found");

            var teachers = await _teacherRepo.FindAllAsync(t =>
                t.TeacherSubjects == null || t.TeacherSubjects.Any(ts => ts.SubjectId == subjectId)
            );

            var mapped = _mapper.Map<IEnumerable<TeacherViewDto>>(teachers);
            return ServiceResult<IEnumerable<TeacherViewDto>>.Ok(
                mapped,
                mapped.Any()
                    ? $"Teachers not assigned to subject {subjectId} loaded"
                    : "No unassigned teachers found"
            );
        }
    }
}
