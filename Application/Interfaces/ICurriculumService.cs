using Application.DTOs.CurriculumDTOs;
using Domain.Wrappers;

namespace Application.Interfaces
{
    public interface ICurriculumService
    {
        Task<ServiceResult<CurriculumDto>> GetByIdAsync(string id);
        Task<ServiceResult<IEnumerable<CurriculumDto>>> GetAllAsync();
        Task<ServiceResult<CurriculumDto>> CreateAsync(CreateCurriculumDto dto);
        Task<ServiceResult<CurriculumDto>> UpdateAsync(string id, UpdateCurriculumDto dto);
        Task<ServiceResult<bool>> DeleteAsync(string id);
        Task<ServiceResult<bool>> ExistsAsync(string id);
        Task<ServiceResult<CurriculumDetailsDto>> GetWithDetailsAsync(string id);
        Task<ServiceResult<bool>> HasStudentsAsync(string curriculumId);
        Task<ServiceResult<bool>> HasTeachersAsync(string curriculumId);
        Task<ServiceResult<int>> GetCountAsync(); // This one is already correct
    }
}
