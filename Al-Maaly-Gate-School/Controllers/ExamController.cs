using Application.DTOs.ExamDTOS;
using Application.Interfaces;
using Application.Services;
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
        [HttpGet]
        public async Task<IActionResult> GetAll() => Ok(await _examService.GetAllAsync());

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(string id) => Ok(await _examService.GetByIdAsync(id));

        [HttpGet("teacher/{teacherId}")]
        public async Task<IActionResult> GetByTeacher(string teacherId) => Ok(await _examService.GetByTeacherAsync(teacherId));

        //[HttpPost]
        //public async Task<IActionResult> Create([FromBody] CreateExamDto dto) => Ok(await _examService.CreateExamForTeacherAsync(dto.TeacherId, dto));

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string id, [FromBody] UpdateExamDto dto) => Ok(await _examService.UpdateAsync(id, dto));

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id) => Ok(await _examService.DeleteAsync(id));
        [HttpPost("with-questions")]
        public async Task<IActionResult> CreateWithQuestions([FromBody] CreateExamWithQuestionsDto dto)
    => Ok(await _examService.CreateExamWithQuestionsAsync(dto));

        [HttpGet("{id}/details")]
        public async Task<IActionResult> GetExamDetails(string id)
            => Ok(await _examService.GetExamWithQuestionsAsync(id));
    }
}
