using Application.DTOs.DegreesDTOs;
using Application.Interfaces;
using Domain.Entities;
using Domain.Wrappers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Al_Maaly_Gate_School.Controllers
{
    [AllowAnonymous]
    [Route("api/[controller]")]
    [ApiController]
    public class DegreeController : ControllerBase
    {
        private readonly IDegreeService _degreeService;

        public DegreeController(IDegreeService degreeService)
        {
            _degreeService = degreeService;
        }

        [HttpPost("add")]
        [AllowAnonymous]
        public async Task<IActionResult> AddDegrees([FromBody] AddDegreesDto dto)
        {
            var result = await _degreeService.AddDegreesAsync(dto.StudentId, dto.Degrees);
            if (!result.Success)
                return BadRequest(result.Message);
            return Ok(ApiResponse<string>.Ok(result.Data!, result.Message));
        }

        // Get Student degrees
        [HttpGet("student/{studentId}")]
        public async Task<IActionResult> GetStudentDegrees(string studentId)
        {
            var result = await _degreeService.GetStudentDegreesAsync(studentId);
            if(!result.Success || result == null)
                return BadRequest(result!.Message);
            return Ok(ApiResponse<StudentDegreesDto>.Ok(result.Data!,result.Message));
        }

        // Get all
        [HttpGet("all")]
        public async Task<IActionResult> GetAllStudentsDegrees()
        {
            var result = await _degreeService.GetAllStudentsDegreesAsync();
            if (!result.Success || result == null)
                return BadRequest(result!.Message);
            return Ok(ApiResponse<List<StudentDegreesDto>>.Ok(result.Data!,result.Message));
        }
    }
}
