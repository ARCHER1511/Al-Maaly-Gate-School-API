using Application.DTOs.AdminDTOs;
using Application.DTOs.AppointmentsDTOs;
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
    public class AppointmentController : ControllerBase
    {
        private readonly IAppointmentService _appointmentService;

        public AppointmentController(IAppointmentService appointmentService)
        {
            _appointmentService = appointmentService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _appointmentService.GetAllAsync();
            if (!result.Success)
                return NotFound(ApiResponse<IEnumerable<ViewAppointmentDto>>.Fail(result.Message!));
            return Ok(ApiResponse<IEnumerable<ViewAppointmentDto>>.Ok(result.Data!, result.Message));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            var result = await _appointmentService.GetByIdAsync(id);
            if (!result.Success)
                return NotFound(ApiResponse<ViewAppointmentDto>.Fail(result.Message!));

            return Ok(ApiResponse<ViewAppointmentDto>.Ok(result.Data!, result.Message));
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] AppointmentDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ApiResponse<AppointmentDto>.Fail("Invalid Appointment data."));

            var result = await _appointmentService.CreateAsync(dto);

            if (!result.Success)
                return BadRequest(ApiResponse<ClassAppointment>.Fail(result.Message!));

            return Ok(ApiResponse<AppointmentDto>.Ok(result.Data!, result.Message));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string id, [FromBody] AppointmentDto dto)
        {
            if (id != dto.Id)
                return BadRequest(ApiResponse<AppointmentDto>.Fail("ID in URL does not match ID in body."));

            var exists = await _appointmentService.GetByIdAsync(id);
            if (!exists.Success)
                return NotFound(ApiResponse<AppointmentDto>.Fail(exists.Message!));

            var result = await _appointmentService.UpdateAsync(dto);

            if (!result.Success)
                return BadRequest(ApiResponse<AppointmentDto>.Fail(result.Message!));

            return Ok(ApiResponse<AppointmentDto>.Ok(result.Data!, result.Message));
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
