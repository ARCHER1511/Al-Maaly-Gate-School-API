using Application.DTOs.SubjectDTOs;
using Application.Interfaces;
using Domain.Wrappers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Al_Maaly_Gate_School.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [AllowAnonymous]
    public class SubjectController : ControllerBase
    {
        private readonly ISubjectService _subjectService;

        public SubjectController(ISubjectService subjectService)
        {
            _subjectService = subjectService;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] SubjectCreateDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ApiResponse<string>.Fail("Invalid subject data."));

            var result = await _subjectService.Create(dto);

            return result.Success
                ? Ok(ApiResponse<SubjectViewDto>.Ok(result.Data!, result.Message))
                : BadRequest(ApiResponse<SubjectViewDto>.Fail(result.Message!));
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _subjectService.GetAll();

            return result.Success
                ? Ok(ApiResponse<IEnumerable<SubjectViewDto>>.Ok(result.Data!, result.Message))
                : NotFound(ApiResponse<IEnumerable<SubjectViewDto>>.Fail(result.Message!));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            var result = await _subjectService.GetById(id);

            return result.Success
                ? Ok(ApiResponse<SubjectViewDto>.Ok(result.Data!, result.Message))
                : NotFound(ApiResponse<SubjectViewDto>.Fail(result.Message!));
        }

        [HttpGet("{id}/with-components")]
        public async Task<IActionResult> GetWithComponents(string id)
        {
            var result = await _subjectService.GetWithComponents(id);

            return result.Success
                ? Ok(ApiResponse<SubjectViewDto>.Ok(result.Data!, result.Message))
                : NotFound(ApiResponse<SubjectViewDto>.Fail(result.Message!));
        }

        [HttpGet("with-components")]
        public async Task<IActionResult> GetSubjectsWithComponentTypes()
        {
            var result = await _subjectService.GetSubjectsWithComponentTypes();

            return result.Success
                ? Ok(ApiResponse<IEnumerable<SubjectViewDto>>.Ok(result.Data!, result.Message))
                : NotFound(ApiResponse<IEnumerable<SubjectViewDto>>.Fail(result.Message!));
        }

        [HttpGet("{id}/has-components")]
        public async Task<IActionResult> HasComponentTypes(string id)
        {
            var result = await _subjectService.HasComponentTypes(id);

            return result.Success
                ? Ok(ApiResponse<bool>.Ok(result.Data!, result.Message))
                : BadRequest(ApiResponse<bool>.Fail(result.Message!));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string id, [FromBody] SubjectUpdateDto dto)
        {
            if (id != dto.Id)
                return BadRequest(ApiResponse<string>.Fail("ID mismatch."));

            if (!ModelState.IsValid)
                return BadRequest(ApiResponse<string>.Fail("Invalid subject data."));

            var result = await _subjectService.Update(dto);

            return result.Success
                ? Ok(ApiResponse<SubjectViewDto>.Ok(result.Data!, result.Message))
                : BadRequest(ApiResponse<SubjectViewDto>.Fail(result.Message!));
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var result = await _subjectService.Delete(id);

            return result.Success
                ? Ok(ApiResponse<bool>.Ok(result.Data!, result.Message))
                : BadRequest(ApiResponse<bool>.Fail(result.Message!));
        }

        [HttpGet("grade/{gradeId}")]
        public async Task<IActionResult> GetSubjectsByGrade(string gradeId)
        {
            var result = await _subjectService.GetSubjectsByGradeIdAsync(gradeId);

            return result.Success
                ? Ok(ApiResponse<IEnumerable<SubjectViewDto>>.Ok(result.Data!, result.Message))
                : NotFound(ApiResponse<IEnumerable<SubjectViewDto>>.Fail(result.Message!));
        }

        [HttpGet("count")]
        public async Task<IActionResult> GetSubjectCount()
        {
            var result = await _subjectService.GetSubjectCountAsync();
            if (!result.Success)
                return BadRequest(ApiResponse<int>.Fail(result.Message!));

            return Ok(ApiResponse<int>.Ok(result.Data!, result.Message));
        }
    }
}