using Application.DTOs.ClassDTOs;
using Application.DTOs.GradeDTOs;
using Application.DTOs.SubjectDTOs;
using Domain.Wrappers;

namespace Application.Interfaces
{
    public interface IGradeService
    {
        Task<ServiceResult<IEnumerable<GradeViewDto>>> GetAllAsync();
        Task<ServiceResult<GradeViewDto>> GetByIdAsync(string id);
        Task<ServiceResult<GradeViewDto>> GetByNameAsync(string gradeName);
        Task<ServiceResult<GradeViewDto>> CreateAsync(CreateGradeDto dto);
        Task<ServiceResult<GradeViewDto>> UpdateAsync(string id, UpdateGradeDto dto);
        Task<ServiceResult<bool>> DeleteAsync(string id);

        // Class Management
        Task<ServiceResult<ClassViewDto>> AddClassToGradeAsync(string gradeId, ClassDto dto);
        Task<ServiceResult<ClassViewDto>> AddClassToGradeAsync(string gradeId, CreateClassInGradeDto dto);
        Task<ServiceResult<SubjectViewDto>> AddSubjectToGradeAsync(string gradeId, SubjectCreateDto dto);
        Task<ServiceResult<bool>> RemoveClassAsync(string classId);
        Task<ServiceResult<bool>> RemoveSubjectAsync(string subjectId);
        Task<ServiceResult<bool>> MoveClassToAnotherGradeAsync(string classId, string newGradeId);
        Task<ServiceResult<bool>> MoveSubjectToAnotherGradeAsync(string subjectId, string newGradeId);

        // Get related entities
        Task<ServiceResult<IEnumerable<ClassViewDto>>> GetClassesByGradeIdAsync(string gradeId);
        Task<ServiceResult<IEnumerable<SubjectViewDto>>> GetSubjectsByGradeIdAsync(string gradeId);
        Task<ServiceResult<GradeWithDetailsDto>> GetGradeWithDetailsAsync(string id);

        Task<ServiceResult<bool>> BulkMoveClassesAsync(BulkMoveClassesDto dto);

        Task<ServiceResult<IEnumerable<GradeViewDto>>> GetGradesByCurriculumAsync(string curriculumId);

        Task<ServiceResult<int>> GetGradeCountAsync();
    }
}
