using Application.DTOs.StudentExamAnswerDTOs;
using Application.Interfaces;
using Domain.Wrappers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Al_Maaly_Gate_School.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [AllowAnonymous]
    public class StudentExamResultController : ControllerBase
    {
        private readonly IStudentExamResultService _StudentExamResultService;
        public StudentExamResultController(IStudentExamResultService studentExamResultService)
        {
            _StudentExamResultService = studentExamResultService;
        }

        [HttpGet("student/results/{Id}")]
        public async Task<IActionResult> GetAllResultsByStudentId(string Id)
        {
            var result = await _StudentExamResultService.GetAllResultsByStudentIdAsync(Id);
            return Ok(ApiResponse<IEnumerable<StudentExamResultDto>>.Ok(result.Data ?? Enumerable.Empty<StudentExamResultDto>(),result.Message));
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _StudentExamResultService.GetAllAsync();
            if (!result.Success) return NotFound(ApiResponse<IEnumerable<StudentExamResultDto>>.Fail(result.Message!));
            return Ok(ApiResponse<IEnumerable<StudentExamResultDto>>.Ok(result.Data!, result.Message));
        }

        [HttpGet("StudentExamResult/{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            var result = await _StudentExamResultService.GetByIdAsync(id);
            if (!result.Success)
                return NotFound(ApiResponse<StudentExamResultDto>.Fail(result.Message!));

            return Ok(ApiResponse<StudentExamResultDto>.Ok(result.Data!, result.Message));
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] StudentExamResultDto StudentExamResultDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ApiResponse<StudentExamResultDto>.Fail("Invalid StudentExamResult data."));

            var result = await _StudentExamResultService.CreateAsync(StudentExamResultDto);

            if (!result.Success)
                return BadRequest(ApiResponse<StudentExamResultDto>.Fail(result.Message!));

            return Ok(ApiResponse<StudentExamResultDto>.Ok(result.Data!, result.Message));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string id, [FromBody] StudentExamResultDto StudentExamResultDto)
        {
            if (id != StudentExamResultDto.Id)
                return BadRequest(ApiResponse<StudentExamResultDto>.Fail("ID in URL does not match ID in body."));

            var exists = await _StudentExamResultService.GetByIdAsync(id);
            if (!exists.Success)
                return NotFound(ApiResponse<StudentExamResultDto>.Fail(exists.Message!));

            var result = await _StudentExamResultService.UpdateAsync(StudentExamResultDto);

            if (!result.Success)
                return BadRequest(ApiResponse<StudentExamResultDto>.Fail(result.Message!));

            return Ok(ApiResponse<StudentExamResultDto>.Ok(result.Data!, result.Message));
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var result = await _StudentExamResultService.DeleteAsync(id);

            if (!result.Success)
                return NotFound(ApiResponse<string>.Fail(result.Message!));

            return Ok(ApiResponse<string>.Ok("StudentExamResult deleted successfully."));
        }
    }
}
