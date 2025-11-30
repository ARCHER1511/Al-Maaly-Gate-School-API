using Application.DTOs.ClassDTOs;
using Application.DTOs.GradeDTOs;
using Application.DTOs.SubjectDTOs;
using Application.Interfaces;
using Domain.Wrappers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Al_Maaly_Gate_School.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [AllowAnonymous] // Consider adding proper authorization for production
    public class GradeController : ControllerBase
    {
        private readonly IGradeService _gradeService;

        public GradeController(IGradeService gradeService)
        {
            _gradeService = gradeService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _gradeService.GetAllAsync();
            if (!result.Success)
                return NotFound(ApiResponse<IEnumerable<GradeViewDto>>.Fail(result.Message!));

            return Ok(ApiResponse<IEnumerable<GradeViewDto>>.Ok(result.Data!, result.Message));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            var result = await _gradeService.GetByIdAsync(id);
            if (!result.Success)
                return NotFound(ApiResponse<GradeViewDto>.Fail(result.Message!));

            return Ok(ApiResponse<GradeViewDto>.Ok(result.Data!, result.Message));
        }

        [HttpGet("name/{gradeName}")]
        public async Task<IActionResult> GetByName(string gradeName)
        {
            var result = await _gradeService.GetByNameAsync(gradeName);
            if (!result.Success)
                return NotFound(ApiResponse<GradeViewDto>.Fail(result.Message!));

            return Ok(ApiResponse<GradeViewDto>.Ok(result.Data!, result.Message));
        }

        [HttpGet("{id}/with-details")]
        public async Task<IActionResult> GetWithDetails(string id)
        {
            var result = await _gradeService.GetGradeWithDetailsAsync(id);
            if (!result.Success)
                return NotFound(ApiResponse<GradeWithDetailsDto>.Fail(result.Message!));

            return Ok(ApiResponse<GradeWithDetailsDto>.Ok(result.Data!, result.Message));
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateGradeDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ApiResponse<CreateGradeDto>.Fail("Invalid grade data."));

            var result = await _gradeService.CreateAsync(dto);
            if (!result.Success)
                return BadRequest(ApiResponse<GradeViewDto>.Fail(result.Message!));

            return Ok(ApiResponse<GradeViewDto>.Ok(result.Data!, result.Message));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string id, [FromBody] UpdateGradeDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ApiResponse<UpdateGradeDto>.Fail("Invalid grade data."));

            var result = await _gradeService.UpdateAsync(id, dto);
            if (!result.Success)
                return BadRequest(ApiResponse<GradeViewDto>.Fail(result.Message!));

            return Ok(ApiResponse<GradeViewDto>.Ok(result.Data!, result.Message));
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var result = await _gradeService.DeleteAsync(id);
            if (!result.Success)
                return BadRequest(ApiResponse<bool>.Fail(result.Message!));

            return Ok(ApiResponse<bool>.Ok(result.Data!, result.Message));
        }

        // Class Management Endpoints
        [HttpPost("{gradeId}/classes")]
        public async Task<IActionResult> CreateClass(string gradeId, [FromBody] CreateClassInGradeDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ApiResponse<CreateClassInGradeDto>.Fail("Invalid class data."));

            var result = await _gradeService.AddClassToGradeAsync(gradeId, dto);
            if (!result.Success)
                return BadRequest(ApiResponse<ClassViewDto>.Fail(result.Message!));

            return Ok(ApiResponse<ClassViewDto>.Ok(result.Data!, result.Message));
        }

        [HttpPut("{gradeId}/classes/assign")]
        public async Task<IActionResult> AssignClassToGrade(string gradeId, [FromBody] ClassDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ApiResponse<ClassDto>.Fail("Invalid class data."));

            // This would move an existing class to this grade
            var result = await _gradeService.AddClassToGradeAsync(gradeId, dto);
            if (!result.Success)
                return BadRequest(ApiResponse<ClassViewDto>.Fail(result.Message!));

            return Ok(ApiResponse<ClassViewDto>.Ok(result.Data!, result.Message));
        }

        [HttpGet("{gradeId}/classes")]
        public async Task<IActionResult> GetClasses(string gradeId)
        {
            var result = await _gradeService.GetClassesByGradeIdAsync(gradeId);
            if (!result.Success)
                return NotFound(ApiResponse<IEnumerable<ClassViewDto>>.Fail(result.Message!));

            return Ok(ApiResponse<IEnumerable<ClassViewDto>>.Ok(result.Data!, result.Message));
        }

        // Subject Management Endpoints
        [HttpPost("{gradeId}/subjects")]
        public async Task<IActionResult> AddSubject(string gradeId, [FromBody] SubjectCreateDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ApiResponse<SubjectCreateDto>.Fail("Invalid subject data."));

            var result = await _gradeService.AddSubjectToGradeAsync(gradeId, dto);
            if (!result.Success)
                return BadRequest(ApiResponse<SubjectViewDto>.Fail(result.Message!));

            return Ok(ApiResponse<SubjectViewDto>.Ok(result.Data!, result.Message));
        }

        [HttpGet("{gradeId}/subjects")]
        public async Task<IActionResult> GetSubjects(string gradeId)
        {
            var result = await _gradeService.GetSubjectsByGradeIdAsync(gradeId);
            if (!result.Success)
                return NotFound(ApiResponse<IEnumerable<SubjectViewDto>>.Fail(result.Message!));

            return Ok(ApiResponse<IEnumerable<SubjectViewDto>>.Ok(result.Data!, result.Message));
        }

        // Move Operations
        [HttpPut("classes/{classId}/move/{newGradeId}")]
        public async Task<IActionResult> MoveClass(string classId, string newGradeId)
        {
            var result = await _gradeService.MoveClassToAnotherGradeAsync(classId, newGradeId);
            if (!result.Success)
                return BadRequest(ApiResponse<bool>.Fail(result.Message!));

            return Ok(ApiResponse<bool>.Ok(result.Data!, result.Message));
        }

        [HttpPut("subjects/{subjectId}/move/{newGradeId}")]
        public async Task<IActionResult> MoveSubject(string subjectId, string newGradeId)
        {
            var result = await _gradeService.MoveSubjectToAnotherGradeAsync(subjectId, newGradeId);
            if (!result.Success)
                return BadRequest(ApiResponse<bool>.Fail(result.Message!));

            return Ok(ApiResponse<bool>.Ok(result.Data!, result.Message));
        }

        // Remove Operations
        [HttpDelete("classes/{classId}")]
        public async Task<IActionResult> RemoveClass(string classId)
        {
            var result = await _gradeService.RemoveClassAsync(classId);
            if (!result.Success)
                return BadRequest(ApiResponse<bool>.Fail(result.Message!));

            return Ok(ApiResponse<bool>.Ok(result.Data!, result.Message));
        }

        [HttpDelete("subjects/{subjectId}")]
        public async Task<IActionResult> RemoveSubject(string subjectId)
        {
            var result = await _gradeService.RemoveSubjectAsync(subjectId);
            if (!result.Success)
                return BadRequest(ApiResponse<bool>.Fail(result.Message!));

            return Ok(ApiResponse<bool>.Ok(result.Data!, result.Message));
        }

        // Add to GradeController
        [HttpPost("bulk-move-classes")]
        public async Task<IActionResult> BulkMoveClasses([FromBody] BulkMoveClassesDto dto)
        {
            var result = await _gradeService.BulkMoveClassesAsync(dto);
            if (!result.Success)
                return BadRequest(ApiResponse<bool>.Fail(result.Message!));

            return Ok(ApiResponse<bool>.Ok(result.Data!, result.Message));
        }
    }
}