using Common.Wrappers;
using Domain.Entities;
using Domain.Interfaces.ApplicationInterfaces;
using Microsoft.AspNetCore.Http;
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
            var admins = await _adminService.GetAllAsync();
            return Ok(ApiResponse<IEnumerable<Admin>>.Ok(admins, "Admins retrieved successfully"));
        }

        //GET: api/Admin/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            var admin = await _adminService.GetByIdAsync(id);
            if (admin == null)
                return NotFound(ApiResponse<Admin>.Fail($"Admin with Id {id} not found."));

            return Ok(ApiResponse<Admin>.Ok(admin, "Admin retrieved successfully"));
        }

        //POST: api/Admin
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Admin admin)
        {
            if (!ModelState.IsValid)
                return BadRequest(ApiResponse<Admin>.Fail("Invalid admin data."));

            var created = await _adminService.CreateAsync(admin);
            return CreatedAtAction(nameof(GetById), new { id = created.Id },
                ApiResponse<Admin>.Ok(created, "Admin created successfully"));
        }

        //PUT: api/Admin/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string id, [FromBody] Admin admin)
        {
            if (id != admin.Id)
                return BadRequest(ApiResponse<Admin>.Fail("ID in URL does not match ID in body."));

            var existing = await _adminService.GetByIdAsync(id);
            if (existing == null)
                return NotFound(ApiResponse<Admin>.Fail($"Admin with Id {id} not found."));

            var updated = await _adminService.UpdateAsync(admin);
            return Ok(ApiResponse<Admin>.Ok(updated, "Admin updated successfully"));
        }

        //DELETE: api/Admin/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var deleted = await _adminService.DeleteAsync(id);
            if (!deleted)
                return NotFound(ApiResponse<string>.Fail($"Admin with Id {id} not found."));

            return Ok(ApiResponse<string>.Ok($"Admin with Id {id} deleted successfully"));
        }
    }
}
