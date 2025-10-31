using Application.DTOs.ClassDTOs;
using Application.Interfaces;
using AutoMapper;
using Domain.Entities;
using Domain.Wrappers;
using Infrastructure.Interfaces;
using Microsoft.EntityFrameworkCore;

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
            var result = await _classRepository.GetAllAsync();
            if (result == null) return ServiceResult<IEnumerable<ClassViewDto>>.Fail("class not found");

            var resultDto = _mapper.Map<IEnumerable<ClassViewDto>>(result);
            return ServiceResult<IEnumerable<ClassViewDto>>.Ok(resultDto, "class retrieved successfully");
        }
        public async Task<ServiceResult<ClassViewDto>> GetByIdAsync(object id)
        {
            var result = await _classRepository.GetByIdAsync(id);
            if (result == null) return ServiceResult<ClassViewDto>.Fail("class not found");

            var resultDto = _mapper.Map<ClassViewDto>(result);
            return ServiceResult<ClassViewDto>.Ok(resultDto, "class retrieved successfully");
        }
        public async Task<ServiceResult<ClassDto>> CreateAsync(ClassDto dto)
        {
            var result = _mapper.Map<Class>(dto);

            await _classRepository.AddAsync(result);
            await _unitOfWork.SaveChangesAsync();

            var viewDto = _mapper.Map<ClassDto>(result);
            return ServiceResult<ClassDto>.Ok(viewDto, "class created successfully");
        }
        public async Task<ServiceResult<ClassDto>> UpdateAsync(ClassDto dto)
        {
            var existingresult = await _classRepository.GetByIdAsync(dto.Id);
            if (existingresult == null)
                return ServiceResult<ClassDto>.Fail("class not found");

            _mapper.Map(dto, existingresult);

            _classRepository.Update(existingresult);
            await _unitOfWork.SaveChangesAsync();

            var viewDto = _mapper.Map<ClassDto>(existingresult);
            return ServiceResult<ClassDto>.Ok(viewDto, "class updated successfully");
        }
        public async Task<ServiceResult<bool>> DeleteAsync(object id)
        {
            var result = await _classRepository.GetByIdAsync(id);
            if (result == null)
                return ServiceResult<bool>.Fail("class not found");

            _classRepository.Delete(result);
            await _unitOfWork.SaveChangesAsync();
            return ServiceResult<bool>.Ok(true, "class deleted successfully");
        }
    }
}
