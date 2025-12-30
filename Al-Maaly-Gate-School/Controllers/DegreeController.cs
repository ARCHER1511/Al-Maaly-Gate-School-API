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

        // Add example endpoints to demonstrate both methods
        [HttpPost("example/simple")]
        public async Task<IActionResult> AddSimpleDegreeExample()
        {
            // Example: Adding Mid1 with just total score
            var exampleDto = new AddDegreesDto
            {
                StudentId = "student-123",
                Degrees = new List<DegreeInput>
                {
                    new DegreeInput
                    {
                        SubjectId = "math-123",
                        DegreeType = DegreeType.MidTerm1,
                        Score = 17,  // Just total score
                        MaxScore = 20
                    }
                }
            };

            return Ok(new
            {
                Message = "Example: Simple total score entry",
                Example = exampleDto,
                Note = "Use this when you don't need component breakdown"
            });
        }

        [HttpPost("example/with-components")]
        public async Task<IActionResult> AddComponentDegreeExample()
        {
            // Example: Adding Mid1 with component breakdown
            var exampleDto = new AddDegreesDto
            {
                StudentId = "student-123",
                Degrees = new List<DegreeInput>
                {
                    new DegreeInput
                    {
                        SubjectId = "math-123",
                        DegreeType = DegreeType.MidTerm1,
                        Components = new List<DegreeComponentInput>
                        {
                            new DegreeComponentInput
                            {
                                ComponentTypeId = "oral-123",
                                ComponentName = "Oral Exam",
                                Score = 5,
                                MaxScore = 5
                            },
                            new DegreeComponentInput
                            {
                                ComponentTypeId = "exam-123",
                                ComponentName = "Written Exam",
                                Score = 8,
                                MaxScore = 10
                            },
                            new DegreeComponentInput
                            {
                                ComponentTypeId = "homework-123",
                                ComponentName = "Homework",
                                Score = 4,
                                MaxScore = 5
                            }
                        }
                        // No total scores - they will be calculated
                    }
                }
            };

            return Ok(new
            {
                Message = "Example: Component breakdown entry",
                Example = exampleDto,
                Note = "Use this when you want detailed component scores"
            });
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

        [HttpPut("update/{degreeId}")]
        public async Task<IActionResult> UpdateDegree(string degreeId, [FromBody] DegreeInput input)
        {
            var result = await _degreeService.UpdateDegreeAsync(degreeId, input);
            if (!result.Success)
                return BadRequest(result.Message);
            return Ok(ApiResponse<string>.Ok(result.Data!, result.Message));
        }

        [HttpPost("convert/{degreeId}")]
        public async Task<IActionResult> ConvertToComponents(string degreeId, [FromBody] List<DegreeComponentInput> components)
        {
            var result = await _degreeService.ConvertToComponentsAsync(degreeId, components);
            if (!result.Success)
                return BadRequest(result.Message);
            return Ok(ApiResponse<string>.Ok(result.Data!, result.Message));
        }
    }
}
