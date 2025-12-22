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
            if (!result.Success)
                return NotFound(ApiResponse<IEnumerable<ClassViewDto>>.Fail(result.Message!));

            return Ok(ApiResponse<IEnumerable<ClassViewDto>>.Ok(result.Data!, result.Message));
        }

        [HttpGet("{id}")] // CHANGED: Removed "class/" prefix for consistency
        public async Task<IActionResult> GetById(string id)
        {
            var result = await _classService.GetByIdAsync(id);
            if (!result.Success)
                return NotFound(ApiResponse<ClassViewDto>.Fail(result.Message!));

            return Ok(ApiResponse<ClassViewDto>.Ok(result.Data!, result.Message));
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateClassDto dto) // Changed to CreateClassDto
        {
            if (!ModelState.IsValid)
                return BadRequest(ApiResponse<CreateClassDto>.Fail("Invalid class data."));

            var result = await _classService.CreateAsync(dto);

            if (!result.Success)
                return BadRequest(ApiResponse<ClassDto>.Fail(result.Message!));

            return Ok(ApiResponse<ClassDto>.Ok(result.Data!, result.Message));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string id, [FromBody] UpdateClassDto dto) // Changed to UpdateClassDto
        {
            if (id != dto.Id)
                return BadRequest(ApiResponse<UpdateClassDto>.Fail("ID in URL does not match ID in body."));

            var exists = await _classService.GetByIdAsync(id);
            if (!exists.Success)
                return NotFound(ApiResponse<UpdateClassDto>.Fail(exists.Message!));

            var result = await _classService.UpdateAsync(dto);

            if (!result.Success)
                return BadRequest(ApiResponse<ClassDto>.Fail(result.Message!));

            return Ok(ApiResponse<ClassDto>.Ok(result.Data!, result.Message));
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var result = await _classService.DeleteAsync(id);

            if (!result.Success)
                return NotFound(ApiResponse<bool>.Fail(result.Message!)); // CHANGED: Return bool instead of string

            return Ok(ApiResponse<bool>.Ok(true, "Class deleted successfully.")); // CHANGED: Return bool with message
        }

        [HttpGet("with-teachers")]
        public async Task<IActionResult> GetAllWithTeachers()
        {
            var result = await _classService.GetAllWithTeachersAsync();
            if (!result.Success)
                return NotFound(ApiResponse<IEnumerable<ClassViewDto>>.Fail(result.Message!));

            return Ok(ApiResponse<IEnumerable<ClassViewDto>>.Ok(result.Data!, result.Message));
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

        // Add to ClassController
        [HttpGet("{classId}/statistics")]
        public async Task<IActionResult> GetClassStatistics(string classId)
        {
            var result = await _classService.GetClassStatisticsAsync(classId);
            if (!result.Success)
                return NotFound(ApiResponse<ClassStatisticsDto>.Fail(result.Message!));

            return Ok(ApiResponse<ClassStatisticsDto>.Ok(result.Data!, result.Message));
        }

        [HttpGet("{classId}/export")]
        public async Task<IActionResult> ExportClassData(string classId)
        {
            var result = await _classService.ExportClassDataAsync(classId);
            if (!result.Success)
                return BadRequest(ApiResponse<byte[]>.Fail(result.Message!));

            return File(result.Data!, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                $"class_{classId}_data_{DateTime.Now:yyyyMMdd}.xlsx");
        }

        [HttpGet("export-all")]
        public async Task<IActionResult> ExportAllClasses()
        {
            var result = await _classService.ExportAllClassesAsync();
            if (!result.Success)
                return BadRequest(ApiResponse<byte[]>.Fail(result.Message!));

            return File(result.Data!, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                $"all_classes_data_{DateTime.Now:yyyyMMdd}.xlsx");
        }

        [HttpGet("count")]
        public async Task<IActionResult> GetClassCount()
        {
            var result = await _classService.GetClassCountAsync();
            if (!result.Success)
                return BadRequest(ApiResponse<int>.Fail(result.Message!));

            return Ok(ApiResponse<int>.Ok(result.Data!, result.Message));
        }
    }
}