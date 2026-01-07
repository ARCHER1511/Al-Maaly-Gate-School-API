using Application.DTOs.ParentDTOs;
using Application.DTOs.StudentDTOs;
using Application.Interfaces;
using Domain.Entities;
using Domain.Wrappers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Al_Maaly_Gate_School.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [AllowAnonymous]
    public class StudentController : ControllerBase
    {
        private readonly IStudentService _studentService;

        public StudentController(IStudentService studentService)
        {
            _studentService = studentService;
        }

        [HttpGet("all")]
        public async Task<IActionResult> GetAll()
        {
            var result = await _studentService.GetAllAsync();
            return Ok(ApiResponse<IEnumerable<StudentViewDto>>.Ok(result.Data!, result.Message));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            var result = await _studentService.GetByIdAsync(id);
            if (!result.Success)
                return NotFound(ApiResponse<StudentViewDto>.Fail(result.Message!));

            return Ok(ApiResponse<StudentViewDto>.Ok(result.Data!, result.Message));
        }

        // NEW ENDPOINT: Get students by curriculum
        [HttpGet("curriculum/{curriculumId}")]
        public async Task<IActionResult> GetByCurriculum(string curriculumId)
        {
            var result = await _studentService.GetStudentsByCurriculumAsync(curriculumId);
            if (!result.Success)
                return NotFound(ApiResponse<IEnumerable<StudentViewDto>>.Fail(result.Message!));

            return Ok(ApiResponse<IEnumerable<StudentViewDto>>.Ok(result.Data!, result.Message));
        }

        // NEW ENDPOINT: Get student count by curriculum
        [HttpGet("curriculum/{curriculumId}/count")]
        public async Task<IActionResult> GetCountByCurriculum(string curriculumId)
        {
            var result = await _studentService.GetStudentCountByCurriculumAsync(curriculumId);
            return Ok(ApiResponse<int>.Ok(result.Data!, result.Message));
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateStudentDto dto) // Changed parameter type
        {
            if (!ModelState.IsValid)
                return BadRequest(ApiResponse<CreateStudentDto>.Fail("Invalid Student data."));

            var result = await _studentService.CreateAsync(dto);

            if (!result.Success)
                return BadRequest(ApiResponse<StudentViewDto>.Fail(result.Message!));

            return Ok(ApiResponse<StudentViewDto>.Ok(result.Data!, result.Message));
        }

        [HttpPut("{id}/additional-info")]
        public async Task<IActionResult> UpdateAdditionalInfo(string id, [FromBody] UpdateStudentDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ApiResponse<UpdateStudentDto>.Fail("Invalid data."));

            var result = await _studentService.UpdateStudentAdditionalInfoAsync(id, dto);

            if (!result.Success)
                return BadRequest(ApiResponse<StudentViewDto>.Fail(result.Message!));

            return Ok(ApiResponse<StudentViewDto>.Ok(result.Data!, result.Message));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string id, [FromBody] UpdateStudentDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ApiResponse<UpdateStudentDto>.Fail("Invalid Student data."));

            var result = await _studentService.UpdateAsync(id, dto);

            if (!result.Success)
                return BadRequest(ApiResponse<StudentViewDto>.Fail(result.Message!));

            return Ok(ApiResponse<StudentViewDto>.Ok(result.Data!, result.Message));
        }

        // NEW ENDPOINT: Move student to different curriculum
        [HttpPut("{studentId}/move-to-curriculum/{curriculumId}")]
        public async Task<IActionResult> MoveToCurriculum(string studentId, string curriculumId)
        {
            var result = await _studentService.MoveStudentToCurriculumAsync(studentId, curriculumId);
            if (!result.Success)
                return BadRequest(ApiResponse<StudentViewDto>.Fail(result.Message!));

            return Ok(ApiResponse<StudentViewDto>.Ok(result.Data!, result.Message));
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var result = await _studentService.DeleteAsync(id);

            if (!result.Success)
                return NotFound(ApiResponse<bool>.Fail(result.Message!));

            return Ok(ApiResponse<bool>.Ok(result.Data!, result.Message));
        }

        [HttpGet("searchTerm")]
        public async Task<IActionResult> SearchStudents([FromQuery] SearchTermDto dto)
        {
            var result = await _studentService.SearchStudentsAsync(dto);
            if (!result.Success)
                return NotFound(ApiResponse<List<StudentSearchResultDto>>.Fail(result.Message!));

            return Ok(ApiResponse<List<StudentSearchResultDto>>.Ok(result.Data!, result.Message));
        }
    }
}