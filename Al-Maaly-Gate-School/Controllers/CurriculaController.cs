using Application.DTOs.CurriculumDTOs;
using Application.Interfaces;
using Domain.Wrappers;
using Microsoft.AspNetCore.Mvc;

namespace Al_Maaly_Gate_School.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CurriculaController : ControllerBase
    {
        private readonly ICurriculumService _curriculumService;

        public CurriculaController(ICurriculumService curriculumService)
        {
            _curriculumService = curriculumService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _curriculumService.GetAllAsync();
            if (!result.Success)
                return NotFound(ApiResponse<IEnumerable<CurriculumDto>>.Fail(result.Message!));

            return Ok(ApiResponse<IEnumerable<CurriculumDto>>.Ok(result.Data!, result.Message));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            var result = await _curriculumService.GetByIdAsync(id);
            if (!result.Success)
                return NotFound(ApiResponse<CurriculumDto>.Fail(result.Message!));

            return Ok(ApiResponse<CurriculumDto>.Ok(result.Data!, result.Message));
        }

        [HttpGet("{id}/details")]
        public async Task<IActionResult> GetWithDetails(string id)
        {
            var result = await _curriculumService.GetWithDetailsAsync(id);
            if (!result.Success)
                return NotFound(ApiResponse<CurriculumDetailsDto>.Fail(result.Message!));

            return Ok(ApiResponse<CurriculumDetailsDto>.Ok(result.Data!, result.Message));
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateCurriculumDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ApiResponse<CurriculumDto>.Fail("Invalid curriculum data."));

            var result = await _curriculumService.CreateAsync(dto);
            if (!result.Success)
                return BadRequest(ApiResponse<CurriculumDto>.Fail(result.Message!));

            return CreatedAtAction(nameof(GetById), new { id = result.Data!.Id },
                ApiResponse<CurriculumDto>.Ok(result.Data, result.Message));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string id, [FromBody] UpdateCurriculumDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ApiResponse<CurriculumDto>.Fail("Invalid curriculum data."));

            var result = await _curriculumService.UpdateAsync(id, dto);
            if (!result.Success)
                return NotFound(ApiResponse<CurriculumDto>.Fail(result.Message!));

            return Ok(ApiResponse<CurriculumDto>.Ok(result.Data!, result.Message));
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var result = await _curriculumService.DeleteAsync(id);
            if (!result.Success)
                return NotFound(ApiResponse<bool>.Fail(result.Message!));

            return Ok(ApiResponse<bool>.Ok(true, "Curriculum deleted successfully"));
        }

        [HttpGet("exists/{id}")]
        public async Task<IActionResult> Exists(string id)
        {
            var result = await _curriculumService.ExistsAsync(id);
            return Ok(ApiResponse<bool>.Ok(result.Data!, result.Message));
        }

        [HttpGet("{id}/has-students")]
        public async Task<IActionResult> HasStudents(string id)
        {
            var result = await _curriculumService.HasStudentsAsync(id);
            if (!result.Success)
                return NotFound(ApiResponse<bool>.Fail(result.Message!));

            return Ok(ApiResponse<bool>.Ok(result.Data!, result.Message));
        }

        [HttpGet("{id}/has-teachers")]
        public async Task<IActionResult> HasTeachers(string id)
        {
            var result = await _curriculumService.HasTeachersAsync(id);
            if (!result.Success)
                return NotFound(ApiResponse<bool>.Fail(result.Message!));

            return Ok(ApiResponse<bool>.Ok(result.Data!, result.Message));
        }

        [HttpGet("count")]
        public async Task<IActionResult> GetCount()
        {
            var result = await _curriculumService.GetCountAsync();
            if (!result.Success)
                return BadRequest(ApiResponse<int>.Fail(result.Message!));

            return Ok(ApiResponse<int>.Ok(result.Data!, result.Message));
        }
    }
}