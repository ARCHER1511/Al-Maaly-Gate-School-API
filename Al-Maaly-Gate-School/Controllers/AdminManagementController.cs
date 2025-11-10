using Application.DTOs.ClassDTOs;
using Application.DTOs.ParentDTOs;
using Application.DTOs.StudentDTOs;
using Application.DTOs.TeacherDTOs;
using Application.Interfaces;
using Application.Services;
using Domain.Wrappers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Al_Maaly_Gate_School.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class AdminManagementController : ControllerBase
    {
        private readonly IAdminManagementService _adminManagementService;
        public AdminManagementController(IAdminManagementService adminManagementService)
        {
            _adminManagementService = adminManagementService;
        }
        // Teachers

        [HttpGet("TeacherCount")]
        public async Task<IActionResult> TeacherCount()
        {
            var result = await _adminManagementService.GetTeacherCount();
            if (!result.Success) { ApiResponse<int>.Fail(result.Message!); }
            return Ok(ApiResponse<int>.Ok(result.Data, result.Message));
        }
        [HttpGet("TeacherBySubject")]
        public async Task<IActionResult> TeacherbySubject([FromQuery] string subjectName)
        {
            var result = await _adminManagementService.GetTeahcerInfo(subjectName);
            if (!result.Success)
                return NotFound(ApiResponse<IEnumerable<TeacherAdminViewDto>>.Fail(result.Message!));
            return Ok(ApiResponse<IEnumerable<TeacherAdminViewDto>>.Ok(result.Data!, result.Message));
        }
        //Approve Teacher
        [HttpPut("approve-teacher/{teacherId}")]
        public async Task<IActionResult> ApproveTeacher(string teacherId, string adminUserId)
        {
            var result = await _adminManagementService.ApproveTeacherAsync(teacherId, adminUserId);
            return result.Success
                ? Ok(ApiResponse<bool>.Ok(result.Data!, result.Message))
                : BadRequest(ApiResponse<bool>.Fail(result.Message!));
        }

        //Reject Teacher
        [HttpPut("reject-teacher/{teacherId}")]
        public async Task<IActionResult> RejectTeacher(string teacherId, string notifyCreatorUserId)
        {
            var result = await _adminManagementService.RejectTeacherAsync(teacherId, notifyCreatorUserId);
            return result.Success
                ? Ok(ApiResponse<bool>.Ok(result.Data!, result.Message))
                : BadRequest(ApiResponse<bool>.Fail(result.Message!));
        }

        //Block User
        [HttpPut("block-user/{appUserId}")]
        public async Task<IActionResult> BlockUser(string appUserId, string adminUserId, string role)
        {
            var result = await _adminManagementService.BlockUserAsync(appUserId, adminUserId, role);
            return result.Success
                ? Ok(ApiResponse<bool>.Ok(result.Data!, result.Message))
                : BadRequest(ApiResponse<bool>.Fail(result.Message!));
        }

        //Unblock User
        [HttpPut("unblock-user/{appUserId}")]
        public async Task<IActionResult> UnblockUser(string appUserId, string adminUserId, string role)
        {
            var result = await _adminManagementService.UnblockUserAsync(appUserId, adminUserId, role);
            return result.Success
                ? Ok(ApiResponse<bool>.Ok(result.Data!, result.Message))
                : BadRequest(ApiResponse<bool>.Fail(result.Message!));
        }

        //Get Teachers by Class
        [HttpGet("teachers/class/{classId}")]
        public async Task<IActionResult> GetTeachersByClass(string classId)
        {
            var result = await _adminManagementService.GetTeachersByClassAsync(classId);
            return result.Success
                ? Ok(ApiResponse<IEnumerable<TeacherAdminViewDto>>.Ok(result.Data!, result.Message))
                : BadRequest(ApiResponse<IEnumerable<TeacherAdminViewDto>>.Fail(result.Message!));
        }

        //Get Students by Class
        [HttpGet("students/class/{classId}")]
        [Authorize(Roles = "admin,teacher")]
        public async Task<IActionResult> GetStudentsByClass(string classId)
        {
            var result = await _adminManagementService.GetStudentsByClassAsync(classId);
            return result.Success
                ? Ok(ApiResponse<IEnumerable<StudentViewDto>>.Ok(result.Data!, result.Message))
                : BadRequest(ApiResponse<IEnumerable<StudentViewDto>>.Fail(result.Message!));
        }

        //Get Student Count
        [HttpGet("students/count")]
        public async Task<IActionResult> GetStudentCount()
        {
            var result = await _adminManagementService.GetStudentCountAsync();
            return result.Success
                ? Ok(ApiResponse<int>.Ok(result.Data!, result.Message))
                : BadRequest(ApiResponse<int>.Fail(result.Message!));
        }

        //Move Teacher to Another Class
        [HttpPut("teachers/move/{teacherId}")]
        public async Task<IActionResult> MoveTeacherToAnotherClass(string teacherId, [FromQuery] string? newClassId, string adminUserId)
        {
            var result = await _adminManagementService.MoveTeacherToAnotherClassAsync(teacherId, newClassId,adminUserId);
            return result.Success
                ? Ok(ApiResponse<bool>.Ok(result.Data!, result.Message))
                : BadRequest(ApiResponse<bool>.Fail(result.Message!));
        }

        //Assign Teacher to Class
        [HttpPost("teachers/assign-class")]
        public async Task<IActionResult> AssignTeacherToClass([FromQuery] string teacherId, [FromQuery] string classId)
        {
            var result = await _adminManagementService.AssignTeacherToClassAsync(teacherId, classId);
            return result.Success
                ? Ok(ApiResponse<bool>.Ok(result.Data!, result.Message))
                : BadRequest(ApiResponse<bool>.Fail(result.Message!));
        }

        //Assign Teacher to Subject
        [HttpPost("teachers/assign-subject")]
        public async Task<IActionResult> AssignTeacherToSubject([FromQuery] string teacherId, [FromQuery] string subjectId)
        {
            var result = await _adminManagementService.AssignTeacherToSubjectAsync(teacherId, subjectId);
            return result.Success
                ? Ok(ApiResponse<bool>.Ok(result.Data!, result.Message))
                : BadRequest(ApiResponse<bool>.Fail(result.Message!));
        }

        //Unassign Teacher
        [HttpDelete("teachers/unassign/{teacherId}")]
        public async Task<IActionResult> UnassignTeacher(string teacherId, string adminUserId)
        {
            var result = await _adminManagementService.UnassignTeacherAsync(teacherId,adminUserId);
            return result.Success
                ? Ok(ApiResponse<bool>.Ok(result.Data!, result.Message))
                : BadRequest(ApiResponse<bool>.Fail(result.Message!));
        }

        //Detect Duplicate Teacher Assignments
        [HttpGet("teachers/duplicates")]
        public async Task<IActionResult> GetDuplicateTeacherAssignments()
        {
            var result = await _adminManagementService.GetDuplicateTeacherAssignmentsAsync();
            return result.Success
                ? Ok(ApiResponse<IEnumerable<TeacherAdminViewDto>>.Ok(result.Data!, result.Message))
                : BadRequest(ApiResponse<IEnumerable<TeacherAdminViewDto>>.Fail(result.Message!));
        }

        //Get Class Results
        [HttpGet("classes/results/{classId}")]
        public async Task<IActionResult> GetClassResults(string classId)
        {
            var result = await _adminManagementService.GetClassResultsAsync(classId);
            return result.Success
                ? Ok(ApiResponse<IEnumerable<ClassResultDto>>.Ok(result.Data!, result.Message))
                : BadRequest(ApiResponse<IEnumerable<ClassResultDto>>.Fail(result.Message!));
        }
        //Get Pending Teachers
        [HttpGet("teachers/pending")]
        public async Task<IActionResult> GetPendingTeachers()
        {
            var result = await _adminManagementService.GetPendingTeachersAsync();
            return result.Success
                ? Ok(ApiResponse<IEnumerable<TeacherAdminViewDto>>.Ok(result.Data!, result.Message))
                : BadRequest(ApiResponse<IEnumerable<TeacherAdminViewDto>>.Fail(result.Message!));
        }
        // Students
        [HttpGet("pending-students")]
        public async Task<IActionResult> GetPendingStudents()
        {
            var result = await _adminManagementService.GetPendingStudentsAsync();
            return result.Success
                ? Ok(ApiResponse<IEnumerable<StudentViewDto>>.Ok(result.Data!, result.Message))
                : BadRequest(ApiResponse<IEnumerable<StudentViewDto>>.Fail(result.Message!));
        }
        [HttpPut("approve-student")]
        public async Task<IActionResult> ApproveStudent(string studentId, string adminUserId)
        {
            var result = await _adminManagementService.ApproveStudentAsync(studentId, adminUserId);
            return result.Success
                ? Ok(ApiResponse<bool>.Ok(result.Data!, result.Message))
                : BadRequest(ApiResponse<bool>.Fail(result.Message!));
        }
        [HttpPut("reject-student")]
        public async Task<IActionResult> RejectStudent(string studentId, string adminUserId)
        {
            var result = await _adminManagementService.RejectStudentAsync(studentId, adminUserId);
            return result.Success
                ? Ok(ApiResponse<bool>.Ok(result.Data!, result.Message))
                : BadRequest(ApiResponse<bool>.Fail(result.Message!));

        }
        [HttpPut("block-student")]
        public async Task<IActionResult> BlockStudent(string appUserId, string adminUserId)
        {
            var result = await _adminManagementService.BlockStudentAsync(appUserId, adminUserId);
            return result.Success
                ? Ok(ApiResponse<bool>.Ok(result.Data!, result.Message))
                : BadRequest(ApiResponse<bool>.Fail(result.Message!));
        }
        [HttpPut("unblock-student")]
        public async Task<IActionResult> UnblockStudent(string appUserId, string adminUserId)
        {
            var result = await _adminManagementService.UnblockStudentAsync(appUserId, adminUserId);
            return result.Success
                ? Ok(ApiResponse<bool>.Ok(result.Data!, result.Message))
                : BadRequest(ApiResponse<bool>.Fail(result.Message!));
        }
        [HttpGet("student-count")]
        public async Task<IActionResult> GetStudentCountAsync()
        {
            var result = await _adminManagementService.GetStudentCountAsync();
            return result.Success
                ? Ok(ApiResponse<int>.Ok(result.Data!, result.Message))
                : BadRequest(ApiResponse<int>.Fail(result.Message!));
        }
        [HttpGet("studnet-by-class")]
        public async Task<IActionResult> GetStudentsByClassAsync(string classId)
        {
            var result = await _adminManagementService.GetStudentsByClassAsync(classId);
            return result.Success
                ? Ok(ApiResponse<IEnumerable<StudentViewDto>>.Ok(result.Data!, result.Message))
                : BadRequest(ApiResponse<IEnumerable<StudentViewDto>>.Fail(result.Message!));
        }
        [HttpPut("move-student-class")]
        public async Task<IActionResult> MoveStudentToAnotherClass(string studentId, string? newClassId, string adminUserId)
        {
            var result = await _adminManagementService.MoveStudentToAnotherClassAsync(studentId, newClassId, adminUserId);
            return result.Success
                ? Ok(ApiResponse<bool>.Ok(result.Data!, result.Message))
                : BadRequest(ApiResponse<bool>.Fail(result.Message!));
        }
        // Parents
        [HttpGet("pending-parents")]
        public async Task<IActionResult> GetPendingParents()
        {
            var result = await _adminManagementService.GetPendingParentsAsync();
            return result.Success
                ? Ok(ApiResponse<IEnumerable<ParentViewDto>>.Ok(result.Data!, result.Message))
                : BadRequest(ApiResponse<IEnumerable<ParentViewDto>>.Fail(result.Message!));
        }
        [HttpPut("approve-parent")]
        public async Task<IActionResult> ApproveParent(string parentId, string adminUserId)
        {
            var result = await _adminManagementService.ApproveParentAsync(parentId, adminUserId);
            return result.Success
                ? Ok(ApiResponse<bool>.Ok(result.Data!, result.Message))
                : BadRequest(ApiResponse<bool>.Fail(result.Message!));
        }
        [HttpPut("reject-parent")]
        public async Task<IActionResult> RejectParent(string parentId, string adminUserId)
        {
            var result = await _adminManagementService.RejectParentAsync(parentId, adminUserId);
            return result.Success
                ? Ok(ApiResponse<bool>.Ok(result.Data!, result.Message))
                : BadRequest(ApiResponse<bool>.Fail(result.Message!));
        }
        [HttpPut("block-parent")]
        public async Task<IActionResult> BlockParent(string appUserId, string adminUserId)
        {
            var result = await _adminManagementService.BlockParentAsync(appUserId, adminUserId);
            return result.Success
                ? Ok(ApiResponse<bool>.Ok(result.Data!, result.Message))
                : BadRequest(ApiResponse<bool>.Fail(result.Message!));
        }
        [HttpPut("unblock-parent")]
        public async Task<IActionResult> UnblockParent(string appUserId, string adminUserId)
        {
            var result = await _adminManagementService.UnblockParentAsync(appUserId, adminUserId);
            return result.Success
                ? Ok(ApiResponse<bool>.Ok(result.Data!, result.Message))
                : BadRequest(ApiResponse<bool>.Fail(result.Message!));
        }
    }
}
