using Application.DTOs.CurriculumDTOs;
using Application.Interfaces;
using AutoMapper;
using Domain.Entities;
using Domain.Wrappers;
using Infrastructure.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Application.Services
{
    public class CurriculumService : ICurriculumService
    {
        private readonly ICurriculumRepository _curriculumRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public CurriculumService(ICurriculumRepository curriculumRepository, IUnitOfWork unitOfWork, IMapper mapper)
        {
            _curriculumRepository = curriculumRepository;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<ServiceResult<CurriculumDto>> GetByIdAsync(string id)
        {
            var curriculum = await _curriculumRepository.GetByIdAsync(id);
            if (curriculum == null)
                return ServiceResult<CurriculumDto>.Fail($"Curriculum with ID '{id}' not found.");

            var dto = _mapper.Map<CurriculumDto>(curriculum);
            return ServiceResult<CurriculumDto>.Ok(dto, "Curriculum retrieved successfully");
        }

        public async Task<ServiceResult<IEnumerable<CurriculumDto>>> GetAllAsync()
        {
            var curricula = await _curriculumRepository.GetAllAsync();
            var dtos = _mapper.Map<IEnumerable<CurriculumDto>>(curricula);
            return ServiceResult<IEnumerable<CurriculumDto>>.Ok(dtos, "Curricula retrieved successfully");
        }

        public async Task<ServiceResult<CurriculumDto>> CreateAsync(CreateCurriculumDto dto)
        {
            try
            {
                // Check if curriculum with same name already exists
                var exists = await _curriculumRepository.ExistsByNameAsync(dto.Name);
                if (exists)
                    return ServiceResult<CurriculumDto>.Fail($"Curriculum with name '{dto.Name}' already exists.");

                var curriculum = _mapper.Map<Curriculum>(dto);
                curriculum.Id = Guid.NewGuid().ToString(); // Generate new ID
                curriculum.CreatedAt = DateTime.Now;

                await _curriculumRepository.AddAsync(curriculum);
                await _unitOfWork.SaveChangesAsync();

                var resultDto = _mapper.Map<CurriculumDto>(curriculum);
                return ServiceResult<CurriculumDto>.Ok(resultDto, "Curriculum created successfully");
            }
            catch (Exception ex)
            {
                return ServiceResult<CurriculumDto>.Fail($"Error creating curriculum: {ex.Message}");
            }
        }

        public async Task<ServiceResult<CurriculumDto>> UpdateAsync(string id, UpdateCurriculumDto dto)
        {
            try
            {
                var curriculum = await _curriculumRepository.GetByIdAsync(id);
                if (curriculum == null)
                    return ServiceResult<CurriculumDto>.Fail($"Curriculum with ID '{id}' not found.");

                // Check if new name conflicts with existing curriculum
                if (curriculum.Name != dto.Name)
                {
                    var exists = await _curriculumRepository.ExistsByNameAsync(dto.Name);
                    if (exists)
                        return ServiceResult<CurriculumDto>.Fail($"Curriculum with name '{dto.Name}' already exists.");
                }

                _mapper.Map(dto, curriculum);
                curriculum.UpdatedAt = DateTime.Now;

                _curriculumRepository.Update(curriculum);
                await _unitOfWork.SaveChangesAsync();

                var resultDto = _mapper.Map<CurriculumDto>(curriculum);
                return ServiceResult<CurriculumDto>.Ok(resultDto, "Curriculum updated successfully");
            }
            catch (Exception ex)
            {
                return ServiceResult<CurriculumDto>.Fail($"Error updating curriculum: {ex.Message}");
            }
        }

        public async Task<ServiceResult<bool>> DeleteAsync(string id)
        {
            try
            {
                var curriculum = await _curriculumRepository.GetByIdAsync(id);
                if (curriculum == null)
                    return ServiceResult<bool>.Fail($"Curriculum with ID '{id}' not found.");

                // Check if curriculum has students or teachers
                if (curriculum.Students?.Any() == true)
                    return ServiceResult<bool>.Fail("Cannot delete curriculum that has students assigned.");

                if (curriculum.Teachers?.Any() == true)
                    return ServiceResult<bool>.Fail("Cannot delete curriculum that has teachers specialized.");

                _curriculumRepository.Delete(curriculum);
                await _unitOfWork.SaveChangesAsync();
                return ServiceResult<bool>.Ok(true, "Curriculum deleted successfully");
            }
            catch (Exception ex)
            {
                return ServiceResult<bool>.Fail($"Error deleting curriculum: {ex.Message}");
            }
        }

        public async Task<ServiceResult<bool>> ExistsAsync(string id)
        {
            var curriculum = await _curriculumRepository.GetByIdAsync(id);
            return ServiceResult<bool>.Ok(curriculum != null,
                curriculum != null ? "Curriculum exists" : "Curriculum does not exist");
        }

        public async Task<ServiceResult<CurriculumDetailsDto>> GetWithDetailsAsync(string id)
        {
            var curriculum = await _curriculumRepository.GetWithDetailsAsync(id);
            if (curriculum == null)
                return ServiceResult<CurriculumDetailsDto>.Fail($"Curriculum with ID '{id}' not found.");

            var detailsDto = _mapper.Map<CurriculumDetailsDto>(curriculum);
            return ServiceResult<CurriculumDetailsDto>.Ok(detailsDto, "Curriculum details retrieved successfully");
        }

        public async Task<ServiceResult<bool>> HasStudentsAsync(string curriculumId)
        {
            var curriculum = await _curriculumRepository.GetByIdAsync(curriculumId);
            if (curriculum == null)
                return ServiceResult<bool>.Fail($"Curriculum with ID '{curriculumId}' not found.");

            return ServiceResult<bool>.Ok(
                curriculum.Students?.Any() == true,
                curriculum.Students?.Any() == true ? "Curriculum has students" : "Curriculum has no students"
            );
        }

        public async Task<ServiceResult<bool>> HasTeachersAsync(string curriculumId)
        {
            var curriculum = await _curriculumRepository.GetByIdAsync(curriculumId);
            if (curriculum == null)
                return ServiceResult<bool>.Fail($"Curriculum with ID '{curriculumId}' not found.");

            return ServiceResult<bool>.Ok(
                curriculum.Teachers?.Any() == true,
                curriculum.Teachers?.Any() == true ? "Curriculum has teachers" : "Curriculum has no teachers"
            );
        }

        public async Task<ServiceResult<int>> GetCountAsync()
        {
            try
            {
                var allCurricula = await _curriculumRepository.GetAllAsync();
                var count = allCurricula.Count();
                return ServiceResult<int>.Ok(count, $"Total curricula: {count}");
            }
            catch (Exception ex)
            {
                return ServiceResult<int>.Fail($"Error counting curricula: {ex.Message}");
            }
        }
    }
}