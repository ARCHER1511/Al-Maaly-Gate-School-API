using Application.DTOs.AdminDTOs;
using Application.DTOs.AppointmentsDTOs;
using Application.DTOs.ClassAppointmentsDTOs;
using Application.DTOs.ClassDTOs;
using Application.Interfaces;
using Domain.Entities;
using Domain.Wrappers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Al_Maaly_Gate_School.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [AllowAnonymous]
    public class ClassAppointmentController : ControllerBase
    {
        private readonly IClassAppointmentService _appointmentService;

        public ClassAppointmentController(IClassAppointmentService appointmentService)
        {
            _appointmentService = appointmentService;
        }
        [HttpGet("teacher/{teacherId}")]
        public async Task<IActionResult> GetAppointmentsByTeacher(string teacherId)
        {
            var result = await _appointmentService.GetAppointmentsByTeacherAsync(teacherId);
            if (!result.Success)
                return NotFound(ApiResponse<IEnumerable<ClassAppointmentDto>>.Fail(result.Message!));
            return Ok(ApiResponse<IEnumerable<ClassAppointmentDto>>.Ok(result.Data!, result.Message));
        }
        [HttpGet("student/{ClassId}")]
        public async Task<IActionResult> GetAppointmentsForStudentByClassIdAsync(string ClassId)
        {
            var result = await _appointmentService.GetAppointmentsForStudentByClassIdAsync(ClassId);
            if (!result.Success)
                return NotFound(ApiResponse<IEnumerable<StudentClassAppointmentDto>>.Fail(result.Message!));
            return Ok(ApiResponse<IEnumerable<StudentClassAppointmentDto>>.Ok(result.Data!, result.Message));
        }
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _appointmentService.GetAllAsync();
            if (!result.Success)
                return NotFound(ApiResponse<IEnumerable<ClassAppointmentDto>>.Fail(result.Message!));
            return Ok(ApiResponse<IEnumerable<ClassAppointmentDto>>.Ok(result.Data!, result.Message));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            var result = await _appointmentService.GetByIdAsync(id);
            if (!result.Success)
                return NotFound(ApiResponse<ClassAppointmentDto>.Fail(result.Message!));

            return Ok(ApiResponse<ClassAppointmentDto>.Ok(result.Data!, result.Message));
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] ClassAppointmentDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ApiResponse<ClassAppointmentDto>.Fail("Invalid Appointment data."));

            var result = await _appointmentService.CreateAsync(dto);

            if (!result.Success)
                return BadRequest(ApiResponse<ClassAppointment>.Fail(result.Message!));

            return Ok(ApiResponse<ClassAppointmentDto>.Ok(result.Data!, result.Message));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string id, [FromBody] ClassAppointmentDto dto)
        {
            if (id != dto.Id)
                return BadRequest(ApiResponse<ClassAppointmentDto>.Fail("ID in URL does not match ID in body."));

            var exists = await _appointmentService.GetByIdAsync(id);
            if (!exists.Success)
                return NotFound(ApiResponse<ClassAppointmentDto>.Fail(exists.Message!));

            var result = await _appointmentService.UpdateAsync(dto);

            if (!result.Success)
                return BadRequest(ApiResponse<ClassAppointmentDto>.Fail(result.Message!));

            return Ok(ApiResponse<ClassAppointmentDto>.Ok(result.Data!, result.Message));
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var result = await _appointmentService.DeleteAsync(id);

            if (!result.Success)
                return NotFound(ApiResponse<string>.Fail(result.Message!));

            return Ok(ApiResponse<string>.Ok("Appointment deleted successfully."));
        }
    }
}
