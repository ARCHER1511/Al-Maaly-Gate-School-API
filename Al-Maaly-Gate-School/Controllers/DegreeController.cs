using Application.DTOs.DegreesDTOs;
using Application.Interfaces;
using Domain.Entities;
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

        // Add Degrees
        [HttpPost("add")]
        [AllowAnonymous]
        public async Task<IActionResult> AddDegrees([FromBody] AddDegreesDto dto)
        {
            var degrees = dto.Degrees.Select(d => new Degree
            {
                SubjectId = d.SubjectId,
                Score = d.Score,
                MaxScore = d.MaxScore,
                DegreeType = d.DegreeType
            }).ToList();

            var result = await _degreeService.AddDegreesAsync(dto.StudentId, degrees);
            return Ok(result);
        }

        // Get Student degrees
        [HttpGet("student/{studentId}")]
        public async Task<IActionResult> GetStudentDegrees(string studentId)
        {
            var result = await _degreeService.GetStudentDegreesAsync(studentId);
            return Ok(result);
        }

        // Get all
        [HttpGet("all")]
        public async Task<IActionResult> GetAllStudentsDegrees()
        {
            var result = await _degreeService.GetAllStudentsDegreesAsync();
            return Ok(result);
        }
    }
}
