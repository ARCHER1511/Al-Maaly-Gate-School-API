using Application.DTOs.AdminDTOs;
using Application.DTOs.ClassDTOs;
using Application.DTOs.StudentDTOs;
using Application.DTOs.TeacherDTOs;
using Domain.Entities;
using Domain.Wrappers;
using Microsoft.EntityFrameworkCore.Query;
using System.Linq.Expressions;
namespace Application.Interfaces
{
    public interface IAdminService
    {
        Task<ServiceResult<AdminViewDto>> GetByIdAsync(object id);
        Task<ServiceResult<IEnumerable<AdminViewDto>>> GetAllAsync();
        Task<ServiceResult<AdminViewDto?>> GetAsync(
            Expression<Func<Admin, bool>> predicate,
            Func<IQueryable<Admin>, IIncludableQueryable<Admin, object>>? include = null
        );
        Task<ServiceResult<IEnumerable<AdminViewDto>>> GetAllAsync(
            Expression<Func<Admin, bool>>? predicate = null,
            Func<IQueryable<Admin>, IIncludableQueryable<Admin, object>>? include = null,
            Func<IQueryable<Admin>, IOrderedQueryable<Admin>>? orderBy = null,
            int? skip = null,
            int? take = null
        );
        Task<ServiceResult<AdminViewDto>> CreateAsync(AdminCreateDto dto);
        Task<ServiceResult<AdminViewDto>> UpdateAsync(AdminUpdateDto admin);
        Task<ServiceResult<bool>> DeleteAsync(object id);
        Task<ServiceResult<int>> GetTeacherCount();
        Task<ServiceResult<IEnumerable<TeacherAdminViewDto>>> GetTeahcerInfo(string subjectName);
        //New
        Task<ServiceResult<bool>> ApproveTeacherAsync(string teacherId);
        Task<ServiceResult<bool>> RejectTeacherAsync(string teacherId);
        Task<ServiceResult<bool>> BlockUserAsync(string appUserId);
        Task<ServiceResult<bool>> UnblockUserAsync(string appUserId);
        Task<ServiceResult<IEnumerable<TeacherAdminViewDto>>> GetTeachersByClassAsync(string classId);
        Task<ServiceResult<IEnumerable<StudentViewDto>>> GetStudentsByClassAsync(string classId);
        Task<ServiceResult<int>> GetStudentCountAsync();
        Task<ServiceResult<bool>> MoveTeacherToAnotherClassAsync(string teacherId, string? newClassId);
        Task<ServiceResult<bool>> AssignTeacherToClassAsync(string teacherId, string classId);
        Task<ServiceResult<bool>> AssignTeacherToSubjectAsync(string teacherId, string subjectId);
        Task<ServiceResult<bool>> UnassignTeacherAsync(string teacherId);
        Task<ServiceResult<IEnumerable<TeacherAdminViewDto>>> GetDuplicateTeacherAssignmentsAsync();
        Task<ServiceResult<IEnumerable<ClassResultDto>>> GetClassResultsAsync(string classId);
    }
}
