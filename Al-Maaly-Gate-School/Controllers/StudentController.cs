using Application.DTOs.ClassDTOs;
using Application.DTOs.StudentDTOs;
using Application.Interfaces;
using Domain.Entities;
using Domain.Wrappers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Al_Maaly_Gate_School.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [AllowAnonymous]
    public class StudentController : ControllerBase
    {
        private readonly IStudentService _StudentService;
        public StudentController(IStudentService studentService)
        {
            _StudentService = studentService;
        }
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _StudentService.GetAllAsync();
            if (!result.Success) return NotFound(ApiResponse<IEnumerable<StudentViewDto>>.Fail(result.Message!));
            return Ok(ApiResponse<IEnumerable<StudentViewDto>>.Ok(result.Data!, result.Message));
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            var result = await _StudentService.GetByIdAsync(id);
            if (!result.Success)
                return NotFound(ApiResponse<StudentViewDto>.Fail(result.Message!));

            return Ok(ApiResponse<StudentViewDto>.Ok(result.Data!, result.Message));
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] StudentViewDto admin)
        {
            if (!ModelState.IsValid)
                return BadRequest(ApiResponse<Student>.Fail("Invalid Student data."));

            var result = await _StudentService.CreateAsync(admin);

            if (!result.Success)
                return BadRequest(ApiResponse<Student>.Fail(result.Message!));

            return Ok(ApiResponse<StudentViewDto>.Ok(result.Data!, result.Message));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string id, [FromBody] StudentViewDto admin)
        {
            if (id != admin.Id)
                return BadRequest(ApiResponse<StudentViewDto>.Fail("ID in URL does not match ID in body."));

            var exists = await _StudentService.GetByIdAsync(id);
            if (!exists.Success)
                return NotFound(ApiResponse<StudentViewDto>.Fail(exists.Message!));

            var result = await _StudentService.UpdateAsync(admin);

            if (!result.Success)
                return BadRequest(ApiResponse<StudentViewDto>.Fail(result.Message!));

            return Ok(ApiResponse<StudentViewDto>.Ok(result.Data!, result.Message));
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var result = await _StudentService.DeleteAsync(id);

            if (!result.Success)
                return NotFound(ApiResponse<string>.Fail(result.Message!));

            return Ok(ApiResponse<string>.Ok("Student deleted successfully."));
        }
    }
}
