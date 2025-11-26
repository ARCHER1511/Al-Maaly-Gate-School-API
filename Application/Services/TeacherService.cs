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
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public TeacherService(ITeacherRepository teacherRepo, IUnitOfWork unitOfWork, IMapper mapper)
        {
            _teacherRepo = teacherRepo;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<ServiceResult<IEnumerable<TeacherViewDto>>> GetAllAsync()
        {
            var teachers = await _teacherRepo.FindAllAsync(
                predicate: null,
                include: q => q
                    .Include(t => t.AppUser) // Include AppUser
                    .Include(t => t.TeacherSubjects!)
                        .ThenInclude(ts => ts.Subject)
                    .Include(t => t.TeacherClasses)
                        .ThenInclude(tc => tc.Class)
            );

            var data = teachers.Select(t => new TeacherViewDto
            {
                Id = t.Id,
                FullName = t.AppUser?.FullName ?? "[No Name]",
                Email = t.Email ?? string.Empty,
                ContactInfo = t.ContactInfo ?? string.Empty,
                AppUserId = t.AppUserId ?? string.Empty,
                ProfileStatus = t.ProfileStatus,
                Subjects = t.TeacherSubjects?.Select(ts => ts.Subject?.SubjectName ?? "[Unknown]").ToList()
                           ?? new List<string>(),
                ClassNames = t.TeacherClasses?.Select(tc => tc.Class?.ClassName ?? "[Unknown]").ToList()
                             ?? new List<string>()
            });

            return ServiceResult<IEnumerable<TeacherViewDto>>.Ok(data);
        }

        public async Task<ServiceResult<TeacherViewDto>> GetByIdAsync(string id)
        {
            var teacher = await _teacherRepo.FirstOrDefaultAsync(
                t => t.Id == id,
                include: q => q
                    .Include(t => t.AppUser) // Include AppUser
                    .Include(t => t.TeacherSubjects!)
                        .ThenInclude(ts => ts.Subject)
                    .Include(t => t.TeacherClasses)
                        .ThenInclude(tc => tc.Class)
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
                ProfileStatus = teacher.ProfileStatus,
                Subjects = teacher.TeacherSubjects?.Select(ts => ts.Subject?.SubjectName ?? "[Unknown]").ToList()
                    ?? new List<string>(),
                ClassNames = teacher.TeacherClasses?.Select(tc => tc.Class?.ClassName ?? "[Unknown]").ToList()
                    ?? new List<string>()
            };

            return ServiceResult<TeacherViewDto>.Ok(dto);
        }



        public async Task<ServiceResult<TeacherViewDto>> CreateAsync(CreateTeacherDto dto)
        {
            var teacher = _mapper.Map<Teacher>(dto);
            await _teacherRepo.AddAsync(teacher);
            await _unitOfWork.SaveChangesAsync();
            return ServiceResult<TeacherViewDto>.Ok(_mapper.Map<TeacherViewDto>(teacher), "Teacher created successfully");
        }

        public async Task<ServiceResult<TeacherViewDto>> UpdateAsync(string id, UpdateTeacherDto dto)
        {
            var teacher = await _teacherRepo.GetByIdAsync(id);
            if (teacher == null)
                return ServiceResult<TeacherViewDto>.Fail("Teacher not found");

            _mapper.Map(dto, teacher);
            _teacherRepo.Update(teacher);
            await _unitOfWork.SaveChangesAsync();

            return ServiceResult<TeacherViewDto>.Ok(_mapper.Map<TeacherViewDto>(teacher), "Teacher updated successfully");
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
    }
}
