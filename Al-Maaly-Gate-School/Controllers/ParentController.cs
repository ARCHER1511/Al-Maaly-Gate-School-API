using Application.DTOs.ParentDTOs;
using Application.DTOs.StudentDTOs;
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
    public class ParentController : ControllerBase
    {
        private readonly IParentService _ParentService;
        public ParentController(IParentService parentService)
        {
            _ParentService = parentService;
        }
        [HttpGet("all")]
        public async Task<IActionResult> GetAll()
        {
            var result = await _ParentService.GetAllAsync();
            if (!result.Success) return NotFound(ApiResponse<IEnumerable<ParentViewDto>>.Fail(result.Message!));
            return Ok(ApiResponse<IEnumerable<ParentViewDto>>.Ok(result.Data!, result.Message));
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            var result = await _ParentService.GetByIdAsync(id);
            if (!result.Success)
                return NotFound(ApiResponse<ParentViewDto>.Fail(result.Message!));

            return Ok(ApiResponse<ParentViewDto>.Ok(result.Data!, result.Message));
        }

        [HttpPost("create")]
        public async Task<IActionResult> Create([FromBody] ParentCreateUpdateDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ApiResponse<Parent>.Fail("Invalid Parent data."));

            var result = await _ParentService.CreateAsync(dto);

            if (!result.Success)
                return BadRequest(ApiResponse<Parent>.Fail(result.Message!));

            return Ok(ApiResponse<ParentCreateUpdateDto>.Ok(result.Data!, result.Message));
        }

        [HttpPut("update/{id}")]
        public async Task<IActionResult> Update(string id, [FromBody] ParentCreateUpdateDto dto)
        {
            if (id != dto.Id)
                return BadRequest(ApiResponse<ParentCreateUpdateDto>.Fail("ID in URL does not match ID in body."));

            var exists = await _ParentService.GetByIdAsync(id);
            if (!exists.Success)
                return NotFound(ApiResponse<ParentCreateUpdateDto>.Fail(exists.Message!));

            var result = await _ParentService.UpdateAsync(dto);

            if (!result.Success)
                return BadRequest(ApiResponse<ParentCreateUpdateDto>.Fail(result.Message!));

            return Ok(ApiResponse<ParentCreateUpdateDto>.Ok(result.Data!, result.Message));
        }

        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var result = await _ParentService.DeleteAsync(id);

            if (!result.Success)
                return NotFound(ApiResponse<string>.Fail(result.Message!));

            return Ok(ApiResponse<string>.Ok("Student deleted successfully."));
        }

        [HttpGet("parent/view/{id}")]
        public async Task<IActionResult> GetParentWithChildrenAsync(string id)
        {
            var result = await _ParentService.GetParentWithChildrenAsync(id);
            if (!result.Success)
                return NotFound(ApiResponse<ParentViewWithChildrenDto>.Fail(result.Message!));

            return Ok(ApiResponse<ParentViewWithChildrenDto>.Ok(result.Data!, result.Message));
        }

    }
}
