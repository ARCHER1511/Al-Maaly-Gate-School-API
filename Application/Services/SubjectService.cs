using Application.DTOs.SubjectDTOs;
using Application.Interfaces;
using AutoMapper;
using Domain.Entities;
using Domain.Wrappers;
using Infrastructure.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Application.Services
{
    public class SubjectService : ISubjectService
    {
        private readonly ISubjectRepository _subjectRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public SubjectService(ISubjectRepository subjectRepository, IUnitOfWork unitOfWork, IMapper mapper)
        {
            _subjectRepository = subjectRepository;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<ServiceResult<SubjectViewDto>> Create(SubjectCreateDto dto)
        {
            var subjectEntity = _mapper.Map<Subject>(dto);
            await _subjectRepository.AddAsync(subjectEntity);
            var result = await _unitOfWork.SaveChangesAsync();

            if (result <= 0)
            {
                return ServiceResult<SubjectViewDto>.Fail("Failed to create subject.");
            }

            var subjectViewDto = _mapper.Map<SubjectViewDto>(subjectEntity);
            return ServiceResult<SubjectViewDto>.Ok(subjectViewDto, "Subject created successfully.");
        }

        public async Task<ServiceResult<IEnumerable<SubjectViewDto>>> GetAll()
        {
            var subjects = await _subjectRepository.GetAllAsync();
            var subjectViewDtos = _mapper.Map<IEnumerable<SubjectViewDto>>(subjects);

            if (subjectViewDtos == null || !subjectViewDtos.Any())
            {
                return ServiceResult<IEnumerable<SubjectViewDto>>.Fail("No subjects found.");
            }

            return ServiceResult<IEnumerable<SubjectViewDto>>.Ok(subjectViewDtos, "Subjects retrieved successfully.");
        }

        public async Task<ServiceResult<IEnumerable<SubjectViewDto>>> GetSubjectsByGradeIdAsync(string gradeId)
        {
            var subjects = await _subjectRepository.GetSubjectsByGradeIdAsync(gradeId);
            var data = _mapper.Map<IEnumerable<SubjectViewDto>>(subjects);
            return ServiceResult<IEnumerable<SubjectViewDto>>.Ok(data);
        }

        public async Task<ServiceResult<SubjectViewDto>> GetById(string id)
        {
            var subject = await _subjectRepository.GetByIdAsync(id);
            if (subject == null)
            {
                return ServiceResult<SubjectViewDto>.Fail("Subject not found.");
            }

            var subjectViewDto = _mapper.Map<SubjectViewDto>(subject);
            return ServiceResult<SubjectViewDto>.Ok(subjectViewDto, "Subject retrieved successfully.");
        }

        public async Task<ServiceResult<SubjectViewDto>> Update(SubjectUpdateDto dto)
        {
            var subjectEntity = await _subjectRepository.GetByIdAsync(dto.Id);
            if (subjectEntity == null)
            {
                return ServiceResult<SubjectViewDto>.Fail("Subject not found.");
            }

            _mapper.Map(dto, subjectEntity);
            _subjectRepository.Update(subjectEntity);
            var result = await _unitOfWork.SaveChangesAsync();

            if (result <= 0)
            {
                return ServiceResult<SubjectViewDto>.Fail("Failed to update subject.");
            }

            var subjectViewDto = _mapper.Map<SubjectViewDto>(subjectEntity);
            return ServiceResult<SubjectViewDto>.Ok(subjectViewDto, "Subject updated successfully.");
        }

        public async Task<ServiceResult<bool>> Delete(string id)
        {
            var subjectEntity = await _subjectRepository.GetByIdAsync(id);
            if (subjectEntity == null)
            {
                return ServiceResult<bool>.Fail("Subject not found.");
            }

            _subjectRepository.Delete(subjectEntity);
            var result = await _unitOfWork.SaveChangesAsync();

            if (result <= 0)
            {
                return ServiceResult<bool>.Fail("Failed to delete subject.");
            }

            return ServiceResult<bool>.Ok(true, "Subject deleted successfully.");
        }

        public async Task<ServiceResult<int>> GetSubjectCountAsync()
        {
            try
            {
                var allSubjects = await _subjectRepository.GetAllAsync();
                var count = allSubjects.Count();
                return ServiceResult<int>.Ok(count, $"Total subjects: {count}");
            }
            catch (Exception ex)
            {
                return ServiceResult<int>.Fail($"Error counting subjects: {ex.Message}");
            }
        }
    }
}