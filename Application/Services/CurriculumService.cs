using Application.DTOs.CurriculumDTOs;
using Application.DTOs.GradeDTOs;
using Application.DTOs.StudentDTOs;
using Application.DTOs.TeacherDTOs;
using Application.Interfaces;
using Domain.Entities;
using Infrastructure.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services
{
    public class CurriculumService : ICurriculumService
    {
        private readonly ICurriculumRepository _curriculumRepository;
        private readonly IUnitOfWork _unitOfWork;

        public CurriculumService(ICurriculumRepository curriculumRepository, IUnitOfWork unitOfWork)
        {
            _curriculumRepository = curriculumRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<CurriculumDto?> GetByIdAsync(string id)
        {
            var curriculum = await _curriculumRepository.GetByIdAsync(id);
            if (curriculum == null) return null;

            return MapToDto(curriculum);
        }

        public async Task<IEnumerable<CurriculumDto>> GetAllAsync()
        {
            var curricula = await _curriculumRepository.GetAllAsync();
            return curricula.Select(MapToDto);
        }

        public async Task<CurriculumDto> CreateAsync(CreateCurriculumDto dto)
        {
            // Check if curriculum with same name already exists
            var exists = await _curriculumRepository.ExistsByNameAsync(dto.Name);
            if (exists)
                throw new InvalidOperationException($"Curriculum with name '{dto.Name}' already exists.");

            var curriculum = new Curriculum
            {
                Name = dto.Name,
                Code = dto.Code,
                Description = dto.Description,
                CreatedAt = DateTime.UtcNow
            };

            await _curriculumRepository.AddAsync(curriculum);
            await _unitOfWork.SaveChangesAsync();

            return MapToDto(curriculum);
        }

        public async Task<CurriculumDto?> UpdateAsync(string id, UpdateCurriculumDto dto)
        {
            var curriculum = await _curriculumRepository.GetByIdAsync(id);
            if (curriculum == null) return null;

            // Check if new name conflicts with existing curriculum
            if (curriculum.Name != dto.Name)
            {
                var exists = await _curriculumRepository.ExistsByNameAsync(dto.Name);
                if (exists)
                    throw new InvalidOperationException($"Curriculum with name '{dto.Name}' already exists.");
            }

            curriculum.Name = dto.Name;
            curriculum.Code = dto.Code;
            curriculum.Description = dto.Description;
            curriculum.UpdatedAt = DateTime.UtcNow;

            _curriculumRepository.Update(curriculum);
            await _unitOfWork.SaveChangesAsync();

            return MapToDto(curriculum);
        }

        public async Task<bool> DeleteAsync(string id)
        {
            var curriculum = await _curriculumRepository.GetByIdAsync(id);
            if (curriculum == null) return false;

            // Check if curriculum has students or teachers
            if (curriculum.Students?.Any() == true)
                throw new InvalidOperationException("Cannot delete curriculum that has students assigned.");

            if (curriculum.Teachers?.Any() == true)
                throw new InvalidOperationException("Cannot delete curriculum that has teachers specialized.");

            _curriculumRepository.Delete(curriculum);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ExistsAsync(string id)
        {
            var curriculum = await _curriculumRepository.GetByIdAsync(id);
            return curriculum != null;
        }

        public async Task<CurriculumDetailsDto?> GetWithDetailsAsync(string id)
        {
            var curriculum = await _curriculumRepository.GetWithDetailsAsync(id);
            if (curriculum == null) return null;

            return new CurriculumDetailsDto
            {
                Id = curriculum.Id,
                Name = curriculum.Name,
                Code = curriculum.Code,
                Description = curriculum.Description,
                GradeCount = curriculum.Grades?.Count ?? 0,
                StudentCount = curriculum.Students?.Count ?? 0,
                TeacherCount = curriculum.Teachers?.Count ?? 0,
                CreatedAt = curriculum.CreatedAt,
                UpdatedAt = curriculum.UpdatedAt,
                Grades = curriculum.Grades?.Select(g => new GradeViewDto
                {
                    Id = g.Id,
                    GradeName = g.GradeName,
                    ClassCount = g.Classes?.Count ?? 0,
                    SubjectCount = g.Subjects?.Count ?? 0
                }).ToList() ?? new(),
                Students = curriculum.Students?.Select(s => new StudentViewDto
                {
                    Id = s.Id,
                    FullName = s.FullName, // Use the existing FullName property
                    Email = s.Email,
                    ClassName = s.Class?.ClassName ?? "Not Assigned",
                    Age = s.Age,
                    ClassId = s.ClassId,
                    ContactInfo = s.ContactInfo,
                    GradeName = s.Class?.Grade?.GradeName ?? "Not Assigned",
                    AccountStatus = s.AccountStatus.ToString(),
                }).ToList() ?? new(),
                Teachers = curriculum.Teachers?.Select(t => new TeacherViewDto
                {
                    Id = t.Id,
                    FullName = t.FullName, // Use the existing FullName property
                    Email = t.Email
                }).ToList() ?? new()
            };
        }

        public async Task<bool> HasStudentsAsync(string curriculumId)
        {
            var curriculum = await _curriculumRepository.GetByIdAsync(curriculumId);
            return curriculum?.Students?.Any() == true;
        }

        public async Task<bool> HasTeachersAsync(string curriculumId)
        {
            var curriculum = await _curriculumRepository.GetByIdAsync(curriculumId);
            return curriculum?.Teachers?.Any() == true;
        }

        private CurriculumDto MapToDto(Curriculum curriculum)
        {
            return new CurriculumDto
            {
                Id = curriculum.Id,
                Name = curriculum.Name,
                Code = curriculum.Code,
                Description = curriculum.Description,
                GradeCount = curriculum.Grades?.Count ?? 0,
                StudentCount = curriculum.Students?.Count ?? 0,
                TeacherCount = curriculum.Teachers?.Count ?? 0,
                CreatedAt = curriculum.CreatedAt,
                UpdatedAt = curriculum.UpdatedAt
            };
        }
    }
}
