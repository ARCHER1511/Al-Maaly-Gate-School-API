using Application.DTOs.ClassDTOs;
using Application.DTOs.ParentDTOs;
using Application.DTOs.StudentDTOs;
using Application.DTOs.TeacherDTOs;
using Domain.Wrappers;
using Microsoft.Identity.Client;

namespace Application.Interfaces
{
    public interface IAdminManagementService
    {
        //Teacher Management
        Task<ServiceResult<int>> GetTeacherCount();
        Task<ServiceResult<IEnumerable<TeacherAdminViewDto>>> GetTeahcerInfo(string subjectName);
        Task<ServiceResult<bool>> ApproveTeacherAsync(string teacherId, string creatorUserId);
        Task<ServiceResult<bool>> RejectTeacherAsync(string teacherId, string creatorUserId);
        Task<ServiceResult<bool>> BlockUserAsync(
            string appUserId,
            string creatorUserId,
            string role
        );
        Task<ServiceResult<bool>> UnblockUserAsync(
            string appUserId,
            string adminUserId,
            string role
        );
        Task<ServiceResult<IEnumerable<TeacherAdminViewDto>>> GetTeachersByClassAsync(
            string classId
        );
        Task<ServiceResult<IEnumerable<StudentViewDto>>> GetStudentsByClassAsync(string classId);
        Task<ServiceResult<int>> GetStudentCountAsync();
        Task<ServiceResult<bool>> MoveTeacherToAnotherClassAsync(
            string teacherId,
            string? newClassId,
            string adminUserId
        );
        Task<ServiceResult<bool>> AssignTeacherToClassAsync(string teacherId, string classId);
        Task<ServiceResult<bool>> AssignTeacherToSubjectAsync(string teacherId, string subjectId);
        Task<ServiceResult<bool>> UnassignTeacherAsync(string teacherId, string adminUserId);
        Task<ServiceResult<IEnumerable<TeacherAdminViewDto>>> GetDuplicateTeacherAssignmentsAsync();
        Task<ServiceResult<IEnumerable<ClassResultDto>>> GetClassResultsAsync(string classId);
        Task<ServiceResult<IEnumerable<TeacherAdminViewDto>>> GetPendingTeachersAsync();

        //Student Management
        Task<ServiceResult<bool>> ApproveStudentAsync(string studentId, string adminUserId);
        Task<ServiceResult<bool>> RejectStudentAsync(string studentId, string adminUserId);
        Task<ServiceResult<bool>> BlockStudentAsync(string appUserId, string adminUserId);
        Task<ServiceResult<bool>> UnblockStudentAsync(string appUserId, string adminUserId);
        Task<ServiceResult<IEnumerable<StudentViewDto>>> GetPendingStudentsAsync();
        Task<ServiceResult<bool>> MoveStudentToAnotherClassAsync(
            string studentId,
            string? newClassId,
            string adminUserId
        );

        //Parent
        Task<ServiceResult<bool>> ApproveParentAsync(string parentId, string adminUserId);
        Task<ServiceResult<bool>> RejectParentAsync(string parentId, string adminUserId);
        Task<ServiceResult<bool>> BlockParentAsync(string appUserId, string adminUserId);
        Task<ServiceResult<bool>> UnblockParentAsync(string appUserId, string adminUserId);
        Task<ServiceResult<IEnumerable<ParentViewDto>>> GetPendingParentsAsync();
    }
}
