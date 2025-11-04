using Application.DTOs.StudentExamAnswerDTOs;
using Application.Interfaces;
using AutoMapper;
using Domain.Entities;
using Domain.Wrappers;
using Infrastructure.Interfaces;


namespace Application.Services
{
    public class StudentExamResultService : IStudentExamResultService
    {
        private readonly IStudentExamResultRepository _StudentExamResultRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public StudentExamResultService(IStudentExamResultRepository studentExamResultRepository, IUnitOfWork unitOfWork, IMapper mapper)
        {
            _StudentExamResultRepository = studentExamResultRepository;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<ServiceResult<IEnumerable<StudentExamResultDto>>> GetAllAsync()
        {
            var result = await  _StudentExamResultRepository.GetAllAsync();
            if (result == null) return ServiceResult<IEnumerable<StudentExamResultDto>>.Fail("StudentExamResult not found");

            var resultDto = _mapper.Map<IEnumerable<StudentExamResultDto>>(result);
            return ServiceResult<IEnumerable<StudentExamResultDto>>.Ok(resultDto, "StudentExamResult retrieved successfully");
        }
        public async Task<ServiceResult<StudentExamResultDto>> GetByIdAsync(object id)
        {
            var result = await  _StudentExamResultRepository.GetByIdAsync(id);
            if (result == null) return ServiceResult<StudentExamResultDto>.Fail("claStudentExamResultss not found");

            var resultDto = _mapper.Map<StudentExamResultDto>(result);
            return ServiceResult<StudentExamResultDto>.Ok(resultDto, "StudentExamResult retrieved successfully");
        }
        public async Task<ServiceResult<StudentExamResultDto>> CreateAsync(StudentExamResultDto dto)
        {
            var result = _mapper.Map<StudentExamResult>(dto);

            await  _StudentExamResultRepository.AddAsync(result);
            await _unitOfWork.SaveChangesAsync();

            var viewDto = _mapper.Map<StudentExamResultDto>(result);
            return ServiceResult<StudentExamResultDto>.Ok(viewDto, "StudentExamResult created successfully");
        }
        public async Task<ServiceResult<StudentExamResultDto>> UpdateAsync(StudentExamResultDto dto)
        {
            var existingresult = await  _StudentExamResultRepository.GetByIdAsync(dto.Id);
            if (existingresult == null)
                return ServiceResult<StudentExamResultDto>.Fail("StudentExamResult not found");

            _mapper.Map(dto, existingresult);

             _StudentExamResultRepository.Update(existingresult);
            await _unitOfWork.SaveChangesAsync();

            var viewDto = _mapper.Map<StudentExamResultDto>(existingresult);
            return ServiceResult<StudentExamResultDto>.Ok(viewDto, "StudentExamResult updated successfully");
        }
        public async Task<ServiceResult<bool>> DeleteAsync(object id)
        {
            var result = await  _StudentExamResultRepository.GetByIdAsync(id);
            if (result == null)
                return ServiceResult<bool>.Fail("StudentExamResult not found");

             _StudentExamResultRepository.Delete(result);
            await _unitOfWork.SaveChangesAsync();
            return ServiceResult<bool>.Ok(true, "StudentExamResult deleted successfully");
        }
    }
}
