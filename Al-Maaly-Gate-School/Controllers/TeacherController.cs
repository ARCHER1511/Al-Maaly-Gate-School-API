using Application.DTOs.ExamDTOS;
using Application.DTOs.TeacherDTOs;
using Application.Interfaces;
using Domain.Wrappers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Al_Maaly_Gate_School.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [AllowAnonymous]
    public class TeacherController : ControllerBase
    {
        private readonly ITeacherService _teacherService;
        private readonly IExamService _examService;

        public TeacherController(ITeacherService teacherService, IExamService examService)
        {
            _teacherService = teacherService;
            _examService = examService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _teacherService.GetAllAsync();
            if (!result.Success)
                return NotFound(ApiResponse<IEnumerable<TeacherViewDto>>.Fail(result.Message!));
            return Ok(ApiResponse<IEnumerable<TeacherViewDto>>.Ok(result.Data!, result.Message));
        }

        // Create exam by teacher
        [HttpPost("{teacherId}/exams")]
        public async Task<IActionResult> CreateExam(string teacherId, [FromBody] CreateExamDto dto)
        {
            var result = await _examService.CreateExamForTeacherAsync(teacherId, dto);
            if (!result.Success)
                return BadRequest(ApiResponse<ExamViewDto>.Fail(result.Message!));
            return Ok(ApiResponse<ExamViewDto>.Ok(result.Data!, result.Message));
        }

        // Assign questions to exam
        [HttpPost("{teacherId}/exams/{examId}/questions")]
        public async Task<IActionResult> AssignQuestions(string teacherId, int examId, [FromBody] IEnumerable<int> questionIds)
        {
            var result = await _examService.AssignQuestionsAsync(teacherId, examId, questionIds);
            if (!result.Success)
                return BadRequest(ApiResponse<bool>.Fail(result.Message!));
            return Ok(ApiResponse<bool>.Ok(result.Data!, result.Message));
        }

        // Get teacher exams
        [HttpGet("{teacherId}/exams")]
        public async Task<IActionResult> GetTeacherExams(string teacherId)
        {
            var result = await _examService.GetByTeacherAsync(teacherId);
            if (!result.Success)
                return NotFound(ApiResponse<IEnumerable<ExamViewDto>>.Fail(result.Message!));
            return Ok(ApiResponse<IEnumerable<ExamViewDto>>.Ok(result.Data!, result.Message));
        }
    }
}
