using Application.DTOs.StudentExamAnswerDTOs;
using Application.Interfaces;
using AutoMapper;
using Domain.Entities;
using Domain.Wrappers;
using Infrastructure.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Application.Services
{
    public class StudentExamResultService : IStudentExamResultService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public StudentExamResultService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<
            ServiceResult<IEnumerable<StudentExamResultDto>>
        > GetAllResultsByStudentIdAsync(object Id)
        {
            var result = await _unitOfWork
                .StudentExamResultRepository.AsQueryable()
                .Where(s => s.StudentId == (string)Id)
                .ToListAsync();

            var resultDto = _mapper.Map<IEnumerable<StudentExamResultDto>>(
                result ?? new List<StudentExamResult>()
            );

            var message =
                result?.Any() == true
                    ? $"found {result.Count} results"
                    : "there is no results for that student";

            return ServiceResult<IEnumerable<StudentExamResultDto>>.Ok(resultDto, message);
        }

        public async Task<ServiceResult<IEnumerable<StudentExamResultDto>>> GetAllAsync()
        {
            var result = await _unitOfWork.StudentExamResultRepository.GetAllAsync();
            if (result == null)
                return ServiceResult<IEnumerable<StudentExamResultDto>>.Fail(
                    "Student Exams Results not found"
                );

            var resultDto = _mapper.Map<IEnumerable<StudentExamResultDto>>(result);
            return ServiceResult<IEnumerable<StudentExamResultDto>>.Ok(
                resultDto,
                "Student Exams Results retrieved successfully"
            );
        }

        public async Task<ServiceResult<StudentExamResultDto>> GetByIdAsync(object id)
        {
            var result = await _unitOfWork.StudentExamResultRepository.GetByIdAsync(id);
            if (result == null)
                return ServiceResult<StudentExamResultDto>.Fail("Student Exam Result not found");

            var resultDto = _mapper.Map<StudentExamResultDto>(result);
            return ServiceResult<StudentExamResultDto>.Ok(
                resultDto,
                "Student Exam Result retrieved successfully"
            );
        }

        public async Task<ServiceResult<StudentExamResultDto>> CreateAsync(StudentExamResultDto dto)
        {
            var result = _mapper.Map<StudentExamResult>(dto);

            await _unitOfWork.StudentExamResultRepository.AddAsync(result);
            await _unitOfWork.SaveChangesAsync();

            var viewDto = _mapper.Map<StudentExamResultDto>(result);
            return ServiceResult<StudentExamResultDto>.Ok(
                viewDto,
                "Student Exam Result created successfully"
            );
        }

        public async Task<ServiceResult<StudentExamResultDto>> UpdateAsync(StudentExamResultDto dto)
        {
            var existingresult = await _unitOfWork.StudentExamResultRepository.GetByIdAsync(dto.Id);
            if (existingresult == null)
                return ServiceResult<StudentExamResultDto>.Fail("Student Exam Result not found");

            _mapper.Map(dto, existingresult);

            _unitOfWork.StudentExamResultRepository.Update(existingresult);
            await _unitOfWork.SaveChangesAsync();

            var viewDto = _mapper.Map<StudentExamResultDto>(existingresult);
            return ServiceResult<StudentExamResultDto>.Ok(
                viewDto,
                "Student Exam Result updated successfully"
            );
        }

        public async Task<ServiceResult<bool>> DeleteAsync(object id)
        {
            var result = await _unitOfWork.StudentExamResultRepository.GetByIdAsync(id);
            if (result == null)
                return ServiceResult<bool>.Fail("Student Exam Result not found");

            _unitOfWork.StudentExamResultRepository.Delete(result);
            await _unitOfWork.SaveChangesAsync();
            return ServiceResult<bool>.Ok(true, "Student Exam Result deleted successfully");
        }
    }
}
