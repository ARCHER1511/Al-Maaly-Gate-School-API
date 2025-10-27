using Application.DTOs.QuestionDTOs;
using Application.Interfaces;
using Domain.Wrappers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Al_Maaly_Gate_School.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [AllowAnonymous]
    public class QuestionController : ControllerBase
    {
        private readonly IQuestionService _questionService;

        public QuestionController(IQuestionService questionService)
        {
            _questionService = questionService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _questionService.GetAllAsync();
            if (!result.Success) return NotFound(ApiResponse<IEnumerable<QuestionViewDto>>.Fail(result.Message!));
            return Ok(ApiResponse<IEnumerable<QuestionViewDto>>.Ok(result.Data!, result.Message));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var result = await _questionService.GetByIdAsync(id);
            if (!result.Success) return NotFound(ApiResponse<QuestionViewDto>.Fail(result.Message!));
            return Ok(ApiResponse<QuestionViewDto>.Ok(result.Data!, result.Message));
        }

        [HttpPost("{teacherId}")]
        public async Task<IActionResult> Create(string teacherId, [FromBody] CreateQuestionDto dto)
        {
            var result = await _questionService.CreateQuestionAsync(teacherId, dto);
            if (!result.Success) return BadRequest(ApiResponse<QuestionViewDto>.Fail(result.Message!));
            return Ok(ApiResponse<QuestionViewDto>.Ok(result.Data!, result.Message));
        }
    }
}
