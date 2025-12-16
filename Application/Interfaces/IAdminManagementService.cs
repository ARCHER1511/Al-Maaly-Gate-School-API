using Application.DTOs.ClassDTOs;
using Application.DTOs.ParentDTOs;
using Application.DTOs.StudentDTOs;
using Application.DTOs.TeacherDTOs;
using Domain.Wrappers;

namespace Application.Interfaces
{
    public interface IAdminManagementService
    {
        //Teacher Management
        Task<ServiceResult<int>> GetTeacherCount();
        Task<ServiceResult<IEnumerable<TeacherAdminViewDto>>> GetTeahcerInfo(string subjectName);

        Task<ServiceResult<bool>> ApproveUserAsync(string accountId, string adminId, string role);
        Task<ServiceResult<bool>> BlockUserAsync(string accountId, string adminId, string role);
        Task<ServiceResult<bool>> UnblockUserAsync(string accountId, string adminId, string role);
        Task<ServiceResult<bool>> RejectUserAsync(string accountId, string adminId, string role);

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

        Task<ServiceResult<IEnumerable<StudentViewDto>>> GetPendingStudentsAsync();
        Task<ServiceResult<bool>> MoveStudentToAnotherClassAsync(
            string studentId,
            string? newClassId,
            string adminUserId
        );

        Task<ServiceResult<bool>> UpdateStudentCurriculumAsync(
            string studentId,
            string curriculumId,
            string adminUserId
         );

        Task<ServiceResult<bool>> BulkUpdateCurriculumForClassAsync(
            string classId,
            string curriculumId,
            string adminUserId
         );

        Task<ServiceResult<IEnumerable<ParentViewWithChildrenDto>>> GetPendingParentsAsync();

        Task<ServiceResult<bool>> UnassignTeacherFromClassAsync(string teacherId, string classId);
        Task<ServiceResult<bool>> BulkAssignTeachersAsync(BulkAssignTeachersDto dto);

        Task<ServiceResult<bool>> ApproveParentWithStudent(RelationParentWithStudentRequest relationRequest);
        Task<ServiceResult<bool>> ApproveParentBulk(ParentApprovalBulkDto bulkDto);
        Task<ServiceResult<bool>> AddStudentToParent(string parentId, ParentStudentApprovalDto studentDto);
        Task<ServiceResult<bool>> RemoveStudentFromParent(RemoveStudentFromParentRequest unRelationRequest);
        Task<ServiceResult<bool>> AddStudentToExistingParent(RelationParentWithStudentRequest relationRequest);
    }
}
