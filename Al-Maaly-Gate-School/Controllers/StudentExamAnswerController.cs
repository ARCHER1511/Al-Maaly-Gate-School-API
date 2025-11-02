using Application.DTOs.StudentExamAnswerDTOs;
using Application.Interfaces;
using Domain.Entities;
using Domain.Wrappers;
using Microsoft.AspNetCore.Mvc;

namespace Al_Maaly_Gate_School.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StudentExamAnswerController : ControllerBase
    {
        private readonly IStudentExamAnswerService _studentExamAnswerService;
        public StudentExamAnswerController(IStudentExamAnswerService studentExamAnswerService)
        {
            _studentExamAnswerService = studentExamAnswerService;
        }
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _studentExamAnswerService.GetAllAsync();
            if (!result.Success) return NotFound(ApiResponse<IEnumerable<StudentExamAnswerDto>>.Fail(result.Message!));
            return Ok(ApiResponse<IEnumerable<StudentExamAnswerDto>>.Ok(result.Data!, result.Message));
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            var result = await _studentExamAnswerService.GetByIdAsync(id);
            if (!result.Success)
                return NotFound(ApiResponse<StudentExamAnswerDto>.Fail(result.Message!));

            return Ok(ApiResponse<StudentExamAnswerDto>.Ok(result.Data!, result.Message));
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] StudentExamAnswerDto admin)
        {
            if (!ModelState.IsValid)
                return BadRequest(ApiResponse<Student>.Fail("Invalid Answer data."));

            var result = await _studentExamAnswerService.CreateAsync(admin);

            if (!result.Success)
                return BadRequest(ApiResponse<Student>.Fail(result.Message!));

            return Ok(ApiResponse<StudentExamAnswerDto>.Ok(result.Data!, result.Message));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string id, [FromBody] StudentExamAnswerDto admin)
        {
            if (id != admin.Id)
                return BadRequest(ApiResponse<StudentExamAnswerDto>.Fail("ID in URL does not match ID in body."));

            var exists = await _studentExamAnswerService.GetByIdAsync(id);
            if (!exists.Success)
                return NotFound(ApiResponse<StudentExamAnswerDto>.Fail(exists.Message!));

            var result = await _studentExamAnswerService.UpdateAsync(admin);

            if (!result.Success)
                return BadRequest(ApiResponse<StudentExamAnswerDto>.Fail(result.Message!));

            return Ok(ApiResponse<StudentExamAnswerDto>.Ok(result.Data!, result.Message));
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var result = await _studentExamAnswerService.DeleteAsync(id);

            if (!result.Success)
                return NotFound(ApiResponse<string>.Fail(result.Message!));

            return Ok(ApiResponse<string>.Ok("Answer deleted successfully."));
        }
    }
}
