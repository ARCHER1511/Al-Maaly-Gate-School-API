using Application.DTOs.ExamDTOS;
using Application.Interfaces;
using Domain.Wrappers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Al_Maaly_Gate_School.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [AllowAnonymous]
    public class ExamController : ControllerBase
    {
        private readonly IExamService _examService;

        public ExamController(IExamService examService)
        {
            _examService = examService;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var result = await _examService.GetByIdAsync(id);
            if (!result.Success) return NotFound(ApiResponse<object>.Fail(result.Message!));
            return Ok(ApiResponse<object>.Ok(result.Data!, result.Message));
        }

        [HttpPost("{teacherId}")]
        public async Task<IActionResult> Create(string teacherId, [FromBody] CreateExamDto dto)
        {
            var result = await _examService.CreateExamForTeacherAsync(teacherId, dto);
            if (!result.Success) return BadRequest(ApiResponse<object>.Fail(result.Message!));
            return Ok(ApiResponse<object>.Ok(result.Data!, result.Message));
        }

        [HttpPost("{teacherId}/{examId}/assign")]
        public async Task<IActionResult> AssignQuestions(string teacherId, int examId, [FromBody] IEnumerable<int> questionIds)
        {
            var result = await _examService.AssignQuestionsAsync(teacherId, examId, questionIds);
            if (!result.Success) return BadRequest(ApiResponse<bool>.Fail(result.Message!));
            return Ok(ApiResponse<bool>.Ok(result.Data!, result.Message));
        }

        [HttpDelete("{examId}")]
        public async Task<IActionResult> Delete(int examId)
        {
            var result = await _examService.DeleteAsync(examId);
            if (!result.Success) return NotFound(ApiResponse<bool>.Fail(result.Message!));
            return Ok(ApiResponse<bool>.Ok(result.Data!, result.Message));
        }
    }
}
