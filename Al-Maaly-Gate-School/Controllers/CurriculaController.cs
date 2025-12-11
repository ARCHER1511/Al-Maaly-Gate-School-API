using Application.DTOs.CurriculumDTOs;
using Application.Interfaces;
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
        public async Task<ActionResult<IEnumerable<CurriculumDto>>> GetAll()
        {
            var curricula = await _curriculumService.GetAllAsync();
            return Ok(curricula);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<CurriculumDto>> GetById(string id)
        {
            var curriculum = await _curriculumService.GetByIdAsync(id);
            if (curriculum == null)
                return NotFound(new { message = $"Curriculum with ID '{id}' not found." });

            return Ok(curriculum);
        }

        [HttpGet("{id}/details")]
        public async Task<ActionResult<CurriculumDetailsDto>> GetWithDetails(string id)
        {
            var curriculum = await _curriculumService.GetWithDetailsAsync(id);
            if (curriculum == null)
                return NotFound(new { message = $"Curriculum with ID '{id}' not found." });

            return Ok(curriculum);
        }

        [HttpPost]
        public async Task<ActionResult<CurriculumDto>> Create([FromBody] CreateCurriculumDto dto)
        {
            try
            {
                var curriculum = await _curriculumService.CreateAsync(dto);
                return CreatedAtAction(nameof(GetById), new { id = curriculum.Id }, curriculum);
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { message = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<CurriculumDto>> Update(string id, [FromBody] UpdateCurriculumDto dto)
        {
            try
            {
                var curriculum = await _curriculumService.UpdateAsync(id, dto);
                if (curriculum == null)
                    return NotFound(new { message = $"Curriculum with ID '{id}' not found." });

                return Ok(curriculum);
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { message = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            try
            {
                var deleted = await _curriculumService.DeleteAsync(id);
                if (!deleted)
                    return NotFound(new { message = $"Curriculum with ID '{id}' not found." });

                return NoContent();
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("exists/{id}")]
        public async Task<ActionResult<bool>> Exists(string id)
        {
            var exists = await _curriculumService.ExistsAsync(id);
            return Ok(exists);
        }

        [HttpGet("{id}/has-students")]
        public async Task<ActionResult<bool>> HasStudents(string id)
        {
            var hasStudents = await _curriculumService.HasStudentsAsync(id);
            return Ok(hasStudents);
        }

        [HttpGet("{id}/has-teachers")]
        public async Task<ActionResult<bool>> HasTeachers(string id)
        {
            var hasTeachers = await _curriculumService.HasTeachersAsync(id);
            return Ok(hasTeachers);
        }
    }
}
