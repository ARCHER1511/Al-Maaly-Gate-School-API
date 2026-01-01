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
    public class StudentExamAnswerController : ControllerBase
    {
        private readonly IStudentExamAnswerService _studentExamAnswerService;
        public StudentExamAnswerController(IStudentExamAnswerService studentExamAnswerService)
        {
            _studentExamAnswerService = studentExamAnswerService;
        }

        [HttpGet("studentAnswerWithCorrection/{studentId}/{examId}")]
        public async Task<ActionResult<ApiResponse<IEnumerable<StudentAnswerWithQuestionDto>>>> GetStudentAnswersWithQuestions(string examId, string studentId)
        {
            var result = await _studentExamAnswerService.GetStudentAnswersWithQuestions(examId, studentId);
            if (!result.Success)
                return NotFound(ApiResponse<IEnumerable<StudentAnswerWithQuestionDto>>.Fail(result.Message!));

            return Ok(ApiResponse<IEnumerable<StudentAnswerWithQuestionDto>>.Ok(result.Data!, result.Message));
        }

        [HttpGet("studentExams/{classId}")]
        public async Task<IActionResult> GetExams(string classId)
        {
            var result = await _studentExamAnswerService.GetExamsForStudentByClassId(classId);

            if (!result.Success)
            {
                return BadRequest(ApiResponse<IEnumerable<GetStudentExamsDto>>.Fail(result.Message!));
            }

            var data = result.Data ?? Enumerable.Empty<GetStudentExamsDto>();
            var message = result.Data?.Any() == true
                ? result.Message
                : "No exams found";

            return Ok(ApiResponse<IEnumerable<GetStudentExamsDto>>.Ok(data, message));
        }

        [HttpPost("SubmitExam")]
        public async Task<IActionResult> SubmitExam([FromBody] StudentExamSubmissionDto submission)
        {
            if (!ModelState.IsValid)
                return BadRequest(ApiResponse<StudentExamSubmissionDto>.Fail("Invalid submission data."));

            var result = await _studentExamAnswerService.SubmitExamAsync(submission);

            if (!result.Success)
                return BadRequest(ApiResponse<List<StudentExamAnswerDto>>.Fail(result.Message!));

            return Ok(ApiResponse<List<StudentExamAnswerDto>>.Ok(result.Data!, result.Message));
        }

        [HttpGet("ExamQuestions/{ExamId}")]
        public async Task<IActionResult> GetExamQuestions(string ExamId)
        {
            var result = await _studentExamAnswerService.GetExamQuestions(ExamId);
            if (!result.Success)
                return NotFound(ApiResponse<ExamQuestionsDto>.Fail(result.Message!));

            return Ok(ApiResponse<ExamQuestionsDto>.Ok(result.Data!, result.Message));
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

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string id, [FromBody] StudentExamAnswerDto dto)
        {
            if (id != dto.Id)
                return BadRequest(ApiResponse<StudentExamAnswerDto>.Fail("ID in URL does not match ID in body."));

            var exists = await _studentExamAnswerService.GetByIdAsync(id);
            if (!exists.Success)
                return NotFound(ApiResponse<StudentExamAnswerDto>.Fail(exists.Message!));

            var result = await _studentExamAnswerService.UpdateAsync(dto);

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
