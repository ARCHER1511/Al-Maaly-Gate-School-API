using Al_Maaly_Gate_School.ControllerResponseHandler;
using Application.DTOs.ExamDTOS;
using Application.Interfaces;
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
        public async Task<IActionResult> GetAll() => await this.HandleAsync(() => _examService.GetAllAsync());

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(string id) => await this.HandleAsync(() => _examService.GetByIdAsync(id));

        [HttpGet("teacher/{teacherId}")]
        public async Task<IActionResult> GetByTeacher(string teacherId) => await this.HandleAsync(() => _examService.GetByTeacherAsync(teacherId));

        //[HttpPost]
        //public async Task<IActionResult> Create([FromBody] CreateExamDto dto) => Ok(await _examService.CreateExamForTeacherAsync(dto.TeacherId, dto));

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string id, [FromBody] UpdateExamDto dto) => await this.HandleAsync(() => _examService.UpdateAsync(id, dto));

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id) => await this.HandleAsync(() => _examService.DeleteAsync(id));
        [HttpPost("with-questions")]
        public async Task<IActionResult> CreateWithQuestions([FromBody] CreateExamWithQuestionsDto dto)
    => await this.HandleAsync(() => _examService.CreateExamWithQuestionsAsync(dto));

        [HttpGet("{id}/details")]
        public async Task<IActionResult> GetExamDetails(string id)
            => await this.HandleAsync(() => _examService.GetExamWithQuestionsAsync(id));
    }
}
