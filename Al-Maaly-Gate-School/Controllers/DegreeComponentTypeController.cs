using Application.DTOs.DegreesDTOs;
using Application.Interfaces;
using Domain.Wrappers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Al_Maaly_Gate_School.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class DegreeComponentTypeController : ControllerBase
    {
        private readonly IDegreeComponentTypeService _componentTypeService;

        public DegreeComponentTypeController(IDegreeComponentTypeService componentTypeService)
        {
            _componentTypeService = componentTypeService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateComponentType([FromBody] CreateDegreeComponentTypeDto dto)
        {
            var result = await _componentTypeService.CreateComponentTypeAsync(dto);
            if (!result.Success)
                return BadRequest(result.Message);
            return Ok(ApiResponse<DegreeComponentTypeDto>.Ok(result.Data!, result.Message));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateComponentType(string id, [FromBody] UpdateDegreeComponentTypeDto dto)
        {
            var result = await _componentTypeService.UpdateComponentTypeAsync(id, dto);
            if (!result.Success)
                return BadRequest(result.Message);
            return Ok(ApiResponse<DegreeComponentTypeDto>.Ok(result.Data!, result.Message));
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteComponentType(string id)
        {
            var result = await _componentTypeService.DeleteComponentTypeAsync(id);
            if (!result.Success)
                return BadRequest(result.Message);
            return Ok(ApiResponse<bool>.Ok(result.Data!, result.Message));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetComponentTypeById(string id)
        {
            var result = await _componentTypeService.GetComponentTypeByIdAsync(id);
            if (!result.Success)
                return BadRequest(result.Message);
            return Ok(ApiResponse<DegreeComponentTypeDto>.Ok(result.Data!, result.Message));
        }

        [HttpGet("subject/{subjectId}")]
        public async Task<IActionResult> GetComponentTypesBySubject(string subjectId)
        {
            var result = await _componentTypeService.GetComponentTypesBySubjectAsync(subjectId);
            if (!result.Success)
                return BadRequest(result.Message);
            return Ok(ApiResponse<List<DegreeComponentTypeDto>>.Ok(result.Data!, result.Message));
        }

        [HttpPut("reorder/{subjectId}")]
        public async Task<IActionResult> ReorderComponentTypes(string subjectId, [FromBody] List<string> componentTypeIds)
        {
            var result = await _componentTypeService.ReorderComponentTypesAsync(subjectId, componentTypeIds);
            if (!result.Success)
                return BadRequest(result.Message);
            return Ok(ApiResponse<bool>.Ok(result.Data!, result.Message));
        }

        [HttpGet("subject/{subjectId}/with-inactive")]
        public async Task<IActionResult> GetAllComponentTypesBySubject(string subjectId)
        {
            // This endpoint returns both active and inactive component types
            // You'll need to add this method to your service
            return Ok("To be implemented");
        }
    }
}