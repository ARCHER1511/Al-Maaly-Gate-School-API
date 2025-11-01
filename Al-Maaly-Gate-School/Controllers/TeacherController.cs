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

        public TeacherController(ITeacherService teacherService)
        {
            _teacherService = teacherService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll() => Ok(await _teacherService.GetAllAsync());

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id) => Ok(await _teacherService.GetByIdAsync(id));

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateTeacherDto dto) => Ok(await _teacherService.CreateAsync(dto));

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string id, [FromBody] UpdateTeacherDto dto) => Ok(await _teacherService.UpdateAsync(id, dto));

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id) => Ok(await _teacherService.DeleteAsync(id));
    }
}
