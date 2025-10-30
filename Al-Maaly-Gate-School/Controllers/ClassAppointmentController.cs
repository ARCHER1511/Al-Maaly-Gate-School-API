using Application.DTOs.ClassAppointmentDTOs;
using Application.Interfaces;
using Domain.Wrappers;
using Microsoft.AspNetCore.Mvc;

namespace Al_Maaly_Gate_School.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClassAppointmentController : ControllerBase
    {
        private readonly IClassAppointmentService _appointmentService;

        public ClassAppointmentController(IClassAppointmentService appointmentService)
        {
            _appointmentService = appointmentService;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id) => Ok(await _appointmentService.GetByIdAsync(id));

        [HttpGet("teacher/{teacherId}")]
        public async Task<IActionResult> GetByTeacher(string teacherId) => Ok(await _appointmentService.GetByTeacherAsync(teacherId));

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateClassAppointmentDto dto) => Ok(await _appointmentService.CreateAsync(dto));

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string id, [FromBody] UpdateClassAppointmentDto dto) => Ok(await _appointmentService.UpdateAsync(id, dto));

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id) => Ok(await _appointmentService.DeleteAsync(id));
    }
}
