using Application.DTOs.SubjectDTOs;
using Domain.Wrappers;

namespace Application.Interfaces
{
    public interface ISubjectService
    {
        Task<ServiceResult<IEnumerable<SubjectViewDto>>> GetAll();
        Task<ServiceResult<IEnumerable<SubjectViewDto>>> GetSubjectsByGradeIdAsync(string gradeId);
        Task<ServiceResult<SubjectViewDto>> GetById(string id);
        Task<ServiceResult<SubjectViewDto>> Create(SubjectCreateDto dto);
        Task<ServiceResult<SubjectViewDto>> Update(SubjectUpdateDto dto);
        Task<ServiceResult<bool>> Delete(string id);
        Task<ServiceResult<int>> GetSubjectCountAsync();
    }
}
