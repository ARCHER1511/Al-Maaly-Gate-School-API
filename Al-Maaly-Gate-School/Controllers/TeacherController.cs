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
        public TeacherController(ITeacherService teacherService) 
        {
            _teacherService = teacherService;
        }
        [HttpGet]
        public async Task<IActionResult> GetAll() 
        {
            var result = await _teacherService.GetAllAsync();
            if (!result.Success) 
                return NotFound(ApiResponse<IEnumerable<TeacherViewDto>>.Fail(result.Message!));
            return Ok(ApiResponse<IEnumerable<TeacherViewDto>>.Ok(result.Data!,result.Message));
        }
    }
}
