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
            var response = await _questionService.GetAllAsync();
            if(!response.Success)
                return NotFound(ApiResponse<IEnumerable<QuestionViewDto>>.Fail(response.Message!));
            return Ok(ApiResponse<IEnumerable<QuestionViewDto>>.Ok(response.Data!,response.Message));
        }



    }
}
