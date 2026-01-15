using Application.DTOs.ClassDTOs;
using Application.DTOs.SubjectDTOs;
using Application.DTOs.TeacherDTOs;
using Domain.Wrappers;

namespace Application.Interfaces
{
    public interface ITeacherService
    {
        Task<ServiceResult<IEnumerable<TeacherViewDto>>> GetAllAsync();
        Task<ServiceResult<TeacherViewDto>> GetByIdAsync(string id);
        Task<ServiceResult<TeacherViewDto>> CreateAsync(CreateTeacherDto dto);
        Task<ServiceResult<TeacherViewDto>> UpdateAsync(string id, UpdateTeacherDto dto);
        Task<ServiceResult<bool>> DeleteAsync(string id);
        Task<ServiceResult<IEnumerable<TeacherViewDto>>> GetTeachersByCurriculumAsync(string curriculumId);
        Task<ServiceResult<TeacherViewDto>> AddTeacherToCurriculumAsync(string teacherId, string curriculumId);
        Task<ServiceResult<TeacherViewDto>> RemoveTeacherFromCurriculumAsync(string teacherId, string curriculumId);
        Task<ServiceResult<int>> GetTeacherCountByCurriculumAsync(string curriculumId);
        Task<ServiceResult<TeacherDetailsDto>> GetTeacherDetailsAsync(string id);
        Task<ServiceResult<IEnumerable<TeacherViewDto>>> GetTeachersNotAssignedToThisSubject(string subjectId);

        Task<ServiceResult<IEnumerable<TeacherViewDto>>> GetTeachersNotAssignedToClassAsync(string classId);

        Task<ServiceResult<bool>> AssignTeacherToClassAsync(string teacherId, string classId);
        Task<ServiceResult<bool>> UnassignTeacherFromClassAsync(string teacherId, string classId);

        Task<ServiceResult<IEnumerable<SubjectViewDto>>> GetTeacherSubjectsAsync(string teacherId);
        Task<ServiceResult<IEnumerable<ClassViewDto>>> GetTeacherClassesAsync(string teacherId);
        Task<ServiceResult<IEnumerable<TeacherViewDto>>> GetTeachersAssignedToThisSubject(string subjectId);
    }
}
