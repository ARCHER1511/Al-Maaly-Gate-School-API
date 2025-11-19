using Application.DTOs.ClassDTOs;
using Application.Interfaces;
using Domain.Entities;
using Domain.Wrappers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Al_Maaly_Gate_School.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [AllowAnonymous]
    public class ClassController : ControllerBase
    {
        private readonly IClassService _classService;
        public ClassController(IClassService classService)
        {
            _classService = classService;
        }
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _classService.GetAllAsync();
            if (!result.Success) return NotFound(ApiResponse<IEnumerable<ClassViewDto>>.Fail(result.Message!));
            return Ok(ApiResponse<IEnumerable<ClassViewDto>>.Ok(result.Data!, result.Message));
        }
        [HttpGet("class/{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            var result = await _classService.GetByIdAsync(id);
            if (!result.Success)
                return NotFound(ApiResponse<ClassViewDto>.Fail(result.Message!));

            return Ok(ApiResponse<ClassViewDto>.Ok(result.Data!, result.Message));
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] ClassDto classDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ApiResponse<ClassDto>.Fail("Invalid class data."));

            var result = await _classService.CreateAsync(classDto);

            if (!result.Success)
                return BadRequest(ApiResponse<ClassDto>.Fail(result.Message!));

            return Ok(ApiResponse<ClassDto>.Ok(result.Data!, result.Message));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string id, [FromBody] ClassDto ClassDto)
        {
            if (id != ClassDto.Id)
                return BadRequest(ApiResponse<ClassDto>.Fail("ID in URL does not match ID in body."));

            var exists = await _classService.GetByIdAsync(id);
            if (!exists.Success)
                return NotFound(ApiResponse<ClassDto>.Fail(exists.Message!));

            var result = await _classService.UpdateAsync(ClassDto);

            if (!result.Success)
                return BadRequest(ApiResponse<ClassDto>.Fail(result.Message!));

            return Ok(ApiResponse<ClassDto>.Ok(result.Data!, result.Message));
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var result = await _classService.DeleteAsync(id);

            if (!result.Success)
                return NotFound(ApiResponse<string>.Fail(result.Message!));

            return Ok(ApiResponse<string>.Ok("class deleted successfully."));
        }

        [HttpGet("{classId}/students")]
        public async Task<IActionResult> GetStudents(string classId)
        {
            var result = await _classService.GetStudentsByClassIdAsync(classId);

            if (!result.Success)
                return NotFound(ApiResponse<List<Student>>.Fail(result.Message!));

            return Ok(ApiResponse<List<Student>>.Ok(result.Data!));
        }

        [HttpGet("{classId}/subjects")]
        public async Task<IActionResult> GetSubjects(string classId)
        {
            var result = await _classService.GetSubjectsByClassIdAsync(classId);

            if (!result.Success)
                return NotFound(ApiResponse<List<Subject>>.Fail(result.Message!));

            return Ok(ApiResponse<List<Subject>>.Ok(result.Data!));
        }
    }
}
