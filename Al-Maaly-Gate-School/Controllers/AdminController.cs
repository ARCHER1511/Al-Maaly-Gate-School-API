using Application.DTOs.AdminDTOs;
using Application.Interfaces;
using Domain.Entities;
using Domain.Wrappers;
using Microsoft.AspNetCore.Mvc;

namespace Al_Maaly_Gate_School.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        private readonly IAdminService _adminService;

        public AdminController(IAdminService adminService)
        {
            _adminService = adminService;
        }

        // GET: api/Admin
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _adminService.GetAllAsync();
            if(!result.Success)
               return NotFound(ApiResponse<IEnumerable<AdminViewDto>>.Fail(result.Message!));
            return Ok(ApiResponse<IEnumerable<AdminViewDto>>.Ok(result.Data!, result.Message));
        }

        //GET: api/Admin/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            var result = await _adminService.GetByIdAsync(id);
            if (!result.Success)
                return NotFound(ApiResponse<AdminViewDto>.Fail(result.Message!));

            return Ok(ApiResponse<AdminViewDto>.Ok(result.Data!, result.Message));
        }

        //POST: api/Admin
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] AdminCreateDto admin)
        {
            if (!ModelState.IsValid)
                return BadRequest(ApiResponse<Admin>.Fail("Invalid admin data."));

            var result = await _adminService.CreateAsync(admin);

            if (!result.Success)
                return BadRequest(ApiResponse<Admin>.Fail(result.Message!));

            return CreatedAtAction(nameof(GetById), new { id = result.Data!.Id },
                ApiResponse<AdminViewDto>.Ok(result.Data, result.Message));
        }

        // PUT: api/Admin/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string id, [FromBody] AdminUpdateDto admin)
        {
            if (id != admin.Id)
                return BadRequest(ApiResponse<AdminViewDto>.Fail("ID in URL does not match ID in body."));

            var exists = await _adminService.GetByIdAsync(id);
            if (!exists.Success)
                return NotFound(ApiResponse<AdminViewDto>.Fail(exists.Message!));

            var result = await _adminService.UpdateAsync(admin);

            if (!result.Success)
                return BadRequest(ApiResponse<AdminViewDto>.Fail(result.Message!));

            return Ok(ApiResponse<AdminViewDto>.Ok(result.Data!, result.Message));
        }

        // DELETE: api/Admin/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var result = await _adminService.DeleteAsync(id);

            if (!result.Success)
                return NotFound(ApiResponse<string>.Fail(result.Message!));

            return Ok(ApiResponse<string>.Ok("Admin deleted successfully."));
        }
    }
}
