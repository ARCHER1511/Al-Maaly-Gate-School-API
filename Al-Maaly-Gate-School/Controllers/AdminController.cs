using Application.DTOs.AdminDTOs;
using Application.DTOs.ClassDTOs;
using Application.DTOs.StudentDTOs;
using Application.DTOs.TeacherDTOs;
using Application.Interfaces;
using Domain.Entities;
using Domain.Wrappers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Al_Maaly_Gate_School.Controllers
{
    [Route("api/[Controller]")]
    [ApiController]
    [AllowAnonymous]
    public class AdminController : ControllerBase
    {
        private readonly IAdminService _adminService;

        public AdminController(IAdminService adminService)
        {
            _adminService = adminService;
        }

        // GET: api/Admin
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _adminService.GetAllAsync();
            if (!result.Success)
                return NotFound(ApiResponse<IEnumerable<AdminViewDto>>.Fail(result.Message!));
            return Ok(ApiResponse<IEnumerable<AdminViewDto>>.Ok(result.Data!, result.Message));
        }

        //GET: api/Admin/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            var result = await _adminService.GetByIdAsync(id);
            if (!result.Success)
                return NotFound(ApiResponse<AdminViewDto>.Fail(result.Message!));

            return Ok(ApiResponse<AdminViewDto>.Ok(result.Data!, result.Message));
        }

        //POST: api/Admin
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] AdminCreateDto admin)
        {
            if (!ModelState.IsValid)
                return BadRequest(ApiResponse<Admin>.Fail("Invalid admin data."));

            var result = await _adminService.CreateAsync(admin);

            if (!result.Success)
                return BadRequest(ApiResponse<Admin>.Fail(result.Message!));

            return CreatedAtAction(nameof(GetById), new { id = result.Data!.Id },
                ApiResponse<AdminViewDto>.Ok(result.Data, result.Message));
        }

        // PUT: api/Admin/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string id, [FromBody] AdminUpdateDto admin)
        {
            if (id != admin.Id)
                return BadRequest(ApiResponse<AdminViewDto>.Fail("ID in URL does not match ID in body."));

            var exists = await _adminService.GetByIdAsync(id);
            if (!exists.Success)
                return NotFound(ApiResponse<AdminViewDto>.Fail(exists.Message!));

            var result = await _adminService.UpdateAsync(admin);

            if (!result.Success)
                return BadRequest(ApiResponse<AdminViewDto>.Fail(result.Message!));

            return Ok(ApiResponse<AdminViewDto>.Ok(result.Data!, result.Message));
        }

        // DELETE: api/Admin/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var result = await _adminService.DeleteAsync(id);

            if (!result.Success)
                return NotFound(ApiResponse<string>.Fail(result.Message!));

            return Ok(ApiResponse<string>.Ok("Admin deleted successfully."));
        }

        // Teachers

        [HttpGet("TeacherCount")]
        public async Task<IActionResult> TeacherCount() 
        {
            var result = await _adminService.GetTeacherCount();
            if (!result.Success) { ApiResponse<int>.Fail(result.Message!); }
            return Ok(ApiResponse<int>.Ok(result.Data,result.Message));
        }
        [HttpGet("TeacherBySubject")]
        public async Task<IActionResult> TeacherbySubject([FromQuery] string subjectName) 
        {
            var result = await _adminService.GetTeahcerInfo(subjectName);
            if(!result.Success)
                return NotFound(ApiResponse<IEnumerable<TeacherAdminViewDto>>.Fail(result.Message!));
            return Ok(ApiResponse<IEnumerable<TeacherAdminViewDto>>.Ok(result.Data!, result.Message) );
        }
        //Approve Teacher
        [HttpPut("approve-teacher/{teacherId}")]
        public async Task<IActionResult> ApproveTeacher(string teacherId)
        {
            var result = await _adminService.ApproveTeacherAsync(teacherId);
            return result.Success
                ? Ok(ApiResponse<bool>.Ok(result.Data!, result.Message))
                : BadRequest(ApiResponse<bool>.Fail(result.Message!));
        }

        //Reject Teacher
        [HttpPut("reject-teacher/{teacherId}")]
        public async Task<IActionResult> RejectTeacher(string teacherId)
        {
            var result = await _adminService.RejectTeacherAsync(teacherId);
            return result.Success
                ? Ok(ApiResponse<bool>.Ok(result.Data!, result.Message))
                : BadRequest(ApiResponse<bool>.Fail(result.Message!));
        }

        //Block User
        [HttpPut("block-user/{appUserId}")]
        public async Task<IActionResult> BlockUser(string appUserId)
        {
            var result = await _adminService.BlockUserAsync(appUserId);
            return result.Success
                ? Ok(ApiResponse<bool>.Ok(result.Data!, result.Message))
                : BadRequest(ApiResponse<bool>.Fail(result.Message!));
        }

        //Unblock User
        [HttpPut("unblock-user/{appUserId}")]
        public async Task<IActionResult> UnblockUser(string appUserId)
        {
            var result = await _adminService.UnblockUserAsync(appUserId);
            return result.Success
                ? Ok(ApiResponse<bool>.Ok(result.Data!, result.Message))
                : BadRequest(ApiResponse<bool>.Fail(result.Message!));
        }

        //Get Teachers by Class
        [HttpGet("teachers/class/{classId}")]
        public async Task<IActionResult> GetTeachersByClass(string classId)
        {
            var result = await _adminService.GetTeachersByClassAsync(classId);
            return result.Success
                ? Ok(ApiResponse<IEnumerable<TeacherAdminViewDto>>.Ok(result.Data!, result.Message))
                : BadRequest(ApiResponse<IEnumerable<TeacherAdminViewDto>>.Fail(result.Message!));
        }

        //Get Students by Class
        [HttpGet("students/class/{classId}")]
        public async Task<IActionResult> GetStudentsByClass(string classId)
        {
            var result = await _adminService.GetStudentsByClassAsync(classId);
            return result.Success
                ? Ok(ApiResponse<IEnumerable<StudentViewDto>>.Ok(result.Data!, result.Message))
                : BadRequest(ApiResponse<IEnumerable<StudentViewDto>>.Fail(result.Message!));
        }

        //Get Student Count
        [HttpGet("students/count")]
        public async Task<IActionResult> GetStudentCount()
        {
            var result = await _adminService.GetStudentCountAsync();
            return result.Success
                ? Ok(ApiResponse<int>.Ok(result.Data!, result.Message))
                : BadRequest(ApiResponse<int>.Fail(result.Message!));
        }

        //Move Teacher to Another Class
        [HttpPut("teachers/move/{teacherId}")]
        public async Task<IActionResult> MoveTeacherToAnotherClass(string teacherId, [FromQuery] string? newClassId)
        {
            var result = await _adminService.MoveTeacherToAnotherClassAsync(teacherId, newClassId);
            return result.Success
                ? Ok(ApiResponse<bool>.Ok(result.Data!, result.Message))
                : BadRequest(ApiResponse<bool>.Fail(result.Message!));
        }

        //Assign Teacher to Class
        [HttpPost("teachers/assign-class")]
        public async Task<IActionResult> AssignTeacherToClass([FromQuery] string teacherId, [FromQuery] string classId)
        {
            var result = await _adminService.AssignTeacherToClassAsync(teacherId, classId);
            return result.Success
                ? Ok(ApiResponse<bool>.Ok(result.Data!, result.Message))
                : BadRequest(ApiResponse<bool>.Fail(result.Message!));
        }

        //Assign Teacher to Subject
        [HttpPost("teachers/assign-subject")]
        public async Task<IActionResult> AssignTeacherToSubject([FromQuery] string teacherId, [FromQuery] string subjectId)
        {
            var result = await _adminService.AssignTeacherToSubjectAsync(teacherId, subjectId);
            return result.Success
                ? Ok(ApiResponse<bool>.Ok(result.Data!, result.Message))
                : BadRequest(ApiResponse<bool>.Fail(result.Message!));
        }

        //Unassign Teacher
        [HttpDelete("teachers/unassign/{teacherId}")]
        public async Task<IActionResult> UnassignTeacher(string teacherId)
        {
            var result = await _adminService.UnassignTeacherAsync(teacherId);
            return result.Success
                ? Ok(ApiResponse<bool>.Ok(result.Data!, result.Message))
                : BadRequest(ApiResponse<bool>.Fail(result.Message!));
        }

        //Detect Duplicate Teacher Assignments
        [HttpGet("teachers/duplicates")]
        public async Task<IActionResult> GetDuplicateTeacherAssignments()
        {
            var result = await _adminService.GetDuplicateTeacherAssignmentsAsync();
            return result.Success
                ? Ok(ApiResponse<IEnumerable<TeacherAdminViewDto>>.Ok(result.Data!, result.Message))
                : BadRequest(ApiResponse<IEnumerable<TeacherAdminViewDto>>.Fail(result.Message!));
        }

        //Get Class Results
        [HttpGet("classes/results/{classId}")]
        public async Task<IActionResult> GetClassResults(string classId)
        {
            var result = await _adminService.GetClassResultsAsync(classId);
            return result.Success
                ? Ok(ApiResponse<IEnumerable<ClassResultDto>>.Ok(result.Data!, result.Message))
                : BadRequest(ApiResponse<IEnumerable<ClassResultDto>>.Fail(result.Message!));
        }
    }
}
